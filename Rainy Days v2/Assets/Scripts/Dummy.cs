using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    public int dummyMaxHealth, dummyMaxAP, dummyInitiative;

    void Start()
    {
        Stats = new EnemyStats(this, dummyMaxHealth, dummyMaxAP, dummyInitiative);
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
}
