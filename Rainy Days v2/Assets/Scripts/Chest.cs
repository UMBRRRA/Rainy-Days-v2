using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private DialogueTrigger trigger;
    private Animator animator;
    private Item item;
    private DialogueManager dm;

    void Start()
    {
        animator = GetComponent<Animator>();
        trigger = GetComponent<DialogueTrigger>();
        item = GetComponent<Item>();
        trigger.dialogues[0].question = $"Inside the chest you found {item.amount} x {item.item.title}.";
        dm = FindObjectOfType<DialogueManager>();
        if (dm.DialoguesSave.ContainsKey(trigger.id))
        {
            animator.SetBool("chestOpen", true);
        }
    }

    public void OpenChest()
    {
        animator.SetBool("chestOpen", true);
    }
}
