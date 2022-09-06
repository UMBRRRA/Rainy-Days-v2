using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private DialogueTrigger trigger;
    private Animator animator;
    private Item item;

    void Start()
    {
        animator = GetComponent<Animator>();
        trigger = GetComponent<DialogueTrigger>();
        item = GetComponent<Item>();
        trigger.dialogues[0].question = $"You found {item.item.title} inside.";
    }

    public void OpenChest()
    {
        animator.SetBool("chestOpen", true);
    }
}
