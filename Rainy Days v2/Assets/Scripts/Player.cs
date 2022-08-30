using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Neutral
}

public class Player : MonoBehaviour
{
    public Vector3Int spawnGridPosition;
    public Vector3Int currentGridPosition;
    public MapManager mapManager;
    public float walkDelayTime;
    //private bool walkN, walkNE, walkE, walkES, walkS, walkSW, walkW, walkNW;
    private bool walk = false;
    public float speed = 1000000f;
    public Vector3 nextField { get; set; }
    public Vector3 currentField { get; set; }
    private PlayerState _state = PlayerState.Neutral;
    public float startTime { get; set; }
    public PlayerState State
    {
        get
        {
            return _state;
        }
        set
        {
            Debug.Log($"Changing player state from {_state} to {value}");
            _state = value;
        }
    }


    void Start()
    {
        currentGridPosition = spawnGridPosition;
        currentField = transform.position;
        nextField = transform.position;
        startTime = Time.time;
    }


    void Update()
    {
        if (transform.position != nextField && State == PlayerState.Neutral)
        {

            float distCovered = (Time.time - startTime) * speed * Time.deltaTime;
            float fractionOfJourney = distCovered / Vector3.Distance(transform.position, nextField);
            transform.position = Vector3.Lerp(transform.position, nextField, fractionOfJourney);

            //Vector3 target = new(nextField.x, nextField.y, 1);
            //transform.position = Vector3.MoveTowards(transform.position, nextField, 0.001f);
        }
        //Debug.Log(nextField);
    }

    public IEnumerator WalkPath(APath path)
    {
        Field[] fields = path.Fields.ToArray();
        for (int i = 1; i < fields.Length; i++)
        {
            //Debug.Log(fields[i]);
            startTime = Time.time;
            Vector3 cf = FindObjectOfType<MapManager>().map.CellToWorld(fields[i - 1].GridPosition);
            currentField = new(cf.x, cf.y, 1);
            Vector3 nf = FindObjectOfType<MapManager>().map.CellToWorld(fields[i].GridPosition);
            nextField = new(nf.x, nf.y + 0.25f, 1);
            //ChangePos(nextField);
            //walk = true;
            Debug.Log(nextField);
            yield return new WaitForSeconds(1f);
        }
        //walk = false;
    }

    private void ChangePos(Vector3 pos)
    {
        nextField = new Vector3(pos.x, pos.y, 1);
    }

}
