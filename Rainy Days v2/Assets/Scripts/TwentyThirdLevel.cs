using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwentyThirdLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;
    public DialogueTrigger startFightTrigger;
    public Vector3Int startFightWalk;
    public GameObject boss;

    void Start()
    {
        GoNeutral();
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
        ApproachFight();
    }

    public void ApproachFight()
    {
        FindObjectOfType<MapManager>().WhenClicked(startFightWalk);
        StartCoroutine(Walk());
    }

    private IEnumerator Walk()
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        player.IdleDirectionNoNeutral(3);
        player.State = PlayerState.Neutral;
        FindObjectOfType<CameraFollow>().Setup(() => boss.transform.position);
        startFightTrigger.TriggerDialogue();
    }

}
