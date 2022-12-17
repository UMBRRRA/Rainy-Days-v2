using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeventeenthLevel : MonoBehaviour
{

    private MainCanvas immortal;
    public GameObject rebel;
    private Player player;
    private static int rebelFirstTalkQuestId = 7;
    public DialogueTrigger trigger;
    public static float abit = 2f;

    void Start()
    {
        StartCoroutine(FindImmortal());
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (immortal.Quests[rebelFirstTalkQuestId] == 0)
        {
            StartCoroutine(RebelApproachCo());
        }
        else
        {
            GoNeutral();
        }
    }

    private IEnumerator RebelApproachCo()
    {
        yield return new WaitForSeconds(abit);
        FindObjectOfType<CameraFollow>().Setup(() => rebel.transform.position);
        FindObjectOfType<Player>().State = PlayerState.Neutral;
        trigger.TriggerDialogue();
        immortal.Quests[rebelFirstTalkQuestId] = 1;
    }

    public void RebelFirstTalkEnd()
    {
        FindObjectOfType<CameraFollow>().Setup(() => FindObjectOfType<Player>().transform.position);
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
        FindObjectOfType<HudFunctions>().ActivateHud();
        player.State = PlayerState.Neutral;
    }

    public void InstallExtendedMag()
    {
        FindObjectOfType<CharacterMenuFunctions>().ExtendedMag();
    }

    public void RebelPasswordQuest()
    {
        immortal.Quests[rebelFirstTalkQuestId] = 2;
    }
}
