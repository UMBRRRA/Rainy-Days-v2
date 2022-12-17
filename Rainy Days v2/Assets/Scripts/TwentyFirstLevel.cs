using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwentyFirstLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;
    public Encounter encounter;
    private MainCanvas immortal;
    private bool hasAmulet, knowsPassword;
    public ItemObject amulet;
    private static int rebelQuestId = 7;
    private static int knowsPassQuestId = 2;
    public DialogueTrigger trigger;
    private static int doorOpenDialogueId = 1;
    void Start()
    {

        GoNeutral();
        StartCoroutine(FindImmortal());
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        yield return new WaitUntil(() => knowsPassword && hasAmulet);
        trigger.ChangeDialogue(doorOpenDialogueId);
    }

    public void GoNeutral()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        StartCoroutine(WaitABit());
        if (player.inventory.HasItem(amulet, 1))
            hasAmulet = true;
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(abit);
        player.State = PlayerState.Neutral;
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (!immortal.FinishedEncounters.ContainsKey(encounter.encounterId))
        {
            StartCoroutine(StartEncounter());
        }
        if (immortal.Quests[rebelQuestId] == knowsPassQuestId)
            knowsPassword = true;
    }

    private IEnumerator StartEncounter()
    {
        yield return new WaitUntil(() => FindObjectOfType<Player>().State == PlayerState.Neutral);
        encounter.StartEncounter();
    }
}
