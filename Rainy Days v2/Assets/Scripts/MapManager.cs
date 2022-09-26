using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class MapManager : MonoBehaviour
{
    public Tilemap map;
    public List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public Player player;
    public List<Vector3Int> OccupiedFields { get; set; } = new();
    public Dictionary<Vector3Int, TransitionField> TransitionFields { get; set; } = new();
    public Dictionary<Vector3Int, Encounter> EncounterFields { get; set; } = new();

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        OccupiedFields.Clear();
        TransitionFields.Clear();
        StartCoroutine(FindMap());
        SpawnPlayer();
        // reading all kinds of tiles
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.Tiles)
            {
                dataFromTiles.Add(tile, tileData);

            }
        }
    }

    public IEnumerator FindMap()
    {
        yield return new WaitUntil(() => (map = FindObjectOfType<Tilemap>()) != null);
    }


    void Update()
    {
        // find a* path on click
        if (Input.GetMouseButtonDown(0) && player.State == PlayerState.Neutral && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            WhenClicked(gridPosition);
        }

    }


    public void WhenClicked(Vector3Int gridPosition)
    {
        if (!player.inFight)
        {
            if (map.GetTile(gridPosition) != null && gridPosition != player.currentGridPosition)
            {
                if (CheckIfTileIsWalkable(dataFromTiles[map.GetTile(gridPosition)]) && CheckIfFieldIsFree(gridPosition))
                {
                    APath path = FindAPath(player.currentGridPosition, gridPosition);
                    StartMoving(path);

                    if (TransitionFields.Keys.Contains(gridPosition))
                    {
                        StartCoroutine(Transition(gridPosition));
                        Debug.Log("Transiting");
                    }

                    if (EncounterFields.Keys.Contains(gridPosition))
                    {
                        StartCoroutine(StartEncounter(gridPosition));
                    }
                }

            }
        }
        else
        {
            if (map.GetTile(gridPosition) != null && gridPosition != player.currentGridPosition)
            {
                if (CheckIfTileIsWalkable(dataFromTiles[map.GetTile(gridPosition)]) && CheckIfFieldIsFree(gridPosition))
                {
                    APath path = FindAPath(player.currentGridPosition, gridPosition);
                    // if a path != null...
                    int apcost = (int)Math.Round(path.DijkstraScore / player.Stats.Movement);
                    if (apcost == 0)
                        apcost = 1;
                    if (player.UseAP(apcost))
                    {
                        StartMoving(path);
                    }
                    else
                    {
                        while (!player.UseAP(apcost))
                        {
                            if (path.Fields.Count == 2)
                            {
                                return;
                            }
                            Field[] pathFields = path.Fields.ToArray();
                            path = FindAPath(player.currentGridPosition, pathFields[pathFields.Length - 2].GridPosition);
                            apcost = (int)Math.Round(path.DijkstraScore / player.Stats.Movement);
                        }
                        StartMoving(path);
                    }

                }
            }
        }
    }

    public void MovePlayerTodialogue(Vector3Int gridPosition, DialogueTrigger dialogue)
    {
        if (!player.inFight && player.State == PlayerState.Neutral)
        {
            int dialogueRange = 1;
            if (StaticHelpers.CheckIfTargetIsInRange(gridPosition, player.currentGridPosition, dialogueRange))
            {
                StartCoroutine(ApproachDialogue(dialogue));
            }
            else
            {
                APath path = ChooseBestAPath(player.currentGridPosition, FindNeighboursOfRange(gridPosition, dialogueRange));
                StartMoving(path);
                StartCoroutine(ApproachDialogue(dialogue));
            }
        }
    }

    public void StartMoving(APath path)
    {
        player.State = PlayerState.Moving;
        StopAllCoroutines();
        StartCoroutine(WalkPath(path));
    }

    public IEnumerator StartEncounter(Vector3Int gridPos)
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        EncounterFields[gridPos].StartEncounter();
    }


    public IEnumerator ApproachDialogue(DialogueTrigger dialogue)
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        dialogue.TriggerDialogue();
    }

    public IEnumerator Transition(Vector3Int gridPos)
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        TransitionFields[gridPos].MakeTransition();
    }

    public IEnumerator WalkPath(APath path)
    {

        //OccupiedFields.Remove(player.currentGridPosition);

        Vector3 nf = new(0, 0, 0);
        Field[] fields = path.Fields.ToArray();
        int toxicity = 0;
        foreach (Field f in fields)
        {
            toxicity += f.TileData.PoisonLevel;
        }

        float timeForPlayerMove = player.timeForMove;

        for (int i = 1; i < fields.Length; i++)
        {
            Vector3 cf = map.CellToWorld(fields[i - 1].GridPosition);
            nf = map.CellToWorld(fields[i].GridPosition);
            Vector3 ne = new(1, 0, 0);
            Vector3 sw = new(-1, 0, 0);
            yield return new WaitForSeconds(timeForPlayerMove);

            if (nf - cf == ne || nf - cf == sw)
            {
                timeForPlayerMove = player.timeForCornerMove;
            }
            else
            {
                timeForPlayerMove = player.timeForMove;
            }

            StartCoroutine(player.MoveToPosition(new(nf.x, nf.y + 0.25f, player.playerZ), timeForPlayerMove));
            Vector3Int walkDir = fields[i].GridPosition - fields[i - 1].GridPosition;
            player.PlayWalk(walkDir);
        }
        player.currentGridPosition = fields[fields.Length - 1].GridPosition;
        Vector3Int idleDir = fields[fields.Length - 1].GridPosition - fields[fields.Length - 2].GridPosition;
        StartCoroutine(player.PlayIdle(idleDir, nf));
        player.IncreaseToxicity(toxicity);

        // OccupiedFields.Add(player.currentGridPosition);

    }

    public void MoveEnemy(Enemy enemy, APath path)
    {
        StartCoroutine(MoveEnemyCo(enemy, path));
    }

    private IEnumerator MoveEnemyCo(Enemy enemy, APath path)
    {
        //OccupiedFields.Remove(enemy.CurrentGridPosition);

        Vector3 nf = new(0, 0, 0);
        Field[] fields = path.Fields.ToArray();

        float timeForMove = enemy.timeForMove;

        for (int i = 1; i < fields.Length; i++)
        {
            Vector3 cf = map.CellToWorld(fields[i - 1].GridPosition);
            nf = map.CellToWorld(fields[i].GridPosition);
            Vector3 ne = new(1, 0, 0);
            Vector3 sw = new(-1, 0, 0);
            yield return new WaitForSeconds(timeForMove);

            if (nf - cf == ne || nf - cf == sw)
            {
                timeForMove = enemy.timeForCornerMove;
            }
            else
            {
                timeForMove = enemy.timeForMove;
            }

            StartCoroutine(enemy.MoveToPosition(new(nf.x, nf.y + 0.25f, enemy.enemyZ), timeForMove));
            Vector3Int walkDir = fields[i].GridPosition - fields[i - 1].GridPosition;
            enemy.PlayWalk(walkDir);
        }
        enemy.CurrentGridPosition = fields[fields.Length - 1].GridPosition;
        Vector3Int idleDir = fields[fields.Length - 1].GridPosition - fields[fields.Length - 2].GridPosition;
        StartCoroutine(enemy.PlayIdle(idleDir, nf));

        OccupiedFields.Add(enemy.CurrentGridPosition);

    }

    public void MoveNPC(NPC npc, APath path)
    {
        StartCoroutine(MoveNPCCo(npc, path));
    }

    private IEnumerator MoveNPCCo(NPC npc, APath path)
    {
        Vector3 nf = new(0, 0, 0);
        Field[] fields = path.Fields.ToArray();

        float timeForMove = npc.timeForMove;

        for (int i = 1; i < fields.Length; i++)
        {
            Vector3 cf = map.CellToWorld(fields[i - 1].GridPosition);
            nf = map.CellToWorld(fields[i].GridPosition);
            Vector3 ne = new(1, 0, 0);
            Vector3 sw = new(-1, 0, 0);
            yield return new WaitForSeconds(timeForMove);

            if (nf - cf == ne || nf - cf == sw)
            {
                timeForMove = npc.timeForCornerMove;
            }
            else
            {
                timeForMove = npc.timeForMove;
            }

            StartCoroutine(npc.MoveToPosition(new(nf.x, nf.y + 0.25f, npc.npcZ), timeForMove));
            Vector3Int walkDir = fields[i].GridPosition - fields[i - 1].GridPosition;
            npc.PlayWalk(walkDir);
        }
        npc.CurrentGridPosition = fields[fields.Length - 1].GridPosition;
        Vector3Int idleDir = fields[fields.Length - 1].GridPosition - fields[fields.Length - 2].GridPosition;
        StartCoroutine(npc.PlayIdle(idleDir, nf));

        if (npc.type == NPCType.Normal)
        {

            OccupiedFields.Add(npc.CurrentGridPosition);
        }

    }

    public APath FindAPath(Vector3Int start, Vector3Int finish)
    {
        int firstX, lastX, firstY, lastY;
        firstX = map.origin.x;
        firstY = map.origin.y;
        lastX = firstX + map.size.x;
        lastY = firstY + map.size.y;
        Dictionary<Vector3Int, Field> fields = new();
        const float heuristicScale = 1.5f;
        // browse all tiles
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                Vector3Int gridPos = new(x, y);
                if (map.GetTile(gridPos) != null)
                {
                    TileData tile = dataFromTiles[map.GetTile(gridPos)];
                    if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(gridPos))
                    {
                        float heuristic = Vector3Int.Distance(gridPos, finish) * heuristicScale;
                        Field field = new(gridPos, tile, heuristic);
                        fields.Add(gridPos, field);
                    }
                }

            }
        }



        List<Field> done = new();
        Field startField = fields[start];
        Field finishField = fields[finish];
        Field currentField = startField;
        FieldsPriorityQueue queue = new();
        startField.BestScore = 0 + startField.Heuristic;
        startField.DijkstraScore = 0;
        startField.Path.Add(startField);



        while (currentField != finishField)
        {
            FindNeighbours(currentField, fields);

            foreach (Field f in currentField.Neighbours)
            {
                if (!done.Contains(f))
                {
                    float score = currentField.DijkstraScore + Distance(currentField, f);
                    if (score + f.Heuristic < f.BestScore)
                    {
                        f.BestScore = score + f.Heuristic;
                        f.DijkstraScore = score;
                        queue.Enqueue(f);

                        f.Path.Clear();
                        foreach (Field field in currentField.Path)
                        {
                            f.Path.Add(field);
                        }
                        f.Path.Add(f);
                    }
                }
            }
            done.Add(currentField);

            if (queue.Entries.Count == 0)
            {
                Debug.Log("Can't find path.");
                return null;
            }
            currentField = queue.Dequeue();
        }
        APath bestPath = new(finishField.Path, finishField.BestScore, finishField.DijkstraScore);
        return bestPath;
    }

    private void FindNeighbours(Field field, Dictionary<Vector3Int, Field> fields)
    {
        Vector3Int s = new(field.GridPosition.x - 1, field.GridPosition.y);
        if (map.GetTile(s) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(s)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(s))
            {
                field.Neighbours.Add(fields[s]);
            }
        }
        // north
        Vector3Int n = new(field.GridPosition.x + 1, field.GridPosition.y);
        if (map.GetTile(n) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(n)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(n))
            {
                field.Neighbours.Add(fields[n]);
            }
        }
        // east
        Vector3Int e = new(field.GridPosition.x, field.GridPosition.y - 1);
        if (map.GetTile(e) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(e)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(e))
            {
                field.Neighbours.Add(fields[e]);
            }
        }
        // west
        Vector3Int w = new(field.GridPosition.x, field.GridPosition.y + 1);
        if (map.GetTile(w) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(w)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(w))
            {
                field.Neighbours.Add(fields[w]);
            }
        }
        // north east
        Vector3Int ne = new(field.GridPosition.x + 1, field.GridPosition.y - 1);
        if (map.GetTile(ne) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(ne)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(ne))
            {
                field.Neighbours.Add(fields[ne]);
            }
        }
        // north west
        Vector3Int nw = new(field.GridPosition.x + 1, field.GridPosition.y + 1);
        if (map.GetTile(nw) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(nw)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(nw))
            {
                field.Neighbours.Add(fields[nw]);
            }
        }
        // south east
        Vector3Int se = new(field.GridPosition.x - 1, field.GridPosition.y - 1);
        if (map.GetTile(se) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(se)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(se))
            {
                field.Neighbours.Add(fields[se]);
            }
        }
        // south west
        Vector3Int sw = new(field.GridPosition.x - 1, field.GridPosition.y + 1);
        if (map.GetTile(sw) != null)
        {
            TileData tile = dataFromTiles[map.GetTile(sw)];
            if (CheckIfTileIsWalkable(tile) && CheckIfFieldIsFree(sw))
            {
                field.Neighbours.Add(fields[sw]);
            }
        }
    }

    private float Distance(Field from, Field to)
    {
        float result = 1f; // scale it perchance
        if (from.GridPosition.x != to.GridPosition.x &&
            from.GridPosition.y != to.GridPosition.y) // that went suprisingly well...
        {
            result *= 1.41f;
        }
        // return result * to.TileData.PoisonLevel;
        return result;
    }

    public bool CheckIfTileIsWalkable(TileData tile)
    {
        if (tile.Type == TileType.Locked)
            return false;
        else
            return true;
    }

    private bool CheckIfFieldIsFree(Vector3Int gridPos)
    {
        if (OccupiedFields.Contains(gridPos))
            return false;
        else
            return true;
    }


    public void SpawnPlayer()
    {
        Player test = FindObjectOfType<Player>();
        if (test == null)
        {
            Vector3Int spawn = new(player.spawnGridPosition.x, player.spawnGridPosition.y, 1);
            player = Instantiate(player, map.CellToWorld(spawn), Quaternion.identity);
            player.currentGridPosition = player.spawnGridPosition;
        }
        else
        {
            player = test;
            player.mapManager = this;
        }
    }

    public List<Vector3Int> FindNeighboursOfRange(Vector3Int pos, int range)
    {
        List<Vector3Int> neighbours = new();

        for (int x = -range; x <= range; x++)
        {
            Vector3Int neighbourPosUp = new(pos.x + x, pos.y + range);
            if (map.GetTile(neighbourPosUp) != null)
            {
                if (CheckIfTileIsWalkable(dataFromTiles[map.GetTile(neighbourPosUp)]) && CheckIfFieldIsFree(neighbourPosUp))
                {
                    neighbours.Add(neighbourPosUp);
                }
            }
            Vector3Int neighbourPosDown = new(pos.x + x, pos.y - range);
            if (map.GetTile(neighbourPosDown) != null)
            {
                if (CheckIfTileIsWalkable(dataFromTiles[map.GetTile(neighbourPosDown)]) && CheckIfFieldIsFree(neighbourPosDown))
                {
                    neighbours.Add(neighbourPosDown);
                }
            }
        }

        for (int y = -range + 1; y < range; y++)
        {
            Vector3Int neighbourPosUp = new(pos.x + range, pos.y + y);
            if (map.GetTile(neighbourPosUp) != null)
            {
                if (CheckIfTileIsWalkable(dataFromTiles[map.GetTile(neighbourPosUp)]) && CheckIfFieldIsFree(neighbourPosUp))
                {
                    neighbours.Add(neighbourPosUp);
                }
            }
            Vector3Int neighbourPosDown = new(pos.x - range, pos.y + y);
            if (map.GetTile(neighbourPosDown) != null)
            {
                if (CheckIfTileIsWalkable(dataFromTiles[map.GetTile(neighbourPosDown)]) && CheckIfFieldIsFree(neighbourPosDown))
                {
                    neighbours.Add(neighbourPosDown);
                }
            }
        }

        foreach (Vector3Int n in neighbours)
        {
            Debug.Log(n);
        }

        return neighbours;
    }


    public APath ChooseBestAPath(Vector3Int start, List<Vector3Int> candidates)
    {
        float bestScore = float.MaxValue;
        APath bestPath = null;
        foreach (Vector3Int candidate in candidates)
        {
            APath path = FindAPath(start, candidate);
            if (path.AScore < bestScore)
            {
                bestScore = path.AScore;
                bestPath = path;
            }
        }
        return bestPath;
    }

}
