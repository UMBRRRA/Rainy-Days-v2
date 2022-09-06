using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public InventoryObject inventory;
    public HudFunctions hud;
    void Start()
    {
        Debug.Log("Clearing save data.");
        dialogueManager.DialoguesSave.Clear();
        inventory.list.Clear();
        StartCoroutine(WaitAndUpdateAmounts());
    }

    public IEnumerator WaitAndUpdateAmounts()
    {
        yield return new WaitForSeconds(5f);
        hud.UpdateAmounts();
    }
}
