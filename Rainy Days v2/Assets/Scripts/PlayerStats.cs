using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{

    public int MaxToxicity { get; set; }
    public int CurrentToxicity { get; set; }

    public PlayerStats(int maxHealth, int maxAP, int maxToxicity)
    {
        this.MaxHealth = maxHealth;
        this.CurrentHealth = maxHealth;
        this.MaxToxicity = maxToxicity;
        this.CurrentToxicity = 0;
        this.MaxAP = maxAP;
        this.CurrentAP = maxAP;
    }

}
