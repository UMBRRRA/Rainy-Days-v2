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
        transform.position = new Vector3(cameraSpawnPos.x, cameraSpawnPos.y, transform.position.z);
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
        CameraFollow.Setup(() => Player.transform.position);
    }
}
