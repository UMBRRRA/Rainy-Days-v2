using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    public string enemyName;

    public int spawnDirection;

    public int CurrentDirection { get; set; }
    public EnemyStats Stats { get; set; }
    public Vector3Int CurrentGridPosition { get; set; }

    public int exp;

    public Slider slider;

    public GameObject sliderParent;

    public abstract void SetupSlider();

    public abstract void MakeTurn();

    public abstract void ShootMe();

    public abstract void MeleeMe();

    public abstract void TakeDamage(int damage);

    public abstract void ActivateSlider();

    public abstract void DeactivateSlider();

}
