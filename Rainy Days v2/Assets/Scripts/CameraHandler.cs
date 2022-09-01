using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public CameraFollow cameraFollow;
    private Player player;
    public Vector3 cameraSpawnPos;

    void Start()
    {
        transform.position = new Vector3(cameraSpawnPos.x, cameraSpawnPos.y, transform.position.z);
        StartCoroutine(FindPlayer());
    }

    public IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        cameraFollow.Setup(() => player.transform.position);
    }
}
