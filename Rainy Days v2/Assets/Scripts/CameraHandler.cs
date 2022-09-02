using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public CameraFollow CameraFollow { get; set; }
    public Player Player { get; set; }
    public Vector3 cameraSpawnPos;

    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    public IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (Player = FindObjectOfType<Player>()) != null);
        StartCoroutine(FindCamera());
    }

    public IEnumerator FindCamera()
    {
        yield return new WaitUntil(() => (CameraFollow = FindObjectOfType<CameraFollow>()) != null);
        CameraFollow.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, CameraFollow.transform.position.z);
        CameraFollow.Setup(() => Player.transform.position);
    }
}
