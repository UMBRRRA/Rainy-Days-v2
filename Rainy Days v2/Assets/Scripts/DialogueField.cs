using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueField : MonoBehaviour
{
    private MapManager mapManager;
    private DialogueTrigger trigger;

    void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        Vector3 myZ0transform = new(transform.position.x, transform.position.y, 0);
        Vector3Int myGridPos = mapManager.map.WorldToCell(myZ0transform);
        Vector3Int gridZ0 = new Vector3Int(myGridPos.x, myGridPos.y, 0);
        mapManager.DialogueFields.Add(gridZ0, this);
    }

    public void StartDialogue()
    {
        trigger.TriggerDialogue();
    }
}
