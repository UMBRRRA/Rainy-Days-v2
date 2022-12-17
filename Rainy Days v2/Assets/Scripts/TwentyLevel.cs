using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwentyLevel : MonoBehaviour
{
    private MainCanvas immortal;
    public GameObject villagers;
    public GameObject maxim;
    private Player player;
    private static int villagersQuestId = 8;
    public DialogueTrigger trigger;
    public static float abit = 2f;
    private Shadow shadow;
    public static int villagersQuestExp = 800;

    void Start()
    {
        StartCoroutine(FindImmortal());
        shadow = FindObjectOfType<Shadow>();
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (immortal.Quests[villagersQuestId] == 0)
        {
            StartCoroutine(VillagersCo());
        }
        else
        {
            Destroy(villagers);
            GoNeutral();
        }
    }

    private IEnumerator VillagersCo()
    {
        yield return new WaitForSeconds(abit);
        FindObjectOfType<CameraFollow>().Setup(() => maxim.transform.position);
        FindObjectOfType<Player>().State = PlayerState.Neutral;
        trigger.TriggerDialogue();
        immortal.Quests[villagersQuestId] = 1;
    }

    public void VillagersTalkEnd()
    {
        shadow.CloseShadow();
        StartCoroutine(OpenShadow());
    }


    private IEnumerator OpenShadow()
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        FindObjectOfType<CameraFollow>().Setup(() => FindObjectOfType<Player>().transform.position);
        shadow.OpenShadow();
        FindObjectOfType<Player>().State = PlayerState.Neutral;
        FindObjectOfType<Player>().exp += villagersQuestExp;
        FindObjectOfType<HudFunctions>().ActivateHud();
        Destroy(villagers);
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
}
