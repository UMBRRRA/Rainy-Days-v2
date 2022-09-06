using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> list = new List<InventorySlot>();

    public void AddItem(ItemObject item, int amount)
    {
        bool hasItem = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].item == item)
            {
                list[i].AddAmount(amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            list.Add(new InventorySlot(item, amount));
        }
    }

    public bool TakeItem(ItemObject item, int amount)
    {
        bool worked = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].item == item)
            {
                if (list[i].amount >= amount)
                {
                    worked = true;
                    list[i].DecreaseAmount(amount);
                    if (list[i].amount == 0)
                    {
                        list.RemoveAt(i);
                    }
                }
                break;
            }
        }
        return worked;
    }

    public bool HasItem(ItemObject item, int amount)
    {
        bool has = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].item == item)
            {
                if (list[i].amount >= amount)
                {
                    has = true;
                }
                break;
            }
        }
        return has;
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public void DecreaseAmount(int value)
    {
        amount -= value;
    }

}

