using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{

    public GameObject child;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ActivateMainMenu()
    {
        child.SetActive(true);
    }

    public void DeactivateMainMenu()
    {
        child.SetActive(false);
    }

}
