using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwelveLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;
    public Encounter encounter;
    private MainCanvas immortal;
    public GameObject rubble;
    public int rubbleQuestId = 6;
    public InventoryObject inventory;
    public ItemObject dynamite;
    public int gotDynamiteDialogueId = 1;
    public int dynamiteQuestXP = 500;
    public Vector3Int walkDynamiteQuestPos;
    private Shadow shadow;
    void Start()
    {
        GoNeutral();
        StartCoroutine(FindImmortal());
        shadow = FindObjectOfType<Shadow>();
    }



    public void GoNeutral()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        StartCoroutine(WaitABit());
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(abit);
        player.State = PlayerState.Neutral;
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (immortal.Quests[rubbleQuestId] == 0)
        {
            rubble.SetActive(true);
            if (inventory.HasItem(dynamite, 1))
            {
                rubble.GetComponent<DialogueTrigger>().ChangeDialogue(1);
            }
        }
        else
        {
            rubble.SetActive(false);
        }
        if (!immortal.FinishedEncounters.ContainsKey(encounter.encounterId))
        {
            StartCoroutine(StartEncounter());
        }
    }

    private IEnumerator StartEncounter()
    {
        yield return new WaitUntil(() => FindObjectOfType<Player>().State == PlayerState.Neutral);
        encounter.StartEncounter();
    }

    public void FireDynamite()
    {
        immortal.Quests[rubbleQuestId] = 1;
        FindObjectOfType<HudFunctions>().DeactivateHud();
        FindObjectOfType<MapManager>().WhenClicked(walkDynamiteQuestPos);
        StartCoroutine(WalkAway());
    }

    private IEnumerator WalkAway()
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        player.IdleDirectionNoNeutral(3);
        player.State = PlayerState.Event;
        rubble.GetComponent<Animator>().SetBool("blowingUp", true);
        player.exp += dynamiteQuestXP;
        shadow.CloseShadow();
        StartCoroutine(WaitAndComeBack());
    }

    private IEnumerator WaitAndComeBack()
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        player.State = PlayerState.Neutral;
        FindObjectOfType<HudFunctions>().ActivateHud();
        shadow.OpenShadow();
        rubble.SetActive(false);
    }
}

