using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private Managers singleton;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

}
