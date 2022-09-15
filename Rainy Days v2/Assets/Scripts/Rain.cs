using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    private MapManager mapManager;
    public float randomTimeMin, randomTimeMax;
    public float rainHeight = 3f;
    public GameObject droplet;
    public float timeForDrop = 2f;
    public float animTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Raining());
    }

    private IEnumerator Raining()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        int firstX, lastX, firstY, lastY;
        firstX = mapManager.map.origin.x;
        firstY = mapManager.map.origin.y;
        lastX = firstX + mapManager.map.size.x;
        lastY = firstY + mapManager.map.size.y;
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                Vector3Int gridPos = new(x, y, 2); // what about z
                Vector3 worldPos = mapManager.map.CellToWorld(gridPos);
                Vector3 littleRandom = new(worldPos.x + Random.Range(-0.2f, 0.2f), worldPos.y, worldPos.z);
                StartCoroutine(GenerateDroplet(worldPos));

            }
        }
    }

    private IEnumerator GenerateDroplet(Vector3 worldPos)
    {
        yield return new WaitForSeconds(Random.Range(randomTimeMin, randomTimeMax));
        Vector3 rainPos = new(worldPos.x, worldPos.y + rainHeight, worldPos.z);
        GameObject instance = Instantiate(droplet, rainPos, Quaternion.identity);
        StartCoroutine(MoveToPosition(instance, worldPos, timeForDrop));
        StartCoroutine(WaitAndDestroy(instance, worldPos));
    }

    private IEnumerator MoveToPosition(GameObject instance, Vector3 position, float timeTo)
    {
        Vector3 currentPos = instance.transform.position;
        float ti = 0f;
        while (true)
        {
            ti += Time.deltaTime / timeTo;
            instance.transform.position = Vector3.Lerp(currentPos, position, ti);

            if (instance.transform.position == position)
            {
                instance.GetComponent<Animator>().SetBool("Hit", true);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitAndDestroy(GameObject instance, Vector3 worldPos)
    {
        yield return new WaitForSeconds(animTime + timeForDrop);
        Destroy(instance);
        StartCoroutine(GenerateDroplet(worldPos));
    }

}
