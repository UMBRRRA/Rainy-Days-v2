using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    public int zombieMaxHealth, zombieMaxAP, zombieInitiative;
    public float zombieMovement;
    private Player player;
    private MapManager mapManager;
    private Animator animator;
    public float hitTime = 0.6f;
    private float beforeHitTime; // if theres more attacks gotta fix this
    //private float beforeDeathTime;

    public override void MakeTurn()
    {
        StartCoroutine(ZombieTurn());
    }

    public IEnumerator ZombieTurn()
    {
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
        yield return new WaitForSeconds(3f);
        FindObjectOfType<EncounterManager>().NextTurn();
    }
    private void OnMouseDown()
    {
        if (player.State == PlayerState.Shooting)
        {
            ShootMe();
        }
        else if (player.State == PlayerState.Meleeing)
        {
            MeleeMe();
        }
    }

    public override void ShootMe()
    {
        beforeHitTime = 0.2f;
        player.Shoot(this);
    }

    public override void MeleeMe()
    {
        beforeHitTime = 0.4f;
        player.Melee(this);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        IdleDirection(spawnDirection);
        Stats = new EnemyStats(this, zombieMaxHealth, zombieMaxAP, zombieInitiative, zombieMovement);
        SetupSlider();
        StartCoroutine(FindPlayer());
        StartCoroutine(FindMapManager());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
    }
    private IEnumerator FindMapManager()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        Vector3 myZ0transform = new(transform.position.x, transform.position.y, 0);
        Vector3Int myGridPos = mapManager.map.WorldToCell(myZ0transform);
        CurrentGridPosition = new Vector3Int(myGridPos.x, myGridPos.y, 0);
    }

    private void IdleDirection(int dir)
    {
        animator.SetInteger("IdleDirection", dir);
        CurrentDirection = dir;
    }

    public override void TakeDamage(int damage)
    {
        int healthLeft = Stats.CurrentHealth -= damage;
        if (healthLeft > 0)
        {
            Stats.CurrentHealth = healthLeft;
            slider.value = Stats.CurrentHealth;
            StartCoroutine(BeforeHit());
        }
        else
        {
            Stats.CurrentHealth = 0;
            slider.value = Stats.CurrentHealth;
            StartCoroutine(BeforeDead());
        }
    }

    private IEnumerator BeforeDead()
    {
        yield return new WaitForSeconds(beforeHitTime);
        animator.SetBool("Idle", false);
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Death", true);
        animator.SetInteger("DeathDirection", CurrentDirection);

        GetComponent<BoxCollider2D>().enabled = false;
        DeactivateSlider();
        player.exp += exp;
        FindObjectOfType<EncounterManager>().SomebodyDies(Stats);

    }

    private IEnumerator BeforeHit()
    {
        yield return new WaitForSeconds(beforeHitTime);
        animator.SetBool("Idle", false);
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Hit", true);
        animator.SetInteger("HitDirection", CurrentDirection);
        StartCoroutine(AfterHit());
    }

    private IEnumerator AfterHit()
    {
        yield return new WaitForSeconds(hitTime);
        animator.SetBool("Hit", false);
        animator.SetInteger("HitDirection", 0);
        animator.SetBool("Idle", true);
        animator.SetInteger("IdleDirection", CurrentDirection);
    }

    public override void SetupSlider()
    {
        slider.maxValue = Stats.MaxHealth;
        slider.value = Stats.CurrentHealth;
    }

    public override void ActivateSlider()
    {
        sliderParent.SetActive(true);
    }

    public override void DeactivateSlider()
    {
        sliderParent.SetActive(false);
    }
}
