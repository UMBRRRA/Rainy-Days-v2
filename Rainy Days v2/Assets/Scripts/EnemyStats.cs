using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{

    public Enemy Enemy { get; set; }

    public EnemyStats(Enemy enemy, int maxHealth, int maxAP, int initiative)
    {
        this.MaxHealth = maxHealth;
        this.CurrentHealth = maxHealth;
        this.MaxAP = maxAP;
        this.CurrentAP = maxAP;
        this.Enemy = enemy;
        this.Initiative = initiative;
    }

    public override void MakeTurn()
    {
        Enemy.MakeTurn();
    }

}
