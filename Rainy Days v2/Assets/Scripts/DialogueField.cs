using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueField : MonoBehaviour
{
    private MapManager mapManager;
    private DialogueTrigger trigger;
    public Vector3Int myGridPosition { get; set; }

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
        myGridPosition = new Vector3Int(myGridPos.x, myGridPos.y, 0);
    }

    public void StartDialogue()
    {
        mapManager.MovePlayerTodialogue(myGridPosition, trigger);
    }

}
