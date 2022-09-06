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

}
