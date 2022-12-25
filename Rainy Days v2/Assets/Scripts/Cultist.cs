using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CultistType
{
    Normal, Leader
}

public class Cultist : Enemy
{
    public CultistType type;

    public GameObject gunLight;

    public int cultistMaxHealth, cultistMaxAP, cultistInitiative;
    public float cultistMovement;
    private Player player;
    private MapManager mapManager;
    private Animator animator;
    public float hitTime = 0.6f;
    private float beforeHitTime; // if theres more attacks gotta fix this
                                 //private float beforeDeathTime;

    // public SpriteRenderer sprite;

    public float gunTime = 1.2f;
    public int gunApCost = 1;
    private bool myTurn = false;
    private bool moveFinished = true;

    public int gunMinDice = 2;
    public int gunMaxDice = 16;
    public int gunModifier = 5;
    public int gunRange = 4;

    public float beforeGunLight, afterGunLight;
    public float myTurnTime { get; set; } = 3f;


    public override void MakeTurn()
    {
        StartCoroutine(CultistTurn());
    }

    public IEnumerator CultistTurn()
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
            ChooseMove();
            StartCoroutine(WaitAndMakeMove());
        }
    }

    private void ChooseMove()
    {
        if (type == CultistType.Normal)
            NormalCultistMove();
        else
            LeaderCultistMove();
    }

    private void LeaderCultistMove()
    {
        //
    }

    private void NormalCultistMove()
    {
        if (!Gun())
        {
            UseResttoMoveUp();
        }
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
        Stats = new EnemyStats(this, cultistMaxHealth, cultistMaxAP, cultistInitiative, cultistMovement);
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
        mapManager.OccupiedFields.Remove(CurrentGridPosition);
        if (mapManager.FindNeighboursOfRange(player.currentGridPosition, gunRange).Contains(CurrentGridPosition))
        {
            myTurn = false;
            moveFinished = true;
            myTurnTime = 0.5f;
            mapManager.OccupiedFields.Add(CurrentGridPosition);
            return true;
        }
        APath path = mapManager.ChooseBestAPath(CurrentGridPosition, mapManager.FindNeighboursOfRange(player.currentGridPosition, gunRange));
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
        IdleDirection(StaticHelpers.ChooseDir(transform.position, player.transform.position));
        myTurn = false;
        moveFinished = true;
    }

    public bool Gun()
    {
        if (StaticHelpers.CheckIfTargetIsInRange(player.currentGridPosition, CurrentGridPosition, gunRange))
        {
            if (UseAP(gunApCost))
            {
                GunAfterChecks();
                myTurnTime = gunTime;
                return true;
            }
        }
        else
        {
            mapManager.OccupiedFields.Remove(CurrentGridPosition);
            APath path = mapManager.ChooseBestAPath(CurrentGridPosition, mapManager.FindNeighboursOfRange(player.currentGridPosition, gunRange));
            if (path == null)
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
                return false;
            }
            int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
            if (apcost == 0)
                apcost = 1;
            apcost += gunApCost;
            if (UseAP(apcost))
            {
                myTurnTime = path.DijkstraScore * timeForCornerMove + gunTime;
                State = EnemyState.Moving;
                mapManager.MoveEnemy(this, path);
                StartCoroutine(WaitForMoveAndGun());
                return true;
            }
            else
            {
                mapManager.OccupiedFields.Add(CurrentGridPosition);
            }
        }
        return false;
    }

    private void GunAfterChecks()
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Gun", true);
        int gunDir = StaticHelpers.ChooseDir(transform.position, player.transform.position);
        animator.SetInteger("GunDirection", gunDir);
        CurrentDirection = gunDir;
        SetGunLight();
        player.beforeHitTime = 0.1f;
        player.DamageHealth(StaticHelpers.RollDamage(gunMinDice, gunMaxDice, gunModifier));
        StartCoroutine(WaitForGunLight());
        StartCoroutine(WaitAndGoBackFromGun());
    }

    public void SetGunLight()
    {
        if (CurrentDirection == 1)
            gunLight.transform.position = transform.position + new Vector3(0.4f, -0.2f, 0);
        else if (CurrentDirection == 2)
            gunLight.transform.position = transform.position + new Vector3(0.6f, 0, 0);
        else if (CurrentDirection == 3)
            gunLight.transform.position = transform.position + new Vector3(0.4f, 0.2f, 0);
        else if (CurrentDirection == 4)
            gunLight.transform.position = transform.position + new Vector3(0, 0.4f, 0);
        else if (CurrentDirection == 5)
            gunLight.transform.position = transform.position + new Vector3(-0.4f, 0.2f, 0);
        else if (CurrentDirection == 6)
            gunLight.transform.position = transform.position + new Vector3(-0.6f, 0, 0);
        else if (CurrentDirection == 7)
            gunLight.transform.position = transform.position + new Vector3(-0.4f, -0.2f, 0);
        else
            gunLight.transform.position = transform.position + new Vector3(0, -0.4f, 0);
    }

    private IEnumerator WaitForMoveAndGun()
    {
        yield return new WaitUntil(() => State == EnemyState.Neutral);
        StartCoroutine(WaitJustABitAndGun());
    }

    private IEnumerator WaitJustABitAndGun()
    {
        yield return new WaitForSeconds(0.05f);
        GunAfterChecks();
    }

    public IEnumerator WaitAndGoBackFromGun()
    {
        yield return new WaitForSeconds(gunTime);
        animator.SetBool("Gun", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("GunDirection", 0);
        animator.SetInteger("IdleDirection", base.CurrentDirection);
        moveFinished = true;
    }

    private IEnumerator WaitForGunLight()
    {
        yield return new WaitForSeconds(beforeGunLight);
        gunLight.SetActive(true);
        StartCoroutine(WaitForGunLightShut());
    }

    private IEnumerator WaitForGunLightShut()
    {
        yield return new WaitForSeconds(afterGunLight);
        gunLight.SetActive(false);
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
        animator.SetInteger("DeathDirection", base.CurrentDirection);


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
        animator.SetInteger("HitDirection", base.CurrentDirection);
        StartCoroutine(AfterHit());
    }

    private IEnumerator AfterHit()
    {
        yield return new WaitForSeconds(hitTime);
        animator.SetBool("Hit", false);
        animator.SetInteger("HitDirection", 0);
        animator.SetBool("Idle", true);
        animator.SetInteger("IdleDirection", base.CurrentDirection);
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
