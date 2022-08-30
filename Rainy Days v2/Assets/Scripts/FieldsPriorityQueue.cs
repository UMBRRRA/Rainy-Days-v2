using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldsPriorityQueue
{
    public LinkedList<Field> Entries { get; } = new LinkedList<Field>();

    public Field Dequeue()
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

    public void Enqueue(Field entry)
    {
        var value = new LinkedListNode<Field>(entry);
        if (Entries.First == null)
        {
            Entries.AddFirst(value);
        }
        else
        {
            var ptr = Entries.First;
            while (ptr.Next != null && ptr.Value.BestScore < entry.BestScore)
            {
                ptr = ptr.Next;
            }

            if (ptr.Value.BestScore <= entry.BestScore)
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
