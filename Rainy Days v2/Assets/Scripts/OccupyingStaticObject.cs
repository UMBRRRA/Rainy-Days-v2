using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyingStaticObject : MonoBehaviour
{
    private MapManager mapManager;
    public List<Vector3Int> additionalFields = new();
    private Vector3Int gridZ0;

    void Start()
    {
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        Vector3 myZ0transform = new(transform.position.x, transform.position.y, 0);
        Vector3Int myGridPos = mapManager.map.WorldToCell(myZ0transform);
        gridZ0 = new Vector3Int(myGridPos.x, myGridPos.y, 0);
        mapManager.OccupiedFields.Add(gridZ0);
        if (additionalFields.Count != 0)
        {
            foreach (Vector3Int field in additionalFields)
            {
                mapManager.OccupiedFields.Add(new Vector3Int(field.x, field.y, 0));
            }
        }
    }

    public void FreeOccupation()
    {
        mapManager.OccupiedFields.Remove(gridZ0);
        if (additionalFields.Count != 0)
        {
            foreach (Vector3Int field in additionalFields)
            {
                mapManager.OccupiedFields.Remove(new Vector3Int(field.x, field.y, 0));
            }
        }
    }

}
