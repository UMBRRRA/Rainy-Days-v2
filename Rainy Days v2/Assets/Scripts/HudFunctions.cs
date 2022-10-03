using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HudFunctions : MonoBehaviour
{
    public GameObject child;
    public InventoryObject inventory;
    public Text ammoAmount;
    public Text potionAmount;
    public Text antidoteAmount;
    public ItemObject ammo;
    public ItemObject potion;
    public ItemObject antidote;
    public Slider healthSlider, toxicitySlider, apSlider;
    public Text maxHealth, currentHealth, maxToxicity, currentToxicity, maxAP, currentAP, magazine;
    public GameObject endTurn;
    public GameObject usingMelee, usingShoot;
    public GameObject meleeTooltip, gunTooltip, reloadTooltip, potionTooltip, antidoteTooltip, endTurnTooltip;

    private Player player;

    public Texture2D attackCursor;

    public void ActivateHud()
    {
        SetTooltips();
        child.SetActive(true);
    }

    public void DeactivateHud()
    {
        DeactivateTooltips();
        usingMelee.SetActive(false);
        usingShoot.SetActive(false);
        child.SetActive(false);
    }

    private void DeactivateTooltips()
    {
        meleeTooltip.SetActive(false);
        gunTooltip.SetActive(false);
        reloadTooltip.SetActive(false);
        potionTooltip.SetActive(false);
        antidoteTooltip.SetActive(false);
        endTurnTooltip.SetActive(false);
    }

    public void SetTooltips()
    {
        StartCoroutine(SetTooltipsCo());
    }

    private IEnumerator SetTooltipsCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        string meleeText = $"Melee (1)\n" +
            $"Damage: {player.meleeMinDice + player.meleeModifier}-{player.meleeMaxDice + player.meleeModifier}\n" +
            $"AP: {player.meleeApCost}";
        meleeTooltip.GetComponentInChildren<Text>().text = meleeText;
        string gunText = $"Gun (2)\n" +
            $"Damage: {player.gunMinDice + player.gunModifier}-{player.gunMaxDice + player.gunModifier}\n" +
            $"AP: {player.gunApCost}";
        gunTooltip.GetComponentInChildren<Text>().text = gunText;
        string reloadText = $"Reload (3)\n" +
            $"AP: {player.reloadApCost}";
        reloadTooltip.GetComponentInChildren<Text>().text = reloadText;
        int potionHeals = (int)Math.Round(player.Stats.MaxHealth * player.potionStrength);
        string potionText = $"Potion (4)\n" +
            $"Health: +{potionHeals}\n" +
            $"AP: {player.potionApCost}";
        potionTooltip.GetComponentInChildren<Text>().text = potionText;
        int antidoteHeals = (int)Math.Round(player.Stats.MaxToxicity * player.potionStrength);
        string antidoteText = $"Antidote (5)\n" +
            $"Toxicity: -{antidoteHeals}\n" +
            $"AP: {player.potionApCost}";
        antidoteTooltip.GetComponentInChildren<Text>().text = antidoteText;
    }

    public void UpdateAmounts()
    {
        // ammo
        if (inventory.HasItem(ammo, 1))
        {
            ammoAmount.text = $"{inventory.list.Where(itemSlot => itemSlot.item == ammo).First().amount}";
        }
        else
        {
            ammoAmount.text = "0";
        }

        // potion
        if (inventory.HasItem(potion, 1))
        {
            potionAmount.text = $"{inventory.list.Where(itemSlot => itemSlot.item == potion).First().amount}";
        }
        else
        {
            potionAmount.text = "0";
        }

        // antidote
        if (inventory.HasItem(antidote, 1))
        {
            antidoteAmount.text = $"{inventory.list.Where(itemSlot => itemSlot.item == antidote).First().amount}";
        }
        else
        {
            antidoteAmount.text = "0";
        }
    }


    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        maxHealth.text = $"{health}";
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
        currentHealth.text = $"{health}";
    }

    public void SetMaxToxicity(int toxicity)
    {
        toxicitySlider.maxValue = toxicity;
        maxToxicity.text = $"{toxicity}";
    }

    public void SetToxicity(int toxicity)
    {
        toxicitySlider.value = toxicity;
        currentToxicity.text = $"{toxicity}";
    }

    public void SetMaxAp(int ap)
    {
        apSlider.maxValue = ap;
        maxAP.text = $"{ap}";
    }

    public void SetAp(int ap)
    {
        apSlider.value = ap;
        currentAP.text = $"{ap}";
    }

    public void DrinkPotion()
    {
        StartCoroutine(FindPlayerAndDrinkPotion());
    }

    public IEnumerator FindPlayerAndDrinkPotion()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.DrinkPotion();
    }

    public void DrinkAntidote()
    {
        StartCoroutine(FindPlayerAndDrinkAntidote());
    }

    public IEnumerator FindPlayerAndDrinkAntidote()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.DrinkAntidote();
    }

    public void Reload()
    {
        StartCoroutine(FindPlayerAndReload());
    }

    public IEnumerator FindPlayerAndReload()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.Reload();
    }

    public void UpdateMagazine()
    {
        StartCoroutine(UpdateMag());
    }

    private IEnumerator UpdateMag()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        magazine.text = $"{player.Stats.CurrentMagazine}/{player.Stats.MagazineSize}";
    }

    public void StartEncounter()
    {
        //endTurn.SetActive(true);
    }

    public void EndEncounter()
    {
        //endTurn.SetActive(false);
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnCo());
    }

    public IEnumerator EndTurnCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.EndTurn();
    }

    public void Shoot()
    {
        StartCoroutine(ShootCo());
    }

    private IEnumerator ShootCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if ((player.State == PlayerState.Neutral || player.State == PlayerState.Meleeing)
            && player.inFight && player.Stats.CurrentMagazine != 0)
        {
            player.State = PlayerState.Shooting;
            DeactivateUsings();
            usingShoot.SetActive(true);
            Vector2 hotspot = new Vector2(attackCursor.width / 2, 0);
            Cursor.SetCursor(attackCursor, hotspot, CursorMode.ForceSoftware);
        }
    }

    public void Melee()
    {
        StartCoroutine(MeleeCo());
    }

    private IEnumerator MeleeCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if ((player.State == PlayerState.Neutral || player.State == PlayerState.Shooting)
            && player.inFight)
        {
            player.State = PlayerState.Meleeing;
            DeactivateUsings();
            usingMelee.SetActive(true);
            Vector2 hotspot = new Vector2(attackCursor.width / 2, 0);
            Cursor.SetCursor(attackCursor, hotspot, CursorMode.ForceSoftware);
        }
    }

    public void DeactivateUsings()
    {
        usingMelee.SetActive(false);
        usingShoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Melee();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Reload();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DrinkPotion();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DrinkAntidote();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EndTurn();
        }
    }

}
