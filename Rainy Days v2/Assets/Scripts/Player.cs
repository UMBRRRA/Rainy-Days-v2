using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum PlayerState
{
    Neutral,
    Moving,
    Pause,
    Dialogue,
    Action,
    NotMyTurn,
    Shooting,
    Meleeing,
    Event,
    Dead,
    Snipe,
    Flurry
}

public class Player : MonoBehaviour
{
    public int startHealth;
    public int startToxicity;
    public int startAP;
    public int startMag;
    public int startInitative;
    public float startMovement;
    public int unusedFeats = 0;
    private int _level = 1;
    public int level
    {
        get
        {
            return _level;
        }
        set
        {
            if (value != _level && value != 1)
            {
                FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Level, "Level Up!", this.transform.position);
                unusedFeats += (value - _level);
            }
            _level = value;
        }
    }
    private int _exp;
    public int exp
    {
        get
        {
            return _exp;
        }
        set
        {
            _exp = value;
            if (value >= 0 && value < secondLevel)
                level = 1;
            else if (value >= secondLevel && value < thirdLevel)
                level = 2;
            else if (value >= thirdLevel && value < fourthLevel)
                level = 3;
            else if (value >= fourthLevel && value < fifthLevel)
                level = 4;
            else if (value >= fifthLevel && value < sixthLevel)
                level = 5;
            else if (value >= sixthLevel && value < seventhLevel)
                level = 6;
            else if (value >= seventhLevel && value < eightLevel)
                level = 7;
            else if (value >= eightLevel && value < ninthLevel)
                level = 8;
            else if (value >= ninthLevel && value < tenthLevel)
                level = 9;
            else if (value >= tenthLevel && value < eleventhLevel)
                level = 10;
            else if (value >= eleventhLevel && value < twelfthLevel)
                level = 11;
            else
                level = 12;
        }
    }

    public PlayerStats Stats { get; set; }

    private static Player singleton;

    public float playerZ = 1.2f;
    public float timeForMove = 0.3f;
    public float timeForCornerMove = 0.5f;
    public Vector3Int spawnGridPosition;
    public Vector3Int currentGridPosition;
    public MapManager mapManager;
    public int spawnDirection;
    private int _currentDirection;
    public int currentDirection
    {
        get
        {
            return _currentDirection;
        }
        set
        {
            _currentDirection = value;
            if (currentDirection == 1)
                gunLight.transform.position = transform.position + new Vector3(0.4f, -0.2f, 0);
            else if (currentDirection == 2)
                gunLight.transform.position = transform.position + new Vector3(0.6f, 0, 0);
            else if (currentDirection == 3)
                gunLight.transform.position = transform.position + new Vector3(0.4f, 0.2f, 0);
            else if (currentDirection == 4)
                gunLight.transform.position = transform.position + new Vector3(0, 0.4f, 0);
            else if (currentDirection == 5)
                gunLight.transform.position = transform.position + new Vector3(-0.4f, 0.2f, 0);
            else if (currentDirection == 6)
                gunLight.transform.position = transform.position + new Vector3(-0.6f, 0, 0);
            else if (currentDirection == 7)
                gunLight.transform.position = transform.position + new Vector3(-0.4f, -0.2f, 0);
            else
                gunLight.transform.position = transform.position + new Vector3(0, -0.4f, 0);
        }
    }
    private Animator animator;
    private PauseMenuFunctions pauseMenu;
    private CharacterMenuFunctions characterMenu;
    private HudFunctions hud;
    public float potionStrength = 0.33f;
    public float potionTime = 1f;
    public float reloadTime = 1f;
    public float shootTime = 1.2f;
    public float meleeTime = 1.5f;
    public int potionApCost = 1;
    public int reloadApCost = 1;
    public int gunApCost = 1;
    public int meleeApCost = 1;
    public int gunRange = 5;

    public ItemObject potion, antidote, ammo;
    public InventoryObject inventory;

    public bool inFight = false;
    public GameObject gunLight;
    public float beforeGunLight, afterGunLight;

    private PlayerState _state = PlayerState.Event;
    public float hitTime = 1f;
    public float beforeHitTime = 0.3f;
    public float deathTime = 1.3f;

    public int gunMinDice = 2;
    public int gunMaxDice = 16;
    public int gunModifier = 1;

    public int meleeMinDice = 1;
    public int meleeMaxDice = 8;
    public int meleeModifier = 2;

    public Texture2D mainCursor;

    public int secondLevel, thirdLevel, fourthLevel, fifthLevel, sixthLevel, seventhLevel, eightLevel,
        ninthLevel, tenthLevel, eleventhLevel, twelfthLevel;

    private float MovementMemory { get; set; } = 1f;
    private bool Wailed { get; set; } = false;

    public int snipeCooldown = 3;
    public int hasteCooldown = 5;
    public int flurryCooldown = 4;
    public float hasteApRefundPercentage = 0.5f;
    public float flurryApCostModifier = 1.5f;

    public int snipeCurrentCooldown = 0;
    public int snipeModifier = 2;
    public float hasteTime = 0.68f;
    public int hasteCurrentCooldown = 0;
    public int flurryCurrentCooldown = 0;
    public int flurryHits = 3;
    private int flurryCounter = 0;

    public PlayerState State
    {
        get
        {
            return _state;
        }
        set
        {
            Debug.Log($"Changing player state from {_state} to {value}");
            _state = value;
        }
    }




    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }



    void Start()
    {
        Stats = new PlayerStats(this, startHealth, startAP, startInitative, startMovement, startToxicity, startMag);
        currentGridPosition = spawnGridPosition;
        mapManager = FindObjectOfType<MapManager>();
        animator = GetComponent<Animator>();
        animator.SetInteger("IdleDirection", spawnDirection);
        currentDirection = spawnDirection;
        StartCoroutine(FindHud());
    }

    public void ResetCooldowns()
    {
        snipeCurrentCooldown = 0;
        hasteCurrentCooldown = 0;
        flurryCurrentCooldown = 0;
    }

    public void ReduceCooldowns()
    {
        if (snipeCurrentCooldown > 0)
        {
            snipeCurrentCooldown -= 1;
            hud.SetSnipeCooldown();
        }
        if (hasteCurrentCooldown > 0)
        {
            hasteCurrentCooldown -= 1;
            hud.SetHasteCooldown();
        }
        if (flurryCurrentCooldown > 0)
        {
            flurryCurrentCooldown -= 1;
            hud.SetFlurryCooldown();
        }
    }

    public void StartSnipeCooldown()
    {
        snipeCurrentCooldown = snipeCooldown;
        hud.SetSnipeCooldown();
    }

    public void StartHasteCooldown()
    {
        hasteCurrentCooldown = hasteCooldown;
        hud.SetHasteCooldown();
    }

    public void StartFlurryCooldown()
    {
        flurryCurrentCooldown = flurryCooldown;
        hud.SetFlurryCooldown();
    }

    public void BecomeWailed()
    {
        MovementMemory = Stats.Movement;
        Stats.Movement = Stats.Movement / 2;
        Wailed = true;
        FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.General, $"Movement decreased", transform.position);
    }

    public void Unwail()
    {
        if (Wailed == true)
        {
            Stats.Movement = MovementMemory;
            Wailed = false;
            FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.General, $"Movement restored", transform.position);
        }
    }


    public IEnumerator FindHud()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.SetMaxHealth(Stats.MaxHealth);
        hud.SetHealth(Stats.CurrentHealth);
        hud.SetMaxToxicity(Stats.MaxToxicity);
        hud.SetToxicity(Stats.CurrentToxicity);
        hud.SetMaxAp(Stats.MaxAP);
        hud.SetAp(Stats.CurrentAP);
    }

    public void ChangeRoom()
    {
        currentGridPosition = spawnGridPosition;
        Vector3 pos = mapManager.map.CellToWorld(spawnGridPosition);
        transform.position = new Vector3(pos.x, pos.y + 0.25f, playerZ);
        IdleDirectionNoNeutral(spawnDirection);
        State = PlayerState.Event;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && State == PlayerState.Neutral)
        {
            State = PlayerState.Pause;
            StartCoroutine(OpenPauseMenu());
        }

        if (Input.GetKeyDown(KeyCode.C) && State == PlayerState.Neutral)
        {
            State = PlayerState.Pause;
            StartCoroutine(OpenCharacterMenu());
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (State == PlayerState.Shooting || State == PlayerState.Meleeing || State == PlayerState.Snipe || State == PlayerState.Flurry)
            {
                State = PlayerState.Neutral;
                Vector2 hotspot = new Vector2(mainCursor.width / 2, 0);
                hud.DeactivateUsings();
                Cursor.SetCursor(mainCursor, hotspot, CursorMode.ForceSoftware);
            }
        }
    }

    private IEnumerator OpenPauseMenu()
    {
        yield return new WaitUntil(() => (pauseMenu = FindObjectOfType<PauseMenuFunctions>()) != null);
        pauseMenu.ActivatePauseMenu();
    }

    private IEnumerator OpenCharacterMenu()
    {
        yield return new WaitUntil(() => (characterMenu = FindObjectOfType<CharacterMenuFunctions>()) != null);
        characterMenu.ActivateCharacterMenu();
    }

    public IEnumerator MoveToPosition(Vector3 position, float timeTo)
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);
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

    public IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nf, int toxicity)
    {
        Vector3 realNF = new(nf.x, nf.y + 0.25f, playerZ);
        yield return new WaitUntil(() => transform.position == realNF);
        animator.SetInteger("WalkDirection", 0);
        animator.SetBool("Walking", false);
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
        IncreaseToxicity(toxicity);
    }

    public void PlayWalk(Vector3Int walkDir)
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

    private void IdleDirection(int dir)
    {
        animator.SetInteger("IdleDirection", dir);
        currentDirection = dir;
        State = PlayerState.Neutral;
    }

    private void IdleDirectionNoNeutral(int dir)
    {
        animator.SetInteger("IdleDirection", dir);
        currentDirection = dir;
    }

    public void RestoreHealth(int amount)
    {
        if (Stats.CurrentHealth + amount >= Stats.MaxHealth)
        {
            Stats.CurrentHealth = Stats.MaxHealth;
        }
        else
        {
            Stats.CurrentHealth += amount;
        }
        FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Health, $"+{amount} Health", this.transform.position);
        hud.SetHealth(Stats.CurrentHealth);
    }

    public void DamageHealth(int amount)
    {
        if (Stats.CurrentHealth - amount <= 0)
        {
            Stats.CurrentHealth = 0;
            //StopAllCoroutines();
            StartCoroutine(BeforeDeath());
        }
        else
        {
            Stats.CurrentHealth -= amount;
            StartCoroutine(BeforeHit());
        }
        FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Health, $"-{amount} Health", this.transform.position);
        hud.SetHealth(Stats.CurrentHealth);
    }


    private IEnumerator BeforeDeath()
    {
        State = PlayerState.Dead;
        yield return new WaitForSeconds(0.8f);
        animator.SetBool("Idle", false);
        animator.SetBool("Death", true);
        animator.SetInteger("IdleDirection", 0);
        animator.SetInteger("DeathDirection", currentDirection);
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
        StartCoroutine(PlayDead());

    }

    private IEnumerator PlayDead()
    {
        yield return new WaitForSeconds(deathTime);
        FindObjectOfType<PauseMenuFunctions>().ReturnToMainMenu();
        FindObjectOfType<HudFunctions>().DeactivateHud();
    }

    private IEnumerator BeforeHit()
    {
        yield return new WaitForSeconds(beforeHitTime);
        animator.SetBool("Idle", false);
        animator.SetBool("Hit", true);
        animator.SetInteger("IdleDirection", 0);
        animator.SetInteger("HitDirection", currentDirection);
        StartCoroutine(PlayHit());
    }

    private IEnumerator PlayHit()
    {
        yield return new WaitForSeconds(hitTime);
        animator.SetBool("Idle", true);
        animator.SetBool("Hit", false);
        animator.SetInteger("IdleDirection", currentDirection);
        animator.SetInteger("HitDirection", 0);
    }

    public void DrinkPotion()
    {
        if (State == PlayerState.Neutral && inventory.HasItem(potion, 1)) // also fight and action points...
        {
            if (inFight)
            {
                if (UseAP(potionApCost))
                    DrinkPotionAfterCheck();
            }
            else
            {
                DrinkPotionAfterCheck();
            }
        }
    }

    private void DrinkPotionAfterCheck()
    {
        State = PlayerState.Action;
        inventory.TakeItem(potion, 1);
        hud.UpdateAmounts();
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Potion", true);
        animator.SetInteger("PotionDirection", currentDirection);

        int amount = (int)Math.Round(Stats.MaxHealth * potionStrength);
        RestoreHealth(amount);
        StartCoroutine(WaitAndGoBackFromDrinkingPotion());
    }

    public IEnumerator WaitAndGoBackFromDrinkingPotion()
    {
        yield return new WaitForSeconds(potionTime);
        animator.SetBool("Potion", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("PotionDirection", 0);
        animator.SetInteger("IdleDirection", currentDirection);
        State = PlayerState.Neutral;
    }

    public void HealToxicity(int amount)
    {
        if (Stats.CurrentToxicity - amount <= 0)
        {
            Stats.CurrentToxicity = 0;
        }
        else
        {
            Stats.CurrentToxicity -= amount;
        }
        FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Toxicity, $"-{amount} Toxicity", this.transform.position);
        hud.SetToxicity(Stats.CurrentToxicity);
    }

    public void IncreaseToxicity(int amount)
    {
        if (Stats.CurrentToxicity + amount >= Stats.MaxToxicity)
        {
            Stats.CurrentToxicity = Stats.MaxToxicity;
            StopAllCoroutines();
            StartCoroutine(BeforeDeath());
        }
        else
        {
            Stats.CurrentToxicity += amount;
        }
        if (amount != 0)
            FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Toxicity, $"+{amount} Toxicity", this.transform.position);
        hud.SetToxicity(Stats.CurrentToxicity);
    }

    public void Haste()
    {
        if (State == PlayerState.Neutral && inFight && hasteCurrentCooldown == 0) // also fight and action points...
        {
            HasteAfterCheck();
        }
    }

    private void HasteAfterCheck()
    {
        StartHasteCooldown();
        State = PlayerState.Action;
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Haste", true);
        animator.SetInteger("HasteDirection", currentDirection);
        RestoreAp(Mathf.RoundToInt(Stats.MaxAP * hasteApRefundPercentage));
        StartCoroutine(WaitAndGoBackFromHaste());
    }

    private IEnumerator WaitAndGoBackFromHaste()
    {
        yield return new WaitForSeconds(hasteTime);
        animator.SetBool("Haste", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("HasteDirection", 0);
        animator.SetInteger("IdleDirection", currentDirection);
        State = PlayerState.Neutral;
    }

    public void RestoreAp(int amount)
    {
        if (Stats.CurrentAP + amount >= Stats.MaxAP)
        {
            Stats.CurrentAP = Stats.MaxAP;
            FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Ap, $"+{Stats.MaxAP - Stats.CurrentAP} AP", transform.position);
        }
        else
        {
            Stats.CurrentAP += amount;
            FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Ap, $"+{amount} AP", transform.position);
        }
        hud.SetAp(Stats.CurrentAP);
    }

    public void DrinkAntidote()
    {
        if (State == PlayerState.Neutral && inventory.HasItem(antidote, 1)) // also fight and action points...
        {

            if (inFight)
            {
                if (UseAP(potionApCost))
                    DrinkAntidoteAfterCheck();
            }
            else
            {
                DrinkAntidoteAfterCheck();
            }
        }
    }

    private void DrinkAntidoteAfterCheck()
    {
        State = PlayerState.Action;
        inventory.TakeItem(antidote, 1);
        hud.UpdateAmounts();
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Antidote", true);
        animator.SetInteger("AntidoteDirection", currentDirection);

        int amount = (int)Math.Round(Stats.MaxToxicity * potionStrength);
        HealToxicity(amount);
        StartCoroutine(WaitAndGoBackFromDrinkingAntidote());
    }

    public IEnumerator WaitAndGoBackFromDrinkingAntidote()
    {
        yield return new WaitForSeconds(potionTime);
        animator.SetBool("Antidote", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("AntidoteDirection", 0);
        animator.SetInteger("IdleDirection", currentDirection);
        State = PlayerState.Neutral;
    }


    public void Reload()
    {
        if (State == PlayerState.Neutral && inventory.HasItem(ammo, 1) && Stats.CurrentMagazine != Stats.MagazineSize)
        {
            if (inFight)
            {
                if (UseAP(reloadApCost))
                    ReloadAfterCheck();
            }
            else
            {
                ReloadAfterCheck();
            }
        }
    }

    private void ReloadAfterCheck()
    {
        State = PlayerState.Action;
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Reload", true);
        animator.SetInteger("ReloadDirection", currentDirection);
        int empty = Stats.MagazineSize - Stats.CurrentMagazine;
        if (inventory.HasItem(ammo, empty))
        {
            inventory.TakeItem(ammo, empty);
            Stats.CurrentMagazine = Stats.MagazineSize;
        }
        else
        {
            int howMuch = inventory.list.Where(item => item.item == ammo).First().amount;
            inventory.TakeItem(ammo, howMuch);
            Stats.CurrentMagazine += howMuch;
        }
        hud.UpdateMagazine();
        hud.UpdateAmounts();
        StartCoroutine(WaitAndGoBackFromReload());
    }

    public IEnumerator WaitAndGoBackFromReload()
    {
        yield return new WaitForSeconds(reloadTime);
        animator.SetBool("Reload", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("ReloadDirection", 0);
        animator.SetInteger("IdleDirection", currentDirection);
        State = PlayerState.Neutral;
    }

    public void UseAmmo()
    {
        if (Stats.CurrentMagazine > 0)
        {
            Stats.CurrentMagazine -= 1;
            hud.UpdateMagazine();
        }
    }

    public void MakeTurn()
    {
        ReduceCooldowns();
        State = PlayerState.Neutral;
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
    }

    public void EndTurn()
    {
        if (State == PlayerState.Neutral && inFight)
        {
            Stats.CurrentAP = Stats.MaxAP;
            hud.SetAp(Stats.CurrentAP);
            FindObjectOfType<EncounterManager>().NextTurn();
            State = PlayerState.NotMyTurn;
            Unwail();
        }
    }

    public bool UseAP(int amount)
    {
        if (Stats.CurrentAP - amount >= 0)
        {
            Stats.CurrentAP -= amount;
            hud.SetAp(Stats.CurrentAP);
            FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.Ap, $"-{amount} AP", transform.position);
            return true;
        }
        else
            return false;
    }

    public IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(2f);
        //State = PlayerState.Neutral;
        EndTurn();
    }

    public void Shoot(Enemy enemy, bool snipe)
    {
        if (Stats.CurrentMagazine > 0)
        {
            if (StaticHelpers.CheckIfTargetIsInRange(enemy.CurrentGridPosition, currentGridPosition, gunRange))
            {
                if (UseAP(gunApCost))
                {
                    ShootAfterChecks(enemy, snipe);
                }
                else
                {
                    DidNotWork();
                }
            }
            else
            {
                APath path = mapManager.ChooseBestAPath(currentGridPosition, mapManager.FindNeighboursOfRange(enemy.CurrentGridPosition, gunRange));
                int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
                if (apcost == 0)
                    apcost = 1;
                apcost += gunApCost;
                if (UseAP(apcost))
                {
                    mapManager.StartMoving(path);
                    StartCoroutine(WaitForMoveAndShoot(enemy, snipe));
                }
                else
                {
                    DidNotWork();
                }
            }
        }
        else
        {
            DidNotWork();
        }
    }

    private void DidNotWork()
    {
        StartCoroutine(DidNotWorkCo());
    }

    private IEnumerator DidNotWorkCo()
    {
        yield return new WaitForEndOfFrame();
        State = PlayerState.Neutral;
        Vector2 hotspot = new Vector2(mainCursor.width / 2, 0);
        hud.DeactivateUsings();
        Cursor.SetCursor(mainCursor, hotspot, CursorMode.ForceSoftware);
    }

    private IEnumerator WaitForMoveAndShoot(Enemy enemy, bool snipe)
    {
        yield return new WaitUntil(() => State == PlayerState.Neutral);
        StartCoroutine(WaitJustABitAndShoot(enemy, snipe));
    }

    private IEnumerator WaitJustABitAndShoot(Enemy enemy, bool snipe)
    {
        yield return new WaitForSeconds(0.05f);
        ShootAfterChecks(enemy, snipe);
    }

    private void ShootAfterChecks(Enemy enemy, bool snipe)
    {
        State = PlayerState.Action;
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Shoot", true);
        int shootDir = StaticHelpers.ChooseDir(transform.position, enemy.transform.position);
        animator.SetInteger("ShootDirection", shootDir);
        currentDirection = shootDir;
        if (!snipe)
            enemy.TakeDamage(StaticHelpers.RollDamage(gunMinDice, gunMaxDice, gunModifier));
        else
        {
            StartSnipeCooldown();
            enemy.TakeDamage(StaticHelpers.RollDamage(gunMinDice, gunMaxDice, gunModifier) * snipeModifier); // maybe
        }
        Stats.CurrentMagazine -= 1;
        hud.UpdateMagazine();
        Vector2 hotspot = new Vector2(mainCursor.width / 2, 0);
        hud.DeactivateUsings();
        Cursor.SetCursor(mainCursor, hotspot, CursorMode.ForceSoftware);
        StartCoroutine(WaitForGunLight());
        StartCoroutine(WaitAndGoBackFromShooting());
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

    public IEnumerator WaitAndGoBackFromShooting()
    {
        yield return new WaitForSeconds(shootTime);
        animator.SetBool("Shoot", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("ShootDirection", 0);
        animator.SetInteger("IdleDirection", currentDirection);
        State = PlayerState.Neutral;
    }

    public void Flurry(Enemy enemy)
    {
        int meleeRange = 1;
        int flurryApCost = Mathf.RoundToInt(meleeApCost * flurryApCostModifier);
        if (StaticHelpers.CheckIfTargetIsInRange(enemy.CurrentGridPosition, currentGridPosition, meleeRange))
        {
            if (UseAP(flurryApCost))
            {
                FlurryAfterChecks(enemy);
            }
            else
            {
                DidNotWork();
            }
        }
        else
        {
            APath path = mapManager.ChooseBestAPath(currentGridPosition, mapManager.FindNeighboursOfRange(enemy.CurrentGridPosition, meleeRange));
            int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
            if (apcost == 0)
                apcost = 1;
            apcost += flurryApCost;
            if (UseAP(apcost))
            {
                mapManager.StartMoving(path);
                StartCoroutine(WaitForMoveAndFlurry(enemy));
            }
            else
            {
                DidNotWork();
            }
        }
    }

    private IEnumerator WaitForMoveAndFlurry(Enemy enemy)
    {
        yield return new WaitUntil(() => State == PlayerState.Neutral);
        StartCoroutine(WaitJustABitAndFlurry(enemy));
    }

    private IEnumerator WaitJustABitAndFlurry(Enemy enemy)
    {
        yield return new WaitForSeconds(0.05f);
        FlurryAfterChecks(enemy);
    }

    private void FlurryAfterChecks(Enemy enemy)
    {
        StartFlurryCooldown();
        State = PlayerState.Action;
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Melee", true);
        int meleeDir = StaticHelpers.ChooseDir(transform.position, enemy.transform.position);
        currentDirection = meleeDir;
        hud.DeactivateUsings();
        Vector2 hotspot = new Vector2(mainCursor.width / 2, 0);
        Cursor.SetCursor(mainCursor, hotspot, CursorMode.ForceSoftware);

        animator.SetInteger("MeleeDirection", meleeDir);
        enemy.TakeDamage(StaticHelpers.RollDamage(meleeMinDice, meleeMaxDice, meleeModifier));
        StartCoroutine(OneFlurryHit(enemy, meleeDir));
        //StartCoroutine(WaitAndGoBackFromMeleeing());
    }

    private IEnumerator OneFlurryHit(Enemy enemy, int meleeDir)
    {
        Debug.Log(flurryCounter);
        if (flurryCounter < (flurryHits - 1))
        {
            yield return new WaitForSeconds(meleeTime);
            animator.SetInteger("MeleeDirection", 0);
            StartCoroutine(WaitAndFlurryAgain(enemy, meleeDir));
        }
        else
        {
            yield return new WaitForSeconds(meleeTime);
            flurryCounter = 0;
            animator.SetBool("Melee", false);
            animator.SetBool("Idle", true);
            animator.SetInteger("MeleeDirection", 0);
            animator.SetInteger("IdleDirection", currentDirection);
            State = PlayerState.Neutral;
        }
    }

    private IEnumerator WaitAndFlurryAgain(Enemy enemy, int meleeDir)
    {
        yield return new WaitForSeconds(0.01f);
        animator.SetInteger("MeleeDirection", meleeDir);
        enemy.TakeDamage(StaticHelpers.RollDamage(meleeMinDice, meleeMaxDice, meleeModifier));
        flurryCounter++;
        StartCoroutine(OneFlurryHit(enemy, meleeDir));
    }


    public void Melee(Enemy enemy)
    {
        int meleeRange = 1;
        if (StaticHelpers.CheckIfTargetIsInRange(enemy.CurrentGridPosition, currentGridPosition, meleeRange))
        {
            if (UseAP(meleeApCost))
            {
                MeleeAfterChecks(enemy);
            }
            else
            {
                DidNotWork();
            }
        }
        else
        {
            APath path = mapManager.ChooseBestAPath(currentGridPosition, mapManager.FindNeighboursOfRange(enemy.CurrentGridPosition, meleeRange));
            int apcost = (int)Math.Round(path.DijkstraScore / Stats.Movement);
            if (apcost == 0)
                apcost = 1;
            apcost += meleeApCost;
            if (UseAP(apcost))
            {
                mapManager.StartMoving(path);
                StartCoroutine(WaitForMoveAndMelee(enemy));
            }
            else
            {
                DidNotWork();
            }
        }
    }

    private void MeleeAfterChecks(Enemy enemy)
    {
        State = PlayerState.Action;
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Melee", true);
        int meleeDir = StaticHelpers.ChooseDir(transform.position, enemy.transform.position);
        animator.SetInteger("MeleeDirection", meleeDir);
        currentDirection = meleeDir;
        enemy.TakeDamage(StaticHelpers.RollDamage(meleeMinDice, meleeMaxDice, meleeModifier));
        hud.DeactivateUsings();
        Vector2 hotspot = new Vector2(mainCursor.width / 2, 0);
        Cursor.SetCursor(mainCursor, hotspot, CursorMode.ForceSoftware);
        StartCoroutine(WaitAndGoBackFromMeleeing());
    }

    private IEnumerator WaitForMoveAndMelee(Enemy enemy)
    {
        yield return new WaitUntil(() => State == PlayerState.Neutral);
        StartCoroutine(WaitJustABitAndMelee(enemy));
    }

    private IEnumerator WaitJustABitAndMelee(Enemy enemy)
    {
        yield return new WaitForSeconds(0.05f);
        MeleeAfterChecks(enemy);
    }

    public IEnumerator WaitAndGoBackFromMeleeing()
    {
        yield return new WaitForSeconds(meleeTime);
        animator.SetBool("Melee", false);
        animator.SetBool("Idle", true);
        animator.SetInteger("MeleeDirection", 0);
        animator.SetInteger("IdleDirection", currentDirection);
        State = PlayerState.Neutral;
    }

    public void EndCombat()
    {
        inFight = false;
        Stats.CurrentAP = Stats.MaxAP;
        hud.SetAp(Stats.CurrentAP);
        Unwail();
        ResetCooldowns();
        hud.ResetCooldowns();
    }

}
