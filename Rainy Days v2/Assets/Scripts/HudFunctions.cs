using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    private Player player;

    public void ActivateHud()
    {
        child.SetActive(true);
    }

    public void DeactivateHud()
    {
        child.SetActive(false);
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
        endTurn.SetActive(true);
    }

    public void EndEncounter()
    {
        endTurn.SetActive(false);
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
        if (player.State == PlayerState.Neutral && player.inFight && player.Stats.CurrentMagazine != 0)
        {
            player.State = PlayerState.Shooting;
        }
    }

    public void Melee()
    {
        StartCoroutine(MeleeCo());
    }

    private IEnumerator MeleeCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if (player.State == PlayerState.Neutral && player.inFight)
        {
            player.State = PlayerState.Meleeing;
        }
    }

}
