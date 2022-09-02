using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShadowOnStart : MonoBehaviour
{
    private Shadow shadow;

    void Start()
    {
        StartCoroutine(FindShadow());
    }

    private IEnumerator FindShadow()
    {
        yield return new WaitUntil(() => (shadow = FindObjectOfType<Shadow>()) != null);
        shadow.OpenShadow();
    }
}
