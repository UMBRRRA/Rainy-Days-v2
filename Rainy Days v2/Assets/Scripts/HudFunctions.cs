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
    public GameObject usingMelee, usingShoot, usingSnipe, usingFlurry;
    public GameObject meleeTooltip, gunTooltip, reloadTooltip, potionTooltip, antidoteTooltip, snipeTooltip, flurryTooltip, hasteTooltip, endTurnTooltip;

    public GameObject snipe, flurry, haste, snipeBox, flurryBox, hasteBox;
    public bool snipeActive = false;
    public bool flurryActive = false;
    public bool hasteActive = false;

    public GameObject snipeCant;
    public Text snipeCooldownText;

    public GameObject hasteCant;
    public Text hasteCooldownText;

    public GameObject flurryCant;
    public Text flurryCooldownText;

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
        DeactivateUsings();
        child.SetActive(false);
    }

    public void ResetCooldowns()
    {
        snipeCant.SetActive(false);
        snipeCooldownText.text = "";
        hasteCant.SetActive(false);
        hasteCooldownText.text = "";
        flurryCant.SetActive(false);
        flurryCooldownText.text = "";
    }

    public void SetSnipeCooldown()
    {
        if (player.snipeCurrentCooldown == 0)
        {
            snipeCant.SetActive(false);
            snipeCooldownText.text = "";
        }
        else
        {
            snipeCant.SetActive(true);
            snipeCooldownText.text = $"{player.snipeCurrentCooldown}";
        }

    }
    public void SetHasteCooldown()
    {
        if (player.hasteCurrentCooldown == 0)
        {
            hasteCant.SetActive(false);
            hasteCooldownText.text = "";
        }
        else
        {
            hasteCant.SetActive(true);
            hasteCooldownText.text = $"{player.hasteCurrentCooldown}";
        }

    }

    public void SetFlurryCooldown()
    {
        if (player.flurryCurrentCooldown == 0)
        {
            flurryCant.SetActive(false);
            flurryCooldownText.text = "";
        }
        else
        {
            flurryCant.SetActive(true);
            flurryCooldownText.text = $"{player.flurryCurrentCooldown}";
        }
    }

    private void DeactivateTooltips()
    {
        meleeTooltip.SetActive(false);
        gunTooltip.SetActive(false);
        reloadTooltip.SetActive(false);
        potionTooltip.SetActive(false);
        antidoteTooltip.SetActive(false);
        endTurnTooltip.SetActive(false);
        snipeTooltip.SetActive(false);
        flurryTooltip.SetActive(false);
        hasteTooltip.SetActive(false);
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
        string snipeText = $"Snipe (6)\n" +
            $"Damage: {(player.gunMinDice + player.gunModifier) * player.snipeModifier}-{(player.gunMaxDice + player.gunModifier) * player.snipeModifier}\n" +
            $"AP: {player.gunApCost}";
        snipeTooltip.GetComponentInChildren<Text>().text = snipeText;
        string hasteText = $"Haste (8)\n" +
            $"AP: +{Mathf.RoundToInt(player.Stats.MaxAP * player.hasteApRefundPercentage)}";
        hasteTooltip.GetComponentInChildren<Text>().text = hasteText;
        string flurryText = $"Flurry (7)\n" +
            $"Damage: {(player.meleeMinDice + player.meleeModifier) * player.flurryHits}-{(player.meleeMaxDice + player.meleeModifier) * player.flurryHits}\n" +
            $"AP: {Mathf.RoundToInt(player.meleeApCost * player.flurryApCostModifier)}";
        flurryTooltip.GetComponentInChildren<Text>().text = flurryText;
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
        DeactivateUsings();
        player.DrinkPotion();
    }

    public void DrinkAntidote()
    {
        StartCoroutine(FindPlayerAndDrinkAntidote());
    }

    public IEnumerator FindPlayerAndDrinkAntidote()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        DeactivateUsings();
        player.DrinkAntidote();
    }

    public void Reload()
    {
        StartCoroutine(FindPlayerAndReload());
    }

    public IEnumerator FindPlayerAndReload()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        DeactivateUsings();
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
        if ((player.State == PlayerState.Neutral || player.State == PlayerState.Meleeing || player.State == PlayerState.Snipe
            || player.State == PlayerState.Flurry)
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
        if ((player.State == PlayerState.Neutral || player.State == PlayerState.Shooting || player.State == PlayerState.Snipe
            || player.State == PlayerState.Flurry)
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
        usingSnipe.SetActive(false);
        usingFlurry.SetActive(false);
    }

    public void Flurry()
    {
        StartCoroutine(FlurryCo());
    }

    private IEnumerator FlurryCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if ((player.State == PlayerState.Neutral || player.State == PlayerState.Shooting || player.State == PlayerState.Snipe
            || player.State == PlayerState.Meleeing)
            && player.inFight && player.flurryCurrentCooldown == 0)
        {
            player.State = PlayerState.Flurry;
            DeactivateUsings();
            usingFlurry.SetActive(true);
            Vector2 hotspot = new Vector2(attackCursor.width / 2, 0);
            Cursor.SetCursor(attackCursor, hotspot, CursorMode.ForceSoftware);
        }
    }

    public void Snipe()
    {
        StartCoroutine(SnipeCo());
    }

    private IEnumerator SnipeCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if ((player.State == PlayerState.Neutral || player.State == PlayerState.Meleeing || player.State == PlayerState.Snipe
            || player.State == PlayerState.Flurry)
            && player.inFight && player.Stats.CurrentMagazine != 0 && player.snipeCurrentCooldown == 0)
        {
            player.State = PlayerState.Snipe;
            DeactivateUsings();
            usingSnipe.SetActive(true);
            Vector2 hotspot = new Vector2(attackCursor.width / 2, 0);
            Cursor.SetCursor(attackCursor, hotspot, CursorMode.ForceSoftware);
        }
    }

    public void Haste()
    {
        StartCoroutine(FindPlayerAndHaste());
    }

    public IEnumerator FindPlayerAndHaste()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        DeactivateUsings();
        player.Haste();
    }

    public void RestartGame()
    {
        snipe.SetActive(false);
        flurry.SetActive(false);
        haste.SetActive(false);
        snipeBox.SetActive(true);
        flurryBox.SetActive(true);
        hasteBox.SetActive(true);
        snipeActive = false;
        flurryActive = false;
        hasteActive = false;
        ResetCooldowns();
    }

    public void ActivateSnipe()
    {
        snipe.SetActive(true);
        snipeBox.SetActive(false);
        snipeActive = true;
    }

    public void ActivateFlurry()
    {
        flurry.SetActive(true);
        flurryBox.SetActive(false);
        flurryActive = true;
    }

    public void ActivateHaste()
    {
        haste.SetActive(true);
        hasteBox.SetActive(false);
        hasteActive = true;
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
        else if (Input.GetKeyDown(KeyCode.Alpha6) && snipeActive)
        {
            Snipe();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && flurryActive)
        {
            Flurry();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) && hasteActive)
        {
            Haste();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EndTurn();
        }
    }

}
