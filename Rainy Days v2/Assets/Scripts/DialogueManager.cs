using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject child;

    public void ActivateDialogueWindow()
    {
        child.SetActive(true);
    }

    public void DeactivateDialogueWindow()
    {
        child.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
