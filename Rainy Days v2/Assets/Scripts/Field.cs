using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public TileData TileData { get; set; }
    public Vector3Int GridPosition { get; set; }
    public List<Field> Neighbours { get; set; } = new();
    public List<Field> Path { get; set; } = new();
    public float DijkstraScore { get; set; } = Mathf.Infinity;
    public float BestScore { get; set; } = Mathf.Infinity;
    public float Heuristic { get; set; }
    public Field(Vector3Int gridPos, TileData tile, float heuristic)
    {
        this.TileData = tile;
        this.GridPosition = gridPos;
        this.Heuristic = heuristic;
    }
    public override string ToString()
    {
        return GridPosition.ToString();
    }
}
