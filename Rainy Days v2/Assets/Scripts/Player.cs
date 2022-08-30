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
    private PlayerState _state = PlayerState.Neutral;
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
    }


    void Update()
    {

    }
}
