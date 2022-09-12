using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{

    public EnemyStats Stats { get; set; }

    public abstract void MakeTurn();

}
