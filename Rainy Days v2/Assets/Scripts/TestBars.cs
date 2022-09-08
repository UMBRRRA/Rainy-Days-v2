using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBars : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            FindObjectOfType<Player>().DamageHealth(10);
            FindObjectOfType<Player>().IncreaseToxicity(15);
        }
    }
}
