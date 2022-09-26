using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FifthLevel : MonoBehaviour
{
    private Player player;
    public static float abit = 2f;
    public Vector3 engineerPosition;
    public GameObject engineer;
    public static int engineerQuestId = 3;
    public GameObject cave;
    public GameObject machine;
    public GameObject transition;

    void Start()
    {
        GoNeutral();
        int engineerQuestState = FindObjectOfType<MainCanvas>().Quests[engineerQuestId];
        if (engineerQuestState == 0 || engineerQuestState == 1)
        {
            GameObject realEngineer = Instantiate(engineer, engineerPosition, Quaternion.identity);
        }
        else
        {
            PumpRepaired();
        }
    }

    public void GoNeutral()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        StartCoroutine(WaitABit());
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(abit);
        player.State = PlayerState.Neutral;
    }

    public void SetCaveAnimator(bool param)
    {
        cave.GetComponent<Animator>().SetBool("PumpRepaired", param);
    }

    public void SetMachineAnimator(bool param)
    {
        machine.GetComponent<Animator>().SetBool("Working", param);
    }

    public void PumpRepaired()
    {
        SetCaveAnimator(true);
        SetMachineAnimator(true);
        transition.SetActive(true);
    }

}
