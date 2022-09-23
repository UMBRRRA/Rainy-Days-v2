using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Ghost,
    Normal
}

public enum NPCState
{
    Neutral,
    Moving
}

public class NPC : MonoBehaviour
{
    public NPCType type;
    private Animator animator;
    private Player player;
    private MapManager mapManager;
    private DialogueTrigger dialogue;
    private MainCanvas immortal;
    private Shadow shadow;
    public static int firstGhostId = 1;
    public float timeForMove;
    public float timeForCornerMove;
    public float npcZ = 1.2f;
    public int spawnDirection;
    public Vector3 spawnPosition;
    public int CurrentDirection { get; set; }
    public Vector3Int CurrentGridPosition { get; set; }
    public Vector3Int ghostWalkDestination;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dialogue = GetComponent<DialogueTrigger>();
        IdleDirection(spawnDirection);
        StartCoroutine(FindPlayer());
        StartCoroutine(FindMapManager());
        StartCoroutine(FindShadow());
        StartCoroutine(FindImmortal());
    }




    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);

    }

    private IEnumerator FindShadow()
    {
        yield return new WaitUntil(() => (shadow = FindObjectOfType<Shadow>()) != null);
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
    }
    private IEnumerator FindMapManager()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        Vector3 myZ0transform = new(transform.position.x, transform.position.y, 0);
        Vector3Int myGridPos = mapManager.map.WorldToCell(myZ0transform);
        CurrentGridPosition = new Vector3Int(myGridPos.x, myGridPos.y, 0);
        if (type == NPCType.Normal)
        {
            mapManager.OccupiedFields.Add(CurrentGridPosition);
        }
        else if (type == NPCType.Ghost)
        {
            GhostEvent();
        }
    }

    public void GhostEvent()
    {
        StartCoroutine(GhostEventCo());
    }

    public void GhostDisappear()
    {
        shadow.CloseShadow();
        StartCoroutine(GhostOpenShadow());
    }

    private IEnumerator GhostOpenShadow()
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        FindObjectOfType<CameraFollow>().Setup(() => player.transform.position);
        shadow.OpenShadow();
        immortal.Quests[firstGhostId] = 1;
        player.State = PlayerState.Neutral;
        Destroy(this.gameObject);
    }

    private IEnumerator GhostEventCo()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
        mapManager.MoveNPC(this, mapManager.FindAPath(CurrentGridPosition, ghostWalkDestination));
        player.State = PlayerState.Neutral;
        dialogue.TriggerDialogue();
    }

    public IEnumerator MoveToPosition(Vector3 position, float timeTo)
    {
        animator.SetInteger("IdleDirection", 0);
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", true);
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

    public IEnumerator PlayIdle(Vector3Int idleDir, Vector3 nextField)
    {
        Vector3 realNF = new(nextField.x, nextField.y + 0.25f, npcZ);
        yield return new WaitUntil(() => transform.position == realNF);
        animator.SetInteger("WalkDirection", 0);
        animator.SetBool("Walk", false);
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


    }
    private void IdleDirection(int dir)
    {
        animator.SetInteger("IdleDirection", dir);
        CurrentDirection = dir;
    }
}
