using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthLevel : MonoBehaviour
{
    private MainCanvas immortal;
    public static int loversQuestId = 2;
    private Player player;
    public static float abit = 2f;
    public Encounter loversEncounter;
    public GameObject roman;
    public Vector3 romanPosition;
    private CameraFollow cameraFollow;

    void Start()
    {
        GoNeutral();
        StartCoroutine(FindImmortal());
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (immortal.Quests[loversQuestId] == 0 || immortal.Quests[loversQuestId] == 1)
        {
            GameObject realRoman = Instantiate(roman, romanPosition, Quaternion.identity);
            realRoman.GetComponent<DialogueTrigger>().TriggerDialogue();
            StartCoroutine(WaitForCamera());
        }
        else
        {
            FindObjectOfType<HudFunctions>().ActivateHud();
        }
    }


    private IEnumerator WaitForCamera()
    {
        yield return new WaitUntil(() => (cameraFollow = FindObjectOfType<CameraFollow>()) != null);
        StartCoroutine(WaitAndSetupCamera());
    }

    private IEnumerator WaitAndSetupCamera()
    {
        yield return new WaitForSeconds(0.5f);
        cameraFollow.Setup(() => romanPosition);
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

    public void StartEncounter()
    {
        StartCoroutine(StartEncounterCo());
    }

    private IEnumerator StartEncounterCo()
    {
        yield return new WaitUntil(() => FindObjectOfType<Player>().State == PlayerState.Neutral);
        loversEncounter.StartEncounter();
    }

}
