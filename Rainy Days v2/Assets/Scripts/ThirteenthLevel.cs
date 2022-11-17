using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirteenthLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;
    private MainCanvas immortal;

    void Start()
    {
        GoNeutral();
        StartCoroutine(FindImmortal());
    }



    public void GoNeutral()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        StartCoroutine(WaitABit());
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(abit);
        player.State = PlayerState.Neutral;
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
    }

}
