using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject child;

    public Text question, answer1, answer2, answer3;

    public UnityEvent event1, event2, event3;

    public float dialogueSpeed = 0.05f;

    public void ActivateDialogueWindow()
    {
        child.SetActive(true);
    }

    public void DeactivateDialogueWindow()
    {
        child.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        ActivateDialogueWindow();
        StopAllCoroutines();
        StartCoroutine(TypeDialogue(dialogue));
    }

    public void EndDialogue()
    {
        ClearDialogue();
        DeactivateDialogueWindow();
    }

    public void ClearDialogue()
    {
        question.text = "";
        answer1.text = "";
        answer2.text = "";
        answer3.text = "";
        event1 = null;
        event2 = null;
        event3 = null;
    }

    public void NextDialogue(Dialogue dialogue)
    {
        ClearDialogue();
        StopAllCoroutines();
        StartCoroutine(TypeDialogue(dialogue));
    }

    private IEnumerator TypeDialogue(Dialogue dialogue)
    {
        foreach (char letter in dialogue.question.ToCharArray())
        {
            question.text += letter;
            yield return new WaitForSeconds(dialogueSpeed);
        }
        answer1.text = "1. " + dialogue.answer1;
        event1 = dialogue.event1;
        if (dialogue.answer2 != "")
        {
            answer2.text = "2. " + dialogue.answer2;
        }
        event2 = dialogue.event2;
        if (dialogue.answer3 != "")
        {
            answer3.text = "3. " + dialogue.answer3;
        }
        event3 = dialogue.event3;
    }

    public void FireEvent1()
    {
        if (event1 != null)
        {
            event1.Invoke();
        }
    }

    public void FireEvent2()
    {
        if (event2 != null)
        {
            event2.Invoke();
        }
    }

    public void FireEvent3()
    {
        if (event3 != null)
        {
            event3.Invoke();
        }
    }

}