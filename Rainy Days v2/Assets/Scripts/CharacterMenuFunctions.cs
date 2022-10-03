using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuFunctions : MonoBehaviour
{

    public GameObject child;
    private Player player;
    public Text levelText, expText, nextLevelText, healthText, toxicityText,
        apText, initiativeText, movementText, meleeDmgText, meleeApText,
        gunDmgText, gunApText, magSizeText, reloadApText, medStrText, medApText;

    void Awake()
    {
        UpdateValues();
    }

    public void UpdateValues()
    {
        StartCoroutine(UpdateValuesCo());
    }

    private IEnumerator UpdateValuesCo()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        levelText.text = $"{player.level}";
        expText.text = $"{player.exp}";
        int nextLevelValue = WhatIsNextLevelValue();
        if (nextLevelValue == 0)
            nextLevelText.text = "---";
        else
            nextLevelText.text = $"{nextLevelValue}";
        healthText.text = $"{player.Stats.MaxHealth}";
        toxicityText.text = $"{player.Stats.MaxToxicity}";
        apText.text = $"{player.Stats.MaxAP}";
        initiativeText.text = $"{player.Stats.Initiative}";
        int movementValue = Mathf.RoundToInt(player.Stats.Movement * 100);
        movementText.text = $"{movementValue}";
        meleeDmgText.text = $"{player.meleeMinDice + player.meleeModifier}-{player.meleeMaxDice + player.meleeModifier}";
        meleeApText.text = $"{player.meleeApCost}";
        gunDmgText.text = $"{player.gunMinDice + player.gunModifier}-{player.gunMaxDice + player.gunModifier}";
        gunApText.text = $"{player.gunApCost}";
        magSizeText.text = $"{player.Stats.MagazineSize}";
        reloadApText.text = $"{player.reloadApCost}";
        int medStr = Mathf.RoundToInt(player.potionStrength * 100);
        medStrText.text = $"{medStr}%";
        medApText.text = $"{player.potionApCost}";
    }

    private int WhatIsNextLevelValue()
    {
        return player.level switch
        {
            1 => player.secondLevel,
            2 => player.thirdLevel,
            3 => player.fourthLevel,
            4 => player.fifthLevel,
            5 => player.sixthLevel,
            6 => player.seventhLevel,
            7 => player.eightLevel,
            8 => player.ninthLevel,
            9 => player.tenthLevel,
            10 => player.eleventhLevel,
            11 => player.twelfthLevel,
            12 => 0,
            _ => 0,
        };
    }

    public void ActivateCharacterMenu()
    {
        FindObjectOfType<HudFunctions>().DeactivateHud();
        child.SetActive(true);
        UpdateValues();
    }

    public void DeactivateCharacterMenu()
    {
        FindObjectOfType<HudFunctions>().ActivateHud();
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

}
