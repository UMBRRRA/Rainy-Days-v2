using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHudOnStart : MonoBehaviour
{
    private HudFunctions hud;

    void Start()
    {
        StartCoroutine(FindHud());
    }

    private IEnumerator FindHud()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        StartCoroutine(WaitABit());
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(2f);
        hud.ActivateHud();
    }

}
