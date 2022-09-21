using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPriorityQueue
{
    public LinkedList<Stats> Entries { get; } = new LinkedList<Stats>();

    public Stats Dequeue()
    {
        if (Entries.Count > 0)
        {
            var itemTobeRemoved = Entries.First.Value;
            Entries.RemoveFirst();
            return itemTobeRemoved;
        }
        else
            return null;
    }

    public void Enqueue(Stats entry)
    {
        var value = new LinkedListNode<Stats>(entry);
        if (Entries.First == null)
        {
            Entries.AddFirst(value);
        }
        else
        {
            var ptr = Entries.First;
            while (ptr.Next != null && ptr.Value.RolledInitiative > entry.RolledInitiative)
            {
                ptr = ptr.Next;
            }

            if (ptr.Value.RolledInitiative >= entry.RolledInitiative)
            {
                Entries.AddAfter(ptr, value);
            }
            else
            {
                Entries.AddBefore(ptr, value);
            }
        }
    }

}
