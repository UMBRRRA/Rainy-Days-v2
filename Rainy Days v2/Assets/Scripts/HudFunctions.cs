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
    public Text maxHealth, currentHealth, maxToxicity, currentToxicity, maxAP, currentAP;

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

}
