using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    public int dummyMaxHealth, dummyMaxAP, dummyInitiative;
    private Player player;
    private MapManager mapManager;

    void Start()
    {
        Stats = new EnemyStats(this, dummyMaxHealth, dummyMaxAP, dummyInitiative, 1f);
        StartCoroutine(FindPlayer());
        StartCoroutine(FindMapManager());
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
        Debug.Log($"My grid pos is {CurrentGridPosition}");
    }

    void Update()
    {

    }

    public override void MakeTurn()
    {
        StartCoroutine(DummyTurn());
    }

    public IEnumerator DummyTurn()
    {
        Debug.Log($"I am dummy and its my turn, my initative is {Stats.Initiative}");
        FindObjectOfType<CameraFollow>().Setup(() => this.transform.position);
        yield return new WaitForSeconds(3f);
        FindObjectOfType<EncounterManager>().NextTurn();
    }

    private void OnMouseDown()
    {
        if (player.State == PlayerState.Shooting)
        {
            ShootMe();
        }
        else if (player.State == PlayerState.Meleeing)
        {
            MeleeMe();
        }
    }

    public override void ShootMe()
    {
        player.Shoot(this);
    }

    public override void MeleeMe()
    {
        player.Melee(this);
    }

    public override void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public override void SetupSlider()
    {
        throw new System.NotImplementedException();
    }

    public override void ActivateSlider()
    {
        throw new System.NotImplementedException();
    }

    public override void DeactivateSlider()
    {
        throw new System.NotImplementedException();
    }
}
