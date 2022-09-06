using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Usable,
    Quest
}

[CreateAssetMenu(fileName = "New Item Object", menuName = "Inventory System/Item")]
public class ItemObject : ScriptableObject
{
    public int id;
    public ItemType type;
    [TextArea(1, 1)]
    public string title;
}
