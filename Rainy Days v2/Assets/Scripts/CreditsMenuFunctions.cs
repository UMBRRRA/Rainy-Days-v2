using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CreditsMenuFunctions : MonoBehaviour
{
    public GameObject child;
    private string[] creditsText = {"RAINY DAYS WAS MADE BY JAKUB AMBROZIAK",
                                    "AS AN ENGINEER DIPLOMA AT PJATK",
                                    "SPECIAL THANKS TO EPG TEAM",
                                    "PROGRAMMING, DESIGN, GRAPHICS AND MUSIC BY JAKUB AMBROZIAK",
                                    "TO ALL THE FEARLESS DREAMERS"};

    public GameObject creditLinePrefab;
    private List<GameObject> creditLines = new();
    public float spawnTime;


    public void ActivateCreditsMenu()
    {
        FindObjectOfType<MainMenuFunctions>().DeactivateButtons();
        child.SetActive(true);
        RollCredits();
    }

    public void DeactivateCreditsMenu()
    {
        UnrollCredits();
        child.SetActive(false);
        FindObjectOfType<MainMenuFunctions>().ActivateButtons();
    }

    private void RollCredits()
    {
        StartCoroutine(SpawnCreditLine(0));
    }

    private IEnumerator SpawnCreditLine(int index)
    {
        GameObject line = Instantiate(creditLinePrefab, this.transform);
        line.GetComponent<Text>().text = creditsText[index];
        creditLines.Add(line);
        yield return new WaitForSeconds(spawnTime);
        index++;
        if (index < creditsText.Length)
            StartCoroutine(SpawnCreditLine(index));
    }

    private void UnrollCredits()
    {
        StopAllCoroutines();
        creditLines.ForEach(cl => Destroy(cl));
        creditLines.Clear();
    }
}
