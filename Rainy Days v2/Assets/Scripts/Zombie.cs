using System;
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

    // public SpriteRenderer sprite;

    public float hammerTime = 1.2f;
    public int hammerApCost = 1;
    private bool myTurn = false;
    private bool moveFinished = true;

    public int hammerMinDice = 2;
    public int hammerMaxDice = 16;
    public int hammerModifier = 5;

    public int pukeApCost = 5;
    public float pukeTime = 1.17f;

    public int pukeMinDice = 1;
    public int pukeMaxDice = 6;
    public int pukeModifier = 2;
    public int pukeToxicMinDice = 2;
    public int pukeToxicMaxDice = 12;
    public int pukeToxicModifier = 3;


    public override void MakeTurn()
    {
        StartCoroutine(ZombieTurn());
    }

    public IEnumerator ZombieTurn()
    {
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
        Stats.CurrentAP = Stats.MaxAP;
        myTurn = true;
        StartCoroutine(MakeMove());
        yield return new WaitUntil(() => !myTurn);
        FindObjectOfType<EncounterManager>().NextTurn();
    }

    private IEnumerator MakeMove()
    {
        yield return new WaitUntil(() => moveFinished || !myTurn);
        if (myTurn)
        {
            moveFinished = false;
            ChooseMove();
            StartCoroutine(WaitAndMakeMove());
        }
    }

    private void ChooseMove()
    {
        int random = UnityEngine.Random.Range(1, 5);
        if (random >= 1 && random <= 3)
        {
            if (!Hammer())
            {
                //if (!Puke())
                UseResttoMoveUp();
            }
        }
        else
        {
            if (!Puke())
            {
                UseResttoMoveUp();
            }
        }
    }

    private IEnumerator WaitAndMakeMove()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(MakeMove());
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
        mapManager.OccupiedFields.Add(CurrentGridPosition);
    }

    public bool UseAP(int apCost)
    {
        if (Stats.CurrentAP - apCost >= 0)
        {
            Stats.CurrentAP -= apCost;
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool UseResttoMoveUp()
    {
        if (StaticHelpers.CheckIfTargetIsInRange(player.currentGridPosition, CurrentGridPosition, 1))
        {
            myTurn = false;
            moveFinished = true;
            return true;
        }
        mapManager.OccupiedFields.Remove(CurrentGridPosition);
        APath path = mapManager.ChooseBestAPath(CurrentGridPosition, mapManager.FindNeighboursOfRange(player.currentGridPosition, 1));
        if (path == null)
        {
            mapManager.OccupiedFields.Add(CurrentGridPosition);
            StartCoroutine(WaitAndEndTurn());
            return false;
        }
        int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
        if (apcost == 0)
            apcost = 1;
        while (!UseAP(apcost))
        {
            if (path.Fields.Count == 2)
            {
                myTurn = false;
                moveFinished = true;
                mapManager.OccupiedFields.Add(CurrentGridPosition);
                return false;
            }
            Field[] pathFields = path.Fields.ToArray();
            path = mapManager.FindAPath(CurrentGridPosition, pathFields[pathFields.Length - 2].GridPosition);
            apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
        }
        State = EnemyState.Moving;
        mapManager.MoveEnemy(this, path);
        StartCoroutine(WaitAndEndTurn());
        return true;
    }

    private IEnumerator WaitAndEndTurn()
    {
        yield return new WaitUntil(() => State == EnemyState.Neutral);
        myTurn = false;
        moveFinished = true;
    }

    public bool Hammer()
    {
        int meleeRange = 1;
        if (StaticHelpers.CheckIfTargetIsInRange(player.currentGridPosition, CurrentGridPosition, meleeRange))
        {
            if (UseAP(hammerApCost))
            {
                HammerAfterChecks();
                return true;
            }
        }
        else
        {
            mapManager.OccupiedFields.Remove(CurrentGridPosition);
            APath path = mapManager.ChooseBestAPath(CurrentGridPosition, mapManager.FindNeighboursOfRange(player.currentGridPosition, meleeRange));
            if (path == null)
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
                return false;
            }
            int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
            if (apcost == 0)
                apcost = 1;
            apcost += hammerApCost;
            if (UseAP(apcost))
            {
                State = EnemyState.Moving;
                mapManager.MoveEnemy(this, path);
                StartCoroutine(WaitForMoveAndHammer());
                return true;
            }
            else
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
            }
        }
        return false;
    }

    private void HammerAfterChecks()
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Hammer", true);
        int meleeDir = StaticHelpers.ChooseDir(transform.position, player.transform.position);
        animator.SetInteger("HammerDirection", meleeDir);
        CurrentDirection = meleeDir;
        player.beforeHitTime = 0.2f;
        player.DamageHealth(StaticHelpers.RollDamage(hammerMinDice, hammerMaxDice, hammerModifier)); // count damage
        StartCoroutine(WaitAndGoBackFromHammer());
    }

    private IEnumerator WaitForMoveAndHammer()
    {
        yield return new WaitUntil(() => State == EnemyState.Neutral);
        StartCoroutine(WaitJustABitAndHammer());
    }

    private IEnumerator WaitJustABitAndHammer()
    {
        yield return new WaitForSeconds(0.05f);
        HammerAfterChecks();
    }

    public IEnumerator WaitAndGoBackFromHammer()
    {
        yield return new WaitForSeconds(hammerTime);
        animator.SetBool("Hammer", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("HammerDirection", 0);
        animator.SetInteger("IdleDirection", CurrentDirection);
        moveFinished = true;
    }

    public bool Puke()
    {
        int meleeRange = 1;
        if (StaticHelpers.CheckIfTargetIsInRange(player.currentGridPosition, CurrentGridPosition, meleeRange))
        {
            if (UseAP(pukeApCost))
            {
                PukeAfterChecks();
                return true;
            }
        }
        else
        {
            mapManager.OccupiedFields.Remove(CurrentGridPosition);
            APath path = mapManager.ChooseBestAPath(CurrentGridPosition, mapManager.FindNeighboursOfRange(player.currentGridPosition, meleeRange));
            if (path == null)
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
                return false;
            }
            int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
            if (apcost == 0)
                apcost = 1;
            apcost += pukeApCost;
            if (UseAP(apcost))
            {
                State = EnemyState.Moving;
                mapManager.MoveEnemy(this, path);
                StartCoroutine(WaitForMoveAndPuke());
                return true;
            }
            else
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
            }
        }
        return false;
    }

    private void PukeAfterChecks()
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Puke", true);
        int meleeDir = StaticHelpers.ChooseDir(transform.position, player.transform.position);
        animator.SetInteger("PukeDirection", meleeDir);
        CurrentDirection = meleeDir;
        player.beforeHitTime = 0.4f;
        player.DamageHealth(StaticHelpers.RollDamage(pukeMinDice, pukeMaxDice, pukeModifier));
        player.IncreaseToxicity(StaticHelpers.RollDamage(pukeToxicMinDice, pukeToxicMaxDice, pukeToxicModifier));
        StartCoroutine(WaitAndGoBackFromPuke());
    }

    private IEnumerator WaitForMoveAndPuke()
    {
        yield return new WaitUntil(() => State == EnemyState.Neutral);
        StartCoroutine(WaitJustABitAndPuke());
    }
    private IEnumerator WaitJustABitAndPuke()
    {
        yield return new WaitForSeconds(0.05f);
        PukeAfterChecks();
    }
    public IEnumerator WaitAndGoBackFromPuke()
    {
        yield return new WaitForSeconds(pukeTime);
        animator.SetBool("Puke", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("PukeDirection", 0);
        animator.SetInteger("IdleDirection", CurrentDirection);
        moveFinished = true;
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
        mapManager.OccupiedFields.Remove(CurrentGridPosition);

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

    public void OnMouseEnter()
    {
        if (player.inFight)
            ActivateSlider();
    }

    public void OnMouseExit()
    {
        DeactivateSlider();
    }

    public override IEnumerator MoveToPosition(Vector3 position, float timeTo)
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", true);
        Vector3 currentPos = transform.position;
        float ti = 0f;
        while (true)
        {
            ti += Time.deltaTime / timeTo;
            transform.position = Vector3.Lerp(currentPos, position, ti);

            if (transform.position == position)
            {
                break;
            }
            yield return null;
        }
    }

    public override void PlayWalk(Vector3Int walkDir)
    {

        if (walkDir.x == 0 && walkDir.y == -1)
        {
            animator.SetInteger("WalkDirection", 1);
        }
        else if (walkDir.x == 1 && walkDir.y == -1)
        {
            animator.SetInteger("WalkDirection", 2);
        }
        else if (walkDir.x == 1 && walkDir.y == 0)
        {
            animator.SetInteger("WalkDirection", 3);
        }
        else if (walkDir.x == 1 && walkDir.y == 1)
        {
            animator.SetInteger("WalkDirection", 4);
        }
        else if (walkDir.x == 0 && walkDir.y == 1)
        {
            animator.SetInteger("WalkDirection", 5);
        }
        else if (walkDir.x == -1 && walkDir.y == 1)
        {
            animator.SetInteger("WalkDirection", 6);
        }
        else if (walkDir.x == -1 && walkDir.y == 0)
        {
            animator.SetInteger("WalkDirection", 7);
        }
        else if (walkDir.x == -1 && walkDir.y == -1)
        {
            animator.SetInteger("WalkDirection", 8);
        }
    }

    public override IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nextField)
    {
        Vector3 realNF = new(nextField.x, nextField.y + 0.25f, enemyZ);
        yield return new WaitUntil(() => transform.position == realNF);
        animator.SetInteger("WalkDirection", 0);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", true);
        if (idleDir.x == 0 && idleDir.y == -1)
        {
            IdleDirection(1);
        }
        else if (idleDir.x == 1 && idleDir.y == -1)
        {
            IdleDirection(2);
        }
        else if (idleDir.x == 1 && idleDir.y == 0)
        {
            IdleDirection(3);
        }
        else if (idleDir.x == 1 && idleDir.y == 1)
        {
            IdleDirection(4);
        }
        else if (idleDir.x == 0 && idleDir.y == 1)
        {
            IdleDirection(5);
        }
        else if (idleDir.x == -1 && idleDir.y == 1)
        {
            IdleDirection(6);
        }
        else if (idleDir.x == -1 && idleDir.y == 0)
        {
            IdleDirection(7);
        }
        else if (idleDir.x == -1 && idleDir.y == -1)
        {
            IdleDirection(8);
        }

        State = EnemyState.Neutral;

    }
}
