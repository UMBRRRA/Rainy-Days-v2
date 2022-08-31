using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyingStaticObject : MonoBehaviour
{
    private MapManager mapManager;

    void Start()
    {
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => FindObjectOfType<MapManager>() != null);
        mapManager = FindObjectOfType<MapManager>();
        Vector3Int myGridPos = mapManager.map.WorldToCell(transform.position);
        Vector3Int gridZ0 = new Vector3Int(myGridPos.x, myGridPos.y, 0);
        mapManager.OccupiedFields.Add(gridZ0);
    }

}
