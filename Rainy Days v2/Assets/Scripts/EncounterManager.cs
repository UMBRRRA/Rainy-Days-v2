using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{

    public StatsPriorityQueue Queue { get; set; } = new();
    public List<Stats> Stats { get; set; } = new();


    void Start()
    {

    }

    public void StartEncounter(List<Stats> stats)
    {
        Debug.Log($"there is {stats.Count} in fight");
        FindObjectOfType<HudFunctions>().StartEncounter();
        Queue.Entries.Clear();
        Stats = stats;
        NextRound();
    }

    public void NextRound()
    {
        Queue = new StatsPriorityQueue();
        foreach (Stats stat in Stats)
        {
            Queue.Enqueue(stat);
        }
        NextTurn();
    }

    public void NextTurn()
    {
        if (Queue.Entries.Count != 0)
            Queue.Dequeue().MakeTurn();
        else
            NextRound();
    }

    public void EndEncounter()
    {
        Stats.Clear();
        Queue.Entries.Clear();
        FindObjectOfType<HudFunctions>().EndEncounter();
        FindObjectOfType<Player>().inFight = false;
    }

}
