using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharacterMenuFunctions : MonoBehaviour
{
    public int towerMaxHealthBonus, towerMaxToxicBonus;
    public int hunterBonus = 1;
    public int recklessIniBonus = 2;
    public float recklessMoveBonus = 0.03f;
    public float alchemMedStr = 0.45f;
    public int alchemMedApReduc = 1;
    public int eternalMaxHealthBonus = 20;
    public int eternalMaxToxicBonus = 30;
    public int gunnerDmgBonus = 2;
    public int gunnerApReduc = 1;
    public int gunnerReloadApReduc = 1;
    public int execDmgBonus = 2;
    public int execApReduc = 1;
    public int psychoMaxApBonus = 5;
    public int laserSightBonus = 1;
    public int extendedMagBonus = 2;

    public GameObject child;
    private Player player;
    public Text levelText, expText, nextLevelText, healthText, toxicityText,
        apText, initiativeText, movementText, meleeDmgText, meleeApText,
        gunDmgText, gunApText, magSizeText, reloadApText, medStrText, medApText,
        ususedPointsText;

    public FeatSlot[] slots;

    public List<FeatObject> availableFeats;
    public List<FeatObject> unlockedFeats;
    public FeatObject tower, alchemist, eternal, hunter, snipe,
        gunner, flurry, executioner, recklessness, haste, psychopath;

    public Text descriptionTitle, description, prereqTitle, prereq;
    public GameObject unlockButton;

    private FeatObject currentFeat;
    private HudFunctions hud;

    public FeatObject shadowAccuracy;
    public FeatObject extendedMag;
    public FeatObject blessing;

    public void RestartGame()
    {
        availableFeats = new();
        unlockedFeats = new();
        availableFeats.Add(tower);
        availableFeats.Add(hunter);
        availableFeats.Add(recklessness);
        availableFeats.Add(alchemist);
        availableFeats.Add(snipe);
        availableFeats.Add(haste);
        availableFeats.Add(flurry);
        availableFeats.Add(eternal);
        availableFeats.Add(gunner);
        availableFeats.Add(executioner);
        availableFeats.Add(psychopath);
    }

    public void SetDetails(FeatSlot featSlot)
    {
        unlockButton.SetActive(false);
        descriptionTitle.text = featSlot.currentFeat.title;
        description.text = MakeDescription(featSlot.currentFeat);
        descriptionTitle.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        prereqTitle.gameObject.SetActive(true);
        prereq.gameObject.SetActive(true);

        if (featSlot.currentFeat.prerequisites.Count == 0)
            prereq.text = "None";
        else
        {
            prereq.text = "";
            FeatObject[] pre = featSlot.currentFeat.prerequisites.ToArray();
            for (int i = 0; i < pre.Length; i++)
            {
                if (i == pre.Length - 1)
                    prereq.text += pre[i].title;
                else
                    prereq.text += $"{pre[i].title}, ";
            }
        }

        if (featSlot.type == FeatSlotType.Available)
        {
            currentFeat = featSlot.currentFeat;
            unlockButton.SetActive(true);
        }
    }

    private string MakeDescription(FeatObject feat)
    {
        if (feat == tower)
            return $"Traversing the dangers of caustic rain yielded you an unprecedented resistance. " +
                $"Maximum health is increased by {towerMaxHealthBonus} and maximum toxicity by {towerMaxToxicBonus}.";
        else if (feat == hunter)
            return $"The hunter becomes the hunted. With every kill your lethality increases. " +
                $"Your gun and melee damage gets a +{hunterBonus} bonus.";
        else if (feat == recklessness)
            return $"Setting forth on a suicidal quest slowly breaks your mentality and unlocks your primal fears. " +
                $"Movement speed increases by {Mathf.RoundToInt(recklessMoveBonus * 100)} and initiative increases by {recklessIniBonus}.";
        else if (feat == snipe)
            return $"Gain access to Snipe special ability. Shoot opponents in their heads with a critical damage bonus. " +
                $"Has {player.snipeCooldown} turns cooldown.";
        else if (feat == alchemist)
            return $"You become one with the medicine you take. Health potions and antidotes strength is increased to {Mathf.RoundToInt(alchemMedStr * 100)}% " +
                $"and their usage costs {alchemMedApReduc} less.";
        else if (feat == haste)
            return $"Gain access to Haste special ability. " +
                $"Break the limits of your body and surge with energy refunding you {Mathf.RoundToInt(player.hasteApRefundPercentage * 100)}% action points. " +
                $"Has {player.hasteCooldown} turns cooldown.";
        else if (feat == flurry)
            return $"Gain access to Flurry special ability. " +
                $"Lash at opponents with flurry of 3 melee swings that costs {Mathf.RoundToInt(player.flurryApCostModifier * 100)}% standard melee action points. " +
                $"Has {player.flurryCooldown} turns cooldown.";
        else if (feat == eternal)
            return $"At this point you feel like nothing can best you. Your destined enemy draws closer. " +
                $"Maximum health is increased by {eternalMaxHealthBonus} and maximum toxicity by {eternalMaxToxicBonus}.";
        else if (feat == gunner)
            return $"Countless fights using your firearm yielded you a complete mastery of guns. Gun damage is increased by {gunnerDmgBonus}, " +
                $"gun action point cost is reduced by {gunnerApReduc} and reload action point cost is reduced by {gunnerReloadApReduc}.";
        else if (feat == executioner)
            return $"After slaying hordes of monsters you developed a true mastery of your blade. Melee damage is increased by {execDmgBonus} " +
                $"and melee action point cost is reduced by {execApReduc}.";
        else if (feat == psychopath)
            return $"Disregarding the dangers to your body and breaking your limits has made you utterly mad. " +
                $"Your maximum action points is increased by {psychoMaxApBonus}.";
        else if (feat == shadowAccuracy)
            return "With a laser sight attached to your gun you're able to see beyond the dark. The range of your gun increases.";
        else if (feat == extendedMag)
            return $"This fancy extended magazine you found enables you to store {extendedMagBonus} more bullets in your gun.";
        else if (feat == blessing)
            return $"With their last breath, the mysterious projection bestowed a blessing on you.";
        else
            return "";
    }

    public void UpdateValues()
    {
        currentFeat = null;
        unlockButton.SetActive(false);
        descriptionTitle.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        prereq.gameObject.SetActive(false);
        prereqTitle.gameObject.SetActive(false);
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
        healthText.text = $"{player.Stats.CurrentHealth}/{player.Stats.MaxHealth}";
        toxicityText.text = $"{player.Stats.CurrentToxicity}/{player.Stats.MaxToxicity}";
        apText.text = $"{player.Stats.CurrentAP}/{player.Stats.MaxAP}";
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
        ususedPointsText.text = $"{player.unusedFeats}";

        int pointer = 0;
        foreach (FeatObject f in unlockedFeats)
            FillSlot(slots[pointer++], f, FeatSlotType.Aquired);

        foreach (FeatObject f in availableFeats)
        {
            if (f.prerequisites.Count == 0 || f.prerequisites.All(p => unlockedFeats.Contains(p)))
            {
                if (player.unusedFeats > 0)
                    FillSlot(slots[pointer++], f, FeatSlotType.Available);
                else
                    FillSlot(slots[pointer++], f, FeatSlotType.Unavailable);
            }
        }

    }

    private void FillSlot(FeatSlot slot, FeatObject feat, FeatSlotType type)
    {
        slot.currentFeat = feat;
        slot.type = type;
        slot.SetColor();
        slot.text.text = feat.title;
        slot.gameObject.SetActive(true);
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
        slots.ToList().ForEach(s => s.gameObject.SetActive(false));
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

    public void UnlockFeat()
    {
        if (!player.inFight && player.unusedFeats > 0)
        {
            player.unusedFeats -= 1;
            if (currentFeat == tower)
            {
                player.Stats.MaxHealth += towerMaxHealthBonus;
                player.Stats.MaxToxicity += towerMaxToxicBonus;
                StartCoroutine(SetHudBars());
            }
            else if (currentFeat == hunter)
            {
                player.meleeModifier += hunterBonus;
                player.gunModifier += hunterBonus;
            }
            else if (currentFeat == recklessness)
            {
                player.Stats.Initiative += recklessIniBonus;
                player.Stats.Movement += recklessMoveBonus;
            }
            else if (currentFeat == alchemist)
            {
                player.potionStrength = alchemMedStr;
                player.potionApCost -= alchemMedApReduc;
            }
            else if (currentFeat == snipe)
            {
                StartCoroutine(ActivateSnipe());
            }
            else if (currentFeat == flurry)
            {
                StartCoroutine(ActivateFlurry());
            }
            else if (currentFeat == haste)
            {
                StartCoroutine(ActivateHaste());
            }
            else if (currentFeat == eternal)
            {
                player.Stats.MaxHealth += eternalMaxHealthBonus;
                player.Stats.MaxToxicity += eternalMaxToxicBonus;
                StartCoroutine(SetHudBars());
            }
            else if (currentFeat == gunner)
            {
                player.gunModifier += gunnerDmgBonus;
                player.gunApCost -= gunnerApReduc;
                player.reloadApCost -= gunnerReloadApReduc;
            }
            else if (currentFeat == executioner)
            {
                player.meleeModifier += execDmgBonus;
                player.meleeApCost -= execApReduc;
            }
            else if (currentFeat == psychopath)
            {
                player.Stats.MaxAP += psychoMaxApBonus;
                player.Stats.CurrentAP = player.Stats.MaxAP;
                StartCoroutine(SetHudBars());
            }
            unlockedFeats.Add(currentFeat);
            availableFeats.Remove(currentFeat);
            UpdateValues();
        }
    }

    private IEnumerator SetHudBars()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.SetMaxHealth(player.Stats.MaxHealth);
        hud.SetHealth(player.Stats.CurrentHealth);
        hud.SetMaxToxicity(player.Stats.MaxToxicity);
        hud.SetToxicity(player.Stats.CurrentToxicity);
        hud.SetMaxAp(player.Stats.MaxAP);
        hud.SetAp(player.Stats.CurrentAP);
    }

    private IEnumerator ActivateSnipe()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.ActivateSnipe();
    }
    private IEnumerator ActivateFlurry()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.ActivateFlurry();
    }
    private IEnumerator ActivateHaste()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.ActivateHaste();
    }

    public void ShadowAccuracy()
    {
        unlockedFeats.Add(shadowAccuracy);
        player = FindObjectOfType<Player>();
        player.gunRange += laserSightBonus;
    }

    public void ExtendedMag()
    {
        player.Stats.MagazineSize += extendedMagBonus;
        unlockedFeats.Add(extendedMag);
        StartCoroutine(UpdateMag());
    }

    private IEnumerator UpdateMag()
    {
        yield return new WaitUntil(() => (hud = FindObjectOfType<HudFunctions>()) != null);
        hud.UpdateMagazine();
    }

    public void GhostBless()
    {
        unlockedFeats.Add(blessing);
        player = FindObjectOfType<Player>();
        player.Stats.MaxHealth += 10;
        player.Stats.MaxToxicity += 10;
        player.Stats.MaxAP += 2;
        player.gunModifier += 1;
        player.meleeModifier += 1;
        StartCoroutine(SetHudBars());
    }
}
