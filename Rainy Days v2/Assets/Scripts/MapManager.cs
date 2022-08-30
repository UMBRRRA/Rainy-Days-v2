using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public Tilemap map;
    public List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public Player player;

    void Start()
    {
        // reading all kinds of tiles
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.Tiles)
            {
                dataFromTiles.Add(tile, tileData);

            }
        }

        SpawnPlayer();

        //

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            TileBase clickedTile = map.GetTile(gridPosition);
            int poisonLevel = dataFromTiles[clickedTile].PoisonLevel;
            TileType type = dataFromTiles[clickedTile].Type;
            Debug.Log($"At position {gridPosition} there is a {type} tile with poison level {poisonLevel}.");
        }

        // find a* path on click
        if (Input.GetMouseButtonDown(0) && player.State == PlayerState.Neutral)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            APath path = FindAPath(player.currentGridPosition, gridPosition);
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
        // browse all tiles
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                Vector3Int gridPos = new(x, y);
                if (map.GetTile(gridPos) != null)
                {
                    TileData tile = dataFromTiles[map.GetTile(gridPos)];
                    if (CheckIfTileIsWalkable(tile))
                    {
                        float heuristic = Vector3Int.Distance(start, finish);
                        Field field = new(gridPos, tile, heuristic);
                        fields.Add(gridPos, field);
                    }
                }

            }
        }
        // test
        foreach (Field field in fields.Values)
        {
            Debug.Log(field);
        }
        /*
        foreach (Field field in fields.Values)
        {
            // neighbours

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; x <= 1; y++)
                {
                    if (x != 0 && y != 0)
                    {
                        Vector3Int gridPos = new(field.GridPosition.x + x, field.GridPosition.y + y);
                        if (map.GetTile(gridPos) != null)
                        {
                            TileData tile = dataFromTiles[map.GetTile(gridPos)];
                            if (CheckIfTileIsWalkable(tile))
                            {
                                field.Neighbours.Add(fields[gridPos]);
                            }
                        }
                    }
                }
            }
        }
        // test
        /*
        foreach (Field field in fields.Values)
        {
            string s = $"I am {field} and my neighbours are: ";
            foreach (Field neighbour in field.Neighbours)
            {
                s += neighbour + "    ";
            }
            Debug.Log(s);
        }
        */

        return null;
    }

    public bool CheckIfTileIsWalkable(TileData tile)
    {
        if (tile.Type == TileType.Locked)
            return false;
        else
            return true;
    }

    public void SpawnPlayer()
    {
        Instantiate(player, map.CellToWorld(player.spawnGridPosition), Quaternion.identity);
    }
}
