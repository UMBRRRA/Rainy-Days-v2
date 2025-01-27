using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    private MainCanvas immortal;
    public static int firstGhostId = 1;
    public NPC firstGhost;
    private Player player;
    public static float abit = 2f;
    private HudFunctions hud;



    void Start()
    {
        StartCoroutine(FindImmortal());
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (immortal.Quests[firstGhostId] == 0)
        {
            Instantiate(firstGhost.gameObject, firstGhost.spawnPosition, Quaternion.identity);
        }
        else
        {
            GoNeutral();
            StartCoroutine(FindHud());
        }

    }
    private IEnumerator FindHud()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        StartCoroutine(WaitABitAndHud());
    }

    private IEnumerator WaitABitAndHud()
    {
        yield return new WaitForSeconds(2f);
        hud.ActivateHud();
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
