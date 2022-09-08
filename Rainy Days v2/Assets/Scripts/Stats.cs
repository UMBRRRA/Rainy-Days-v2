using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stats
{

    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxAP { get; set; }
    public int CurrentAP { get; set; }
    public int Initiative { get; set; }
    public float Movement { get; set; }


}
