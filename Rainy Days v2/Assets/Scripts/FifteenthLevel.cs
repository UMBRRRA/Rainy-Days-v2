using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FifteenthLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;
    public Encounter encounter;
    private MainCanvas immortal;

    public Enemy cultist;
    public Vector3Int walkDestination;
    public DialogueTrigger trigger;

    void Start()
    {
        StartCoroutine(FindImmortal());
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
        if (!immortal.FinishedEncounters.ContainsKey(encounter.encounterId))
        {
            CultistApproach();
        }
        else
        {
            GoNeutral();
            FindObjectOfType<HudFunctions>().ActivateHud();
        }
    }

    private void CultistApproach()
    {
        StartCoroutine(CultistApproachCo());
    }

    private IEnumerator CultistApproachCo()
    {
        yield return new WaitForSeconds(2f);
        FindObjectOfType<CameraFollow>().Setup(() => cultist.transform.position);
        FindObjectOfType<MapManager>().OccupiedFields.Remove(cultist.CurrentGridPosition);
        FindObjectOfType<MapManager>().MoveEnemy(cultist, FindObjectOfType<MapManager>().FindAPath(cultist.CurrentGridPosition, walkDestination));
        FindObjectOfType<Player>().State = PlayerState.Neutral;
        trigger.TriggerDialogue();
    }

    public void StartCultistFight()
    {
        FindObjectOfType<HudFunctions>().ActivateHud();
        StartCoroutine(StartEncounter());
    }

    private IEnumerator StartEncounter()
    {
        yield return new WaitUntil(() => FindObjectOfType<Player>().State == PlayerState.Neutral);
        encounter.StartEncounter();
    }
}
