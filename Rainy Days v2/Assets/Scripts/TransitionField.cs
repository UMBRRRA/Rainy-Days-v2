using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionField : MonoBehaviour
{
    private MapManager mapManager;
    public int level;
    public Vector3Int spawnPos;
    public int spawnDir;
    private Player player;
    private Shadow shadow;

    void Start()
    {
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        Vector3 myZ0transform = new(transform.position.x, transform.position.y, 0);
        Vector3Int myGridPos = mapManager.map.WorldToCell(myZ0transform);
        Vector3Int gridZ0 = new Vector3Int(myGridPos.x, myGridPos.y, 0);
        mapManager.TransitionFields.Add(gridZ0, this);
    }

    public void MakeTransition()
    {
        StartCoroutine(WaitForPlayer());
    }

    public IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        shadow = FindObjectOfType<Shadow>();
        shadow.CloseShadow();
        FindObjectOfType<AudioManager>().TransitionSound();
        FindObjectOfType<HudFunctions>().DeactivateHud();
        FindObjectOfType<AudioManager>().ChangeLevel(level);
        StartCoroutine(WaitForShadow(shadow));
    }

    public IEnumerator WaitForShadow(Shadow shadow)
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        player.spawnGridPosition = spawnPos;
        player.spawnDirection = spawnDir;
        player.ChangeRoom();
        SceneManager.LoadScene(level);
    }
}
