using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    private MainCanvas immortal;
    public static int firstGhostId = 1;
    public NPC firstGhost;
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

    }

}
