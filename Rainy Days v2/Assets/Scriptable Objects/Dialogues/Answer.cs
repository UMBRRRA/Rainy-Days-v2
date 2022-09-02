using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Answer : ScriptableObject
{
    [TextArea(3, 10)]
    public string text;

    public UnityEvent unityEvent;

}
