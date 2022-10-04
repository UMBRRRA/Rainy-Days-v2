using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyState
{
    Neutral, Moving, Action
}

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

    public float timeForMove;

    public float timeForCornerMove;

    public float enemyZ = 1.2f;

    public abstract void SetupSlider();

    public abstract void MakeTurn();

    public abstract void ShootMe();

    public abstract void MeleeMe();
    public abstract void SnipeMe();

    public abstract void FlurryMe();

    public abstract void TakeDamage(int damage);

    public abstract void ActivateSlider();

    public abstract void DeactivateSlider();

    public abstract IEnumerator MoveToPosition(Vector3 position, float timeTo);

    public abstract void PlayWalk(Vector3Int walkDir);

    public abstract IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nextField);

    public EnemyState State { get; set; } = EnemyState.Neutral;

}
