using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{

    public EnemyStats Stats { get; set; }
    public Vector3Int currentGridPosition;

    public abstract void MakeTurn();

    public abstract void ShootMe();

}
