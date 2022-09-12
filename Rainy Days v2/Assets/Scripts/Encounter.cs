using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public int firstX, lastX, firstY, lastY;
    private MapManager mapManager;
    private Player player;
    private EncounterManager encounterManager;
    public List<Enemy> enemies;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindMap());
    }

    public IEnumerator FindMap()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                mapManager.EncounterFields.Add(new Vector3Int(x, y, 0), this);
            }
        }
    }



    public void StartEncounter()
    {
        Debug.Log("Starting Encounter");
        StartCoroutine(FindPlayer());
    }

    public IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.inFight = true;
        StartCoroutine(FindEncounterManager());
    }


    public IEnumerator FindEncounterManager()
    {
        yield return new WaitUntil(() => (encounterManager = FindObjectOfType<EncounterManager>()) != null);
        player.State = PlayerState.NotMyTurn;
        List<Stats> stats = new();
        stats.Add(player.Stats);
        enemies.ForEach(e => stats.Add(e.Stats));
        encounterManager.StartEncounter(stats);
    }

}
