using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixthLevel : MonoBehaviour
{

    private Player player;
    public static float abit = 2f;
    public GameObject door;
    private DialogueTrigger doorTrigger;
    public InventoryObject inventory;
    public static int haveKeyId = 1;
    public ItemObject key;
    public Encounter encounter;
    private MainCanvas immortal;

    void Start()
    {
        GoNeutral();
        StartCoroutine(WaitForDoorLoad());
        StartCoroutine(FindImmortal());
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
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


    private IEnumerator WaitForDoorLoad()
    {
        yield return new WaitUntil(() => (doorTrigger = door.GetComponent<DialogueTrigger>()) != null);
        if (inventory.HasItem(key, 1))
        {
            doorTrigger.ChangeDialogue(haveKeyId);
        }
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

}
