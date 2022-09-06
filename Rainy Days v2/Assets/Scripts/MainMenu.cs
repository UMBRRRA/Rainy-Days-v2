using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private DialogueManager dialogueManager;
    public InventoryObject inventory;
    private HudFunctions hud;
    void Start()
    {
        Debug.Log("Clearing save data.");
        inventory.list.Clear();
        StartCoroutine(WaitForDialogueManager());
        StartCoroutine(WaitForHUD());
    }

    public IEnumerator WaitForDialogueManager()
    {
        yield return new WaitUntil(() => (dialogueManager = FindObjectOfType<DialogueManager>()) != null);
        dialogueManager.DialoguesSave.Clear();
    }

    public IEnumerator WaitForHUD()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.UpdateAmounts();
    }

}
