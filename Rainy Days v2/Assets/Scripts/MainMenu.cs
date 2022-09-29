using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private MainCanvas imortal;
    public InventoryObject inventory;
    private HudFunctions hud;
    void Start()
    {
        Debug.Log("Clearing save data.");
        inventory.list.Clear();
        StartCoroutine(WaitForDialogueManager());
        StartCoroutine(WaitForHUD());
        StartCoroutine(WaitForImortal());
    }



    private IEnumerator WaitForImortal()
    {
        yield return new WaitUntil(() => (imortal = FindObjectOfType<MainCanvas>()) != null);
        imortal.FinishedEncounters.Clear();
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
        hud.UpdateMagazine();
        hud.EndEncounter();
    }

}
