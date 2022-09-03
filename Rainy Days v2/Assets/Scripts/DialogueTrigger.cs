using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public int id;
    public Dialogue[] dialogues;
    public int spawnDialogue;
    private int currentDialogue;
    private DialogueManager dialogueManager;
    private Player player;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        currentDialogue = spawnDialogue;
    }

    public void TriggerDialogue()
    {
        StartCoroutine(FindPlayer());
    }

    public IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        StartCoroutine(WaitForNeutral());
    }

    public IEnumerator WaitForNeutral()
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        dialogueManager.StartDialogue(dialogues[currentDialogue]);
        player.State = PlayerState.Dialogue;
    }

    public void EndDialogue()
    {
        currentDialogue = spawnDialogue;
        dialogueManager.EndDialogue();
        player.State = PlayerState.Neutral;
    }

    public void NextDialogue()
    {
        currentDialogue++;
        dialogueManager.NextDialogue(dialogues[currentDialogue]);
    }

    public void NextDialogue(int id)
    {
        currentDialogue = id;
        dialogueManager.NextDialogue(dialogues[currentDialogue]);
    }

    public void ChangeDialogue(int id)
    {
        spawnDialogue = id;
        currentDialogue = spawnDialogue;
    }

}
