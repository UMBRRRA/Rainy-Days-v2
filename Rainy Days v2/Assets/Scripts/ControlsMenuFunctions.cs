using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenuFunctions : MonoBehaviour
{
    public GameObject child;
    public bool fromPause = false;
    public bool fromMain = false;
    private Player player;


    public void ActivateControlsMenu()
    {
        if (fromPause)
            FindObjectOfType<HudFunctions>().DeactivateHud();
        child.SetActive(true);
    }

    public void DeactivateControlsMenu()
    {
        if (fromPause)
        {
            FindObjectOfType<HudFunctions>().ActivateHud();
            PlayerGoNeutral();
        }
        else if (fromMain)
        {
            FindObjectOfType<MainMenuFunctions>().ActivateButtons();
        }
        child.SetActive(false);
        fromPause = false;
        fromMain = false;
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
}
