using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{

    public int id;

    public string question;

    public Answer answer1, answer2, answer3;

}
