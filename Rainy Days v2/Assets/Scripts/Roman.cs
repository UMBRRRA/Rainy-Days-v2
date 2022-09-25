using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roman : MonoBehaviour
{
    public static int loversQuestId = 2;
    public static int notTalkedToNina = 1;
    public static int talkedToNina = 2;
    private Shadow shadow;
    public static int loversQuestXP = 500;

    public void StartEncounter()
    {
        Player player = FindObjectOfType<Player>();
        FindObjectOfType<CameraFollow>().Setup(() => player.transform.position);
        FindObjectOfType<HudFunctions>().ActivateHud();
        FindObjectOfType<FourthLevel>().StartEncounter();
        int questState = FindObjectOfType<MainCanvas>().Quests[loversQuestId];
        if (questState == 0)
            GetComponent<DialogueTrigger>().ChangeDialogue(notTalkedToNina);
        else
            GetComponent<DialogueTrigger>().ChangeDialogue(talkedToNina);
    }

    public void LoversQuestFinished()
    {
        FindObjectOfType<MainCanvas>().Quests[loversQuestId] = 2;
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.OccupiedFields.Remove(mapManager.map.WorldToCell(this.transform.position));
        FindObjectOfType<Player>().exp += loversQuestXP;
        shadow = FindObjectOfType<Shadow>();
        shadow.CloseShadow();
        StartCoroutine(WaitForShadow());
    }

    private IEnumerator WaitForShadow()
    {

        yield return new WaitUntil(() => shadow.doneAnimating);
        FindObjectOfType<Player>().State = PlayerState.Neutral;
        shadow.OpenShadow();
        Destroy(this.gameObject);
    }
}
