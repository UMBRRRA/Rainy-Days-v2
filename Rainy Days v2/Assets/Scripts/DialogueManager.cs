using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DialogueState
{
    Typing, Ready
}

public class DialogueManager : MonoBehaviour
{
    private DialogueState State { get; set; } = DialogueState.Typing;

    public GameObject child;

    public Text question, answer1, answer2, answer3;

    public GameObject button1, button2, button3;

    public UnityEvent event1, event2, event3;

    public float dialogueSpeed = 0.05f;

    public Dictionary<int, int> DialoguesSave { get; set; } = new();

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
        State = DialogueState.Typing;
        foreach (char letter in dialogue.question.ToCharArray())
        {
            question.text += letter;
            yield return new WaitForSeconds(dialogueSpeed);
        }
        button1.SetActive(true);
        answer1.text = "1. " + dialogue.answer1;
        event1 = dialogue.event1;
        if (dialogue.answer2 != "")
        {
            button2.SetActive(true);
            answer2.text = "2. " + dialogue.answer2;
        }
        event2 = dialogue.event2;
        if (dialogue.answer3 != "")
        {
            button3.SetActive(true);
            answer3.text = "3. " + dialogue.answer3;
        }
        event3 = dialogue.event3;
        State = DialogueState.Ready;
    }

    public void FireEvent1()
    {
        if (event1 != null)
        {
            event1.Invoke();
            State = DialogueState.Typing;
            button1.SetActive(false);
            button2.SetActive(false);
            button3.SetActive(false);
        }
    }

    public void FireEvent2()
    {
        if (event2 != null)
        {
            event2.Invoke();
            State = DialogueState.Typing;
            button1.SetActive(false);
            button2.SetActive(false);
            button3.SetActive(false);
        }
    }

    public void FireEvent3()
    {
        if (event3 != null)
        {
            event3.Invoke();
            State = DialogueState.Typing;
            button1.SetActive(false);
            button2.SetActive(false);
            button3.SetActive(false);
        }
    }

    private void Update()
    {
        if (State == DialogueState.Ready)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                FireEvent1();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                FireEvent2();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                FireEvent3();
            }
        }

    }

}
