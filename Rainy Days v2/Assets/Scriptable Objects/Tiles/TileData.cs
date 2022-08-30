using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{

    public TileBase[] Tiles;
    public TileType Type;
    public int PoisonLevel;
    public List<TileData> Neighbours;

}

public enum TileType
{
    Free, Occupied, Locked
}
