using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuFunctions : MonoBehaviour
{
    public GameObject child;
    public MainMenuFunctions mainMenu;
    private Player player;

    public void ActivatePauseMenu()
    {
        child.SetActive(true);
    }

    public void DeactivatePauseMenu()
    {
        child.SetActive(false);
    }

    public void PlayerGoNeutral()
    {
        StartCoroutine(PlayerNeutral());
    }

    public IEnumerator PlayerNeutral()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.State = PlayerState.Neutral;
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(DestroyPlayer());
        child.SetActive(false);
        mainMenu.child.SetActive(true);
        SceneManager.LoadScene(0);

    }

    public IEnumerator DestroyPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if (player != null)
            Destroy(player.gameObject);
    }


}
