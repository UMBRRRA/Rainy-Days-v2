using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : MonoBehaviour
{
    public static int engineerQuestId = 3;
    public InventoryObject inventory;
    public ItemObject generator;
    public static int gotGeneratorDialougeId = 10;
    public static int engineerQuestXP = 500;
    private Shadow shadow;

    void Start()
    {
        if (inventory.HasItem(generator, 1))
        {
            GetComponent<DialogueTrigger>().ChangeDialogue(gotGeneratorDialougeId);
        }
    }

    public void ChangeQuestState(int id)
    {
        FindObjectOfType<MainCanvas>().Quests[engineerQuestId] = id;
    }

    public void RepairedPump()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.OccupiedFields.Remove(mapManager.map.WorldToCell(this.transform.position));
        FindObjectOfType<Player>().exp += engineerQuestXP;
        shadow = FindObjectOfType<Shadow>();
        shadow.CloseShadow();
        StartCoroutine(WaitForShadow());
    }

    private IEnumerator WaitForShadow()
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        FindObjectOfType<Player>().State = PlayerState.Neutral;
        FindObjectOfType<FifthLevel>().PumpRepaired();
        FindObjectOfType<HudFunctions>().ActivateHud();
        shadow.OpenShadow();
        Destroy(this.gameObject);
    }

}
