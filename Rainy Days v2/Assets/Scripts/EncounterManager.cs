using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public Encounter CurrentEncounter { get; set; }
    public StatsPriorityQueue Queue { get; set; } = new();
    public List<Stats> Stats { get; set; } = new();
    public List<Stats> DeadStats { get; set; } = new();

    public void SomebodyDies(Stats stats)
    {
        Stats.Remove(stats);
        DeadStats.Add(stats);
        if (Stats.Count == 1)
        {
            EndEncounter();
        }
    }

    public void StartEncounter(List<Stats> stats, Encounter encounter)
    {
        CurrentEncounter = encounter;
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
        {
            Stats next = Queue.Dequeue();
            if (!DeadStats.Contains(next))
                next.MakeTurn();
            else
                NextTurn();
        }
        else
            NextRound();
    }

    public void EndEncounter()
    {
        Stats.Clear();
        DeadStats.Clear();
        Queue.Entries.Clear();
        FindObjectOfType<HudFunctions>().EndEncounter();
        FindObjectOfType<Player>().EndCombat();
        CurrentEncounter.EndEncounter();
    }

}
