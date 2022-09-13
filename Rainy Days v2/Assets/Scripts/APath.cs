using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class APath
{
    public List<Field> Fields { get; set; }
    public float AScore { get; set; }
    public float DijkstraScore { get; set; }

    public APath(List<Field> path, float aScore, float dijkstraScore)
    {
        this.Fields = path;
        this.AScore = aScore;
        this.DijkstraScore = dijkstraScore;
    }

    public override string ToString()
    {
        string result = "";
        Fields.ForEach(f => result += f);
        return result;
    }
}
