using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boss : Enemy
{
    public int bossMaxHealth, bossMaxAP, bossInitiative;
    public float bossMovement;
    private Player player;
    private MapManager mapManager;
    private Animator animator;
    public float hitTime = 0.6f;
    private float beforeHitTime;

    public float lightningTime = 1.2f;
    public int lightningApCost = 1;
    private bool myTurn = false;
    private bool moveFinished = true;

    public int lightningMinDice = 2;
    public int lightningMaxDice = 16;
    public int lightningModifier = 5;
    public int lightningRange = 4;
    public float myTurnTime { get; set; } = 3f;

    public float dissappearTime = 0.5f;
    public float teleportTime = 1f;
    public float beforeBoltSpawnTime = 0.67f;
    public float boltFlyTimeTo = 0.2f;

    public DialogueTrigger endGameDialogue;

    public AudioSource hitSound, deathSound, teleportSound, shockSound;

    public GameObject lightningBolt;
    public GameObject boltSpawnE, boltSpawnNE, boltSpawnN, boltSpawnNW, boltSpawnW, boltSpawnSW, boltSpawnS, boltSpawnSE;

    public override void MakeTurn()
    {
        StartCoroutine(BossTurn());
    }

    public IEnumerator BossTurn()
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
        BossMove();
    }

    private void BossMove()
    {
        if (!MoveUp())
            Lightning();
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
        Stats = new EnemyStats(this, bossMaxHealth, bossMaxAP, bossInitiative, bossMovement);
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


    public bool MoveUp()
    {
        mapManager.OccupiedFields.Remove(CurrentGridPosition);
        if (mapManager.FindNeighboursOfRange(player.currentGridPosition, lightningRange).Contains(CurrentGridPosition))
        {
            //myTurnTime = 0f;
            mapManager.OccupiedFields.Add(CurrentGridPosition);
            return false;
        }
        APath path = mapManager.ChooseBestAPath(CurrentGridPosition, mapManager.FindNeighboursOfRange(player.currentGridPosition, lightningRange));
        if (path == null)
        {
            mapManager.OccupiedFields.Add(CurrentGridPosition);
            StartCoroutine(WaitAndEndTurn());
            return false;
        }
        int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
        if (apcost == 0)
            apcost = 1;
        UseAP(apcost);
        myTurnTime = teleportTime;
        State = EnemyState.Moving;
        Teleport(path);
        //StartCoroutine(WaitAndEndPort());
        return true;
    }

    private void Teleport(APath path)
    {
        teleportSound.Play();
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Port", true);
        Vector3 realPos = mapManager.map.CellToWorld(path.Fields[path.Fields.Count - 1].GridPosition);
        Vector3 realRealPos = new(realPos.x, realPos.y + 0.25f, enemyZ);
        CurrentDirection = StaticHelpers.ChooseDir(transform.position, realRealPos);
        animator.SetInteger("PortDirection", CurrentDirection);
        StartCoroutine(TeleportChangePos(path));
    }

    private IEnumerator TeleportChangePos(APath path)
    {
        yield return new WaitForSeconds(dissappearTime);
        CurrentGridPosition = path.Fields[path.Fields.Count - 1].GridPosition;
        mapManager.OccupiedFields.Add(CurrentGridPosition);
        Vector3 realPos = mapManager.map.CellToWorld(CurrentGridPosition);
        Vector3 realRealPos = new(realPos.x, realPos.y + 0.25f, enemyZ);
        transform.position = realRealPos;
        StartCoroutine(EndPort());
    }

    private IEnumerator EndPort()
    {
        yield return new WaitForSeconds(teleportTime - dissappearTime);
        animator.SetInteger("PortDirection", 0);
        animator.SetBool("Idle", true);
        animator.SetBool("Port", false);
        CurrentDirection = StaticHelpers.ChooseDir(transform.position, player.transform.position);
        animator.SetInteger("IdleDirection", CurrentDirection);
        State = EnemyState.Neutral;
        moveFinished = true;
    }

    private IEnumerator WaitAndEndTurn()
    {
        yield return new WaitUntil(() => State == EnemyState.Neutral);
        myTurn = false;
        moveFinished = true;
    }


    public bool Lightning()
    {
        if (UseAP(lightningApCost))
        {
            LightningAfterChecks();
            myTurnTime = lightningTime;
            return true;
        }
        else
        {
            StartCoroutine(WaitAndEndTurn());
            return false;
        }
    }

    private void LightningAfterChecks()
    {
        shockSound.Play();
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Shock", true);
        int shockDir = StaticHelpers.ChooseDir(transform.position, player.transform.position);
        animator.SetInteger("ShockDirection", shockDir);
        CurrentDirection = shockDir;
        StartCoroutine(SpawnBolt());
        StartCoroutine(WaitAndGoBackFromLightning());
    }

    private IEnumerator SpawnBolt()
    {
        yield return new WaitForSeconds(beforeBoltSpawnTime);
        switch (CurrentDirection)
        {
            case 1:
                GameObject boltE = Instantiate(lightningBolt, boltSpawnE.transform);
                boltE.GetComponent<Animator>().SetInteger("Direction", 1);
                StartCoroutine(SendBolt(boltE));
                break;
            case 2:
                GameObject boltNE = Instantiate(lightningBolt, boltSpawnNE.transform);
                boltNE.GetComponent<Animator>().SetInteger("Direction", 2);
                StartCoroutine(SendBolt(boltNE));
                break;
            case 3:
                GameObject boltN = Instantiate(lightningBolt, boltSpawnN.transform);
                boltN.GetComponent<Animator>().SetInteger("Direction", 3);
                StartCoroutine(SendBolt(boltN));
                break;
            case 4:
                GameObject boltNW = Instantiate(lightningBolt, boltSpawnNW.transform);
                boltNW.GetComponent<Animator>().SetInteger("Direction", 4);
                StartCoroutine(SendBolt(boltNW));
                break;
            case 5:
                GameObject boltW = Instantiate(lightningBolt, boltSpawnW.transform);
                boltW.GetComponent<Animator>().SetInteger("Direction", 5);
                StartCoroutine(SendBolt(boltW));
                break;
            case 6:
                GameObject boltSW = Instantiate(lightningBolt, boltSpawnSW.transform);
                boltSW.GetComponent<Animator>().SetInteger("Direction", 6);
                StartCoroutine(SendBolt(boltSW));
                break;
            case 7:
                GameObject boltS = Instantiate(lightningBolt, boltSpawnS.transform);
                boltS.GetComponent<Animator>().SetInteger("Direction", 7);
                StartCoroutine(SendBolt(boltS));
                break;
            case 8:
                GameObject boltSE = Instantiate(lightningBolt, boltSpawnSE.transform);
                boltSE.GetComponent<Animator>().SetInteger("Direction", 8);
                StartCoroutine(SendBolt(boltSE));
                break;
        }

    }

    private IEnumerator SendBolt(GameObject bolt)
    {
        float timeTo = boltFlyTimeTo;
        Vector3 currentPos = bolt.transform.position;
        Vector3 playerPos = new(player.transform.position.x, player.transform.position.y + 0.25f, 10f);
        float ti = 0f;
        player.beforeHitTime = boltFlyTimeTo;
        player.DamageHealth(StaticHelpers.RollDamage(lightningMinDice, lightningMaxDice, lightningModifier));
        while (true)
        {
            ti += Time.deltaTime / timeTo;
            bolt.transform.position = Vector3.Lerp(currentPos, playerPos, ti);

            if (bolt.transform.position == playerPos)
            {
                bolt.GetComponent<Animator>().SetBool("Boom", true);
                StartCoroutine(WaitAndDestroyBolt(bolt));
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitAndDestroyBolt(GameObject bolt)
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(bolt);
    }

    public IEnumerator WaitAndGoBackFromLightning()
    {
        yield return new WaitForSeconds(lightningTime);
        animator.SetBool("Shock", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("ShockDirection", 0);
        animator.SetInteger("IdleDirection", base.CurrentDirection);
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

        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
        endGameDialogue.TriggerDialogue();

        deathSound.Play();
        FindObjectOfType<AudioManager>().BloodHit();
        animator.SetBool("Idle", false);
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Death", true);
        animator.SetInteger("DeathDirection", base.CurrentDirection);


        GetComponent<BoxCollider2D>().enabled = false;
        DeactivateSlider();
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
        hitSound.Play();
        FindObjectOfType<AudioManager>().BloodHit();
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
        yield return null;
    }

    public override void PlayWalk(Vector3Int walkDir)
    {

    }

    public override IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nextField)
    {
        yield return null;
    }

    public override void PlayWalkSound()
    {

    }

    public override void StopWalkSound()
    {

    }

    public void FinishGame()
    {
        FindObjectOfType<PauseMenuFunctions>().FinishGame();
    }
}
