using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public int encounterId;
    public int firstX, lastX, firstY, lastY;
    private MapManager mapManager;
    private Player player;
    private EncounterManager encounterManager;
    private MainCanvas imortal;
    public List<Enemy> enemies;
    private DialogueTrigger dialogue;
    private Item item;

    void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitUntil(() => (imortal = FindObjectOfType<MainCanvas>()) != null);
        if (imortal.FinishedEncounters.ContainsKey(encounterId))
        {
            enemies.ForEach(e => Destroy(e.gameObject));
        }
        else
        {
            item = GetComponent<Item>();
            dialogue = GetComponent<DialogueTrigger>();
            dialogue.dialogues[0].question = $"Among the remains you found {item.amount} x {item.item.title}.";
            StartCoroutine(FindMap());
        }
    }

    public IEnumerator FindMap()
    {
        yield return new WaitUntil(() => (mapManager = FindObjectOfType<MapManager>()) != null);
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                mapManager.EncounterFields.Add(new Vector3Int(x, y, 0), this);
            }
        }
    }



    public void StartEncounter()
    {
        Debug.Log("Starting Encounter");
        StartCoroutine(FindPlayer());
    }

    public IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.inFight = true;
        StartCoroutine(FindEncounterManager());
    }


    public IEnumerator FindEncounterManager()
    {
        yield return new WaitUntil(() => (encounterManager = FindObjectOfType<EncounterManager>()) != null);
        FindObjectOfType<AudioManager>().EncounterSound();
        player.State = PlayerState.NotMyTurn;
        int playerRoll = Random.Range(1, 21);
        player.Stats.RolledInitiative = player.Stats.Initiative + playerRoll;
        FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.General,
            $"Initiative: {player.Stats.RolledInitiative}", player.transform.position);
        List<Stats> stats = new();
        stats.Add(player.Stats);
        enemies.ForEach(e =>
        {
            int enemyRoll = Random.Range(1, 21);
            e.Stats.RolledInitiative = e.Stats.Initiative + enemyRoll;
            FindObjectOfType<CombatInfoManager>().GenerateCombatInfo(CombatInfoType.General,
                $"Initiative: {e.Stats.RolledInitiative}", e.transform.position);
            stats.Add(e.Stats);
        });
        encounterManager.StartEncounter(stats, this);
    }

    public void EndEncounter()
    {
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                mapManager.EncounterFields.Remove(new Vector3Int(x, y, 0));
            }
        }

        imortal.FinishedEncounters.Add(encounterId, this);
        StartCoroutine(ItemsAdd());
    }

    private IEnumerator ItemsAdd()
    {
        yield return new WaitUntil(() => player.State == PlayerState.Neutral);
        dialogue.TriggerDialogue();
    }

}
