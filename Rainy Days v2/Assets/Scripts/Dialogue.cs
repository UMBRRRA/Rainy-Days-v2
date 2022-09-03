using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    public int id;
    [TextArea(3, 10)]
    public string question;
    [TextArea(1, 10)]
    public string answer1, answer2, answer3;
    public UnityEvent event1, event2, event3;

}
