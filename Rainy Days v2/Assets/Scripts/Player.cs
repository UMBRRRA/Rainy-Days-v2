using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    Neutral,
    Moving,
    Pause,
    Dialogue
}

public class Player : MonoBehaviour
{
    private static Player singleton;

    public float playerZ = 1.2f;
    public Vector3Int spawnGridPosition;
    public Vector3Int currentGridPosition;
    public MapManager mapManager;
    public int spawnDirection;
    public int currentDirection;
    private Animator animator;
    public Vector3 NextField { get; set; }
    private PauseMenuFunctions pauseMenu;
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


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }



    void Start()
    {
        currentGridPosition = spawnGridPosition;
        NextField = transform.position;
        mapManager = FindObjectOfType<MapManager>();
        animator = GetComponent<Animator>();
        IdleDirection(spawnDirection);
    }

    public void ChangeRoom()
    {
        currentGridPosition = spawnGridPosition;
        Vector3 pos = mapManager.map.CellToWorld(spawnGridPosition);
        transform.position = new Vector3(pos.x, pos.y + 0.25f, playerZ);
        IdleDirection(spawnDirection);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && State == PlayerState.Neutral)
        {
            State = PlayerState.Pause;
            StartCoroutine(OpenPauseMenu());
        }
    }

    public IEnumerator OpenPauseMenu()
    {
        yield return new WaitUntil(() => (pauseMenu = FindObjectOfType<PauseMenuFunctions>()) != null);
        pauseMenu.ActivatePauseMenu();
    }

    public IEnumerator MoveToPosition(Vector3 position, float timeTo)
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);
        Vector3 currentPos = transform.position;
        float ti = 0f;
        while (true)
        {
            ti += Time.deltaTime / timeTo;
            transform.position = Vector3.Lerp(currentPos, position, ti);

            if (transform.position == position)
            {
                break;
            }
            yield return null;
        }
    }

    public IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nf)
    {
        Vector3 realNF = new(nf.x, nf.y + 0.25f, playerZ);
        yield return new WaitUntil(() => transform.position == realNF);
        animator.SetInteger("WalkDirection", 0);
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", true);
        if (idleDir.x == 0 && idleDir.y == -1)
        {
            IdleDirection(1);
        }
        else if (idleDir.x == 1 && idleDir.y == -1)
        {
            IdleDirection(2);
        }
        else if (idleDir.x == 1 && idleDir.y == 0)
        {
            IdleDirection(3);
        }
        else if (idleDir.x == 1 && idleDir.y == 1)
        {
            IdleDirection(4);
        }
        else if (idleDir.x == 0 && idleDir.y == 1)
        {
            IdleDirection(5);
        }
        else if (idleDir.x == -1 && idleDir.y == 1)
        {
            IdleDirection(6);
        }
        else if (idleDir.x == -1 && idleDir.y == 0)
        {
            IdleDirection(7);
        }
        else if (idleDir.x == -1 && idleDir.y == -1)
        {
            IdleDirection(8);
        }

        State = PlayerState.Neutral;
    }

    public void PlayWalk(Vector3Int walkDir)
    {
        if (walkDir.x == 0 && walkDir.y == -1)
        {
            animator.SetInteger("WalkDirection", 1);
        }
        else if (walkDir.x == 1 && walkDir.y == -1)
        {
            animator.SetInteger("WalkDirection", 2);
        }
        else if (walkDir.x == 1 && walkDir.y == 0)
        {
            animator.SetInteger("WalkDirection", 3);
        }
        else if (walkDir.x == 1 && walkDir.y == 1)
        {
            animator.SetInteger("WalkDirection", 4);
        }
        else if (walkDir.x == 0 && walkDir.y == 1)
        {
            animator.SetInteger("WalkDirection", 5);
        }
        else if (walkDir.x == -1 && walkDir.y == 1)
        {
            animator.SetInteger("WalkDirection", 6);
        }
        else if (walkDir.x == -1 && walkDir.y == 0)
        {
            animator.SetInteger("WalkDirection", 7);
        }
        else if (walkDir.x == -1 && walkDir.y == -1)
        {
            animator.SetInteger("WalkDirection", 8);
        }
    }

    private void IdleDirection(int dir)
    {
        animator.SetInteger("IdleDirection", dir);
        currentDirection = dir;
    }



}
