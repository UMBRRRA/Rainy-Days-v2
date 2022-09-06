using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemObject item;
    public int amount;
    public InventoryObject inventory;

    public void AddItem()
    {
        inventory.AddItem(item, amount);
    }

}

