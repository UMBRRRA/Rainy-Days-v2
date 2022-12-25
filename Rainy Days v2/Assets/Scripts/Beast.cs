using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Beast : Enemy
{

    public int beastMaxHealth, beastMaxAP, beastInitiative;
    public float beastMovement;
    private Player player;
    private MapManager mapManager;
    private Animator animator;
    public float hitTime = 0.6f;
    private float beforeHitTime;

    public float meleeTime = 1.2f;
    public int meleeApCost = 1;
    private bool myTurn = false;
    private bool moveFinished = true;

    public int meleeMinDice = 2;
    public int meleeMaxDice = 16;
    public int meleeModifier = 5;

    public int meleePoisonMinDice = 1;
    public int meleePoisonMaxDice = 4;
    public int meleePoisonModifier = 0;

    public float myTurnTime { get; set; } = 3f;

    public override void MakeTurn()
    {
        StartCoroutine(BeastTurn());
    }

    public IEnumerator BeastTurn()
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
        if (myTurn && player.State != PlayerState.Dead)
        {
            moveFinished = false;
            BeastMove();
            StartCoroutine(WaitAndMakeMove());
        }
    }

    private void BeastMove()
    {
        if (!Melee())
            UseResttoMoveUp();
    }

    private IEnumerator WaitAndMakeMove()
    {
        yield return new WaitForSeconds(myTurnTime + 0.4f);
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
        else if (player.State == PlayerState.Snipe)
        {
            SnipeMe();
        }
        else if (player.State == PlayerState.Flurry)
        {
            FlurryMe();
        }
    }

    public override void ShootMe()
    {
        beforeHitTime = 0.2f;
        player.Shoot(this, false);
    }

    public override void MeleeMe()
    {
        beforeHitTime = 0.3f;
        player.Melee(this);
    }

    public override void SnipeMe()
    {
        beforeHitTime = 0.2f;
        player.Shoot(this, true);
    }

    public override void FlurryMe()
    {
        beforeHitTime = 0.3f;
        player.Flurry(this);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        IdleDirection(spawnDirection);
        Stats = new EnemyStats(this, beastMaxHealth, beastMaxAP, beastInitiative, beastMovement);
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
            myTurnTime = 0.5f;
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
        myTurnTime = path.DijkstraScore * timeForCornerMove;
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

    public bool Melee()
    {
        int meleeRange = 1;
        if (StaticHelpers.CheckIfTargetIsInRange(player.currentGridPosition, CurrentGridPosition, meleeRange))
        {
            if (UseAP(meleeApCost))
            {
                MeleeAfterChecks();
                myTurnTime = meleeTime;
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
            apcost += meleeApCost;
            if (UseAP(apcost))
            {
                myTurnTime = path.DijkstraScore * timeForCornerMove + meleeTime;
                State = EnemyState.Moving;
                mapManager.MoveEnemy(this, path);
                StartCoroutine(WaitForMoveAndMelee());
                return true;
            }
            else
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
            }
        }
        return false;
    }

    private void MeleeAfterChecks()
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Melee", true);
        int meleeDir = StaticHelpers.ChooseDir(transform.position, player.transform.position);
        animator.SetInteger("MeleeDirection", meleeDir);
        CurrentDirection = meleeDir;
        player.beforeHitTime = 0.2f;
        player.DamageHealth(StaticHelpers.RollDamage(meleeMinDice, meleeMaxDice, meleeModifier)); // count damage
        player.IncreaseToxicity(StaticHelpers.RollDamage(meleePoisonMinDice, meleePoisonMaxDice, meleePoisonModifier));
        StartCoroutine(WaitAndGoBackFromMelee());
    }

    private IEnumerator WaitForMoveAndMelee()
    {
        yield return new WaitUntil(() => State == EnemyState.Neutral);
        StartCoroutine(WaitJustABitAndMelee());
    }

    private IEnumerator WaitJustABitAndMelee()
    {
        yield return new WaitForSeconds(0.05f);
        MeleeAfterChecks();
    }

    public IEnumerator WaitAndGoBackFromMelee()
    {
        yield return new WaitForSeconds(meleeTime);
        animator.SetBool("Melee", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("MeleeDirection", 0);
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
        FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Health, $"-{damage} Health", this.transform.position);
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
        StartCoroutine(ChangeZAfterDeath());
    }

    private IEnumerator ChangeZAfterDeath()
    {
        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.1f);

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

    public override void PlayWalkSound()
    {

    }

    public override void StopWalkSound()
    {

    }
}
