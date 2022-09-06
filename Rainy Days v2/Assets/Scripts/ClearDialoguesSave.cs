using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDialoguesSave : MonoBehaviour
{
    public DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager.DialoguesSave.Clear();
    }


}
