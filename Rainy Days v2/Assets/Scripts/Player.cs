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
    Meleeing
}

public class Player : MonoBehaviour
{
    public int startHealth;
    public int startToxicity;
    public int startAP;
    public int startMag;
    public int startInitative;

    public PlayerStats Stats { get; set; }

    private static Player singleton;

    public float playerZ = 1.2f;
    public Vector3Int spawnGridPosition;
    public Vector3Int currentGridPosition;
    public MapManager mapManager;
    public int spawnDirection;
    public int currentDirection;
    private Animator animator;
    public Vector3 NextField { get; set; }
    private PauseMenuFunctions pauseMenu;
    private HudFunctions hud;
    public float potionStrength = 0.33f;
    public float potionTime = 1f;
    public float reloadTime = 1f;
    public float shootTime = 1.2f;
    public int potionApCost = 1;
    public int reloadApCost = 1;
    public int gunApCost = 1;
    public int meleeApCost = 1;

    public ItemObject potion, antidote, ammo;
    public InventoryObject inventory;

    public bool inFight = false;

    private PlayerState _state = PlayerState.Neutral;
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
        Stats = new PlayerStats(this, startHealth, startAP, startInitative, startToxicity, startMag);
        currentGridPosition = spawnGridPosition;
        NextField = transform.position;
        mapManager = FindObjectOfType<MapManager>();
        animator = GetComponent<Animator>();
        IdleDirection(spawnDirection);
        StartCoroutine(FindHud());
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
        Debug.Log("Am i here?");
    }

    public void ChangeRoom()
    {
        currentGridPosition = spawnGridPosition;
        Vector3 pos = mapManager.map.CellToWorld(spawnGridPosition);
        transform.position = new Vector3(pos.x, pos.y + 0.25f, playerZ);
        IdleDirection(spawnDirection);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && State == PlayerState.Neutral)
        {
            State = PlayerState.Pause;
            StartCoroutine(OpenPauseMenu());
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (State == PlayerState.Shooting || State == PlayerState.Meleeing)
            {
                State = PlayerState.Neutral;
            }
        }
    }

    public IEnumerator OpenPauseMenu()
    {
        yield return new WaitUntil(() => (pauseMenu = FindObjectOfType<PauseMenuFunctions>()) != null);
        pauseMenu.ActivatePauseMenu();
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

    public IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nf)
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

        State = PlayerState.Neutral;
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
        hud.SetHealth(Stats.CurrentHealth);
    }

    public void DamageHealth(int amount)
    {
        if (Stats.CurrentHealth - amount <= 0)
        {
            Stats.CurrentHealth = 0;
            // dead
        }
        else
        {
            Stats.CurrentHealth -= amount;
        }
        hud.SetHealth(Stats.CurrentHealth);
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
        hud.SetToxicity(Stats.CurrentToxicity);
    }

    public void IncreaseToxicity(int amount)
    {
        if (Stats.CurrentToxicity + amount >= Stats.MaxToxicity)
        {
            Stats.CurrentToxicity = Stats.MaxToxicity;
            // dead
        }
        else
        {
            Stats.CurrentToxicity += amount;
        }
        hud.SetToxicity(Stats.CurrentToxicity);
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
        State = PlayerState.Neutral;
        Debug.Log($"I am player and its my turn, my initative is {Stats.Initiative}");
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
    }

    public void EndTurn()
    {
        if (State == PlayerState.Neutral)
        {
            Stats.CurrentAP = Stats.MaxAP;
            hud.SetAp(Stats.CurrentAP);
            FindObjectOfType<EncounterManager>().NextTurn();
            State = PlayerState.NotMyTurn;
        }
    }

    public bool UseAP(int amount)
    {
        if (Stats.CurrentAP - amount >= 0)
        {
            Stats.CurrentAP -= amount;
            hud.SetAp(Stats.CurrentAP);
            if (Stats.CurrentAP == 0)
                StartCoroutine(WaitAndEndTurn());
            return true;
        }
        else
            return false;
    }

    public IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(2f);
        EndTurn();
    }

    public void Shoot(Enemy enemy)
    {
        if (Stats.CurrentMagazine > 0)
        {
            if (UseAP(gunApCost))
            {
                State = PlayerState.Action;
                animator.SetInteger("IdleDirection", 0);
                animator.SetBool("Idle", false);
                animator.SetBool("Shoot", true);
                int shootDir = ChooseDir(transform.position, enemy.transform.position);
                animator.SetInteger("ShootDirection", shootDir);
                currentDirection = shootDir;
                Stats.CurrentMagazine -= 1;
                hud.UpdateMagazine();
                StartCoroutine(WaitAndGoBackFromShooting());
            }
        }
    }

    public int ChooseDir(Vector3 me, Vector3 they)
    {
        Vector3 newMe = new(me.x, me.y, 0);
        Vector3 newThey = new(they.x, they.y, 0);
        Debug.Log(newMe - newThey);


        Vector3 normal = (newMe - newThey).normalized;
        Debug.Log(normal);
        if (normal.x <= -0.28 && normal.y >= 0.38)
            return 1;
        else if (normal.x <= -0.28 && (normal.y > -0.28 && normal.y < 0.38))
            return 2;
        else if (normal.x <= -0.28 && normal.y <= -0.28)
            return 3;
        else if ((normal.x > -0.28 && normal.x < 0.38) && normal.y <= -0.28)
            return 4;
        else if (normal.x >= 0.38 && normal.y <= -0.28)
            return 5;
        else if (normal.x >= 0.38 && (normal.y > -0.28 && normal.y < 0.38))
            return 6;
        else if (normal.x >= 0.38 && normal.y >= 0.38)
            return 7;
        else
            return 8;
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
}
