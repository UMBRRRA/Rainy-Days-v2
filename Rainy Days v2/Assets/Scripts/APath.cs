using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class APath
{
    public List<Field> Fields { get; set; }
    public float Score { get; set; }

    public APath(List<Field> path, float score)
    {
        this.Fields = path;
        this.Score = score;
    }

    public override string ToString()
    {
        string result = "";
        Fields.ForEach(f => result += f);
        return result;
    }
}
