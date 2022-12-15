using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixteenthLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;

    private void Start()
    {
        GoNeutral();
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
}
