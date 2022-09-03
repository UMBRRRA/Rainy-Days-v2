using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour
{
    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
    }



}
