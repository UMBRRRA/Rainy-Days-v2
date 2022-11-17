using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Madman : MonoBehaviour
{
    public static int madmanQuestId = 5;
    public static int startDialogueId = 0;
    public static int firstTalkDialogueId = 6;
    public static int gotLaserDialogueId = 7;
    public InventoryObject inventory;
    public ItemObject laserSight;
    public static int gotLaserSightId = 10;
    public static int madmanQuestXP = 500;
    DialogueTrigger trigger;
    MainCanvas immortal;

    void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
        immortal = FindObjectOfType<MainCanvas>();
        if (inventory.HasItem(laserSight, 1) && immortal.Quests[madmanQuestId] == 1) // and quest id
        {
            trigger.ChangeDialogue(gotLaserDialogueId);
        }
    }

    public void FirstTalk()
    {
        if (inventory.HasItem(laserSight, 1))
        {
            trigger.ChangeDialogue(gotLaserDialogueId);
        }
        else
        {
            trigger.ChangeDialogue(firstTalkDialogueId);
        }
        immortal.Quests[madmanQuestId] = 1;
    }

    public void QuestCompleted()
    {
        FindObjectOfType<Player>().exp += madmanQuestXP;
        immortal.Quests[madmanQuestId] = 2;
    }

    public void LaserSight()
    {
        FindObjectOfType<CharacterMenuFunctions>().ShadowAccuracy();
    }
}
