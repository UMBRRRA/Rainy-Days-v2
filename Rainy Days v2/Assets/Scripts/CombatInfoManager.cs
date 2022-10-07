using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CombatInfoType
{
    Health, Toxicity, Ap, Level, General
}

public class CombatInfoManager : MonoBehaviour
{
    public Color healthDamageColor;
    public Color toxicityDamageColor;
    public Color apCostColor;
    public Color levelUpColor;
    public Color generalColor;

    public GameObject prefab;
    public Vector3 offset, target;

    public float destroyTime = 2f;
    public float moveStepTime = 0.05f;
    public float moveStepYValue = 0.01f;
    public float delayHealth, delayToxic, delayAp, delayLevel, delayGeneral;
    public float yHealth, yToxic, yAp, yLevel, yGeneral;
    public float xRandomMin, xRandomMax;

    public void GenerateCombatInfo(CombatInfoType type, string text, Vector3 position)
    {

        if (type == CombatInfoType.Health)
        {
            StartCoroutine(WaitAndSpawn(type, text, position, delayHealth, new(offset.x + Random.Range(xRandomMin, xRandomMax), offset.y + yHealth)));
        }
        else if (type == CombatInfoType.Toxicity)
        {
            StartCoroutine(WaitAndSpawn(type, text, position, delayToxic, new(offset.x + Random.Range(xRandomMin, xRandomMax), offset.y + yToxic)));
        }
        else if (type == CombatInfoType.Ap)
        {
            StartCoroutine(WaitAndSpawn(type, text, position, delayAp, new(offset.x + Random.Range(xRandomMin, xRandomMax), offset.y + yAp)));
        }
        else if (type == CombatInfoType.Level)
        {
            StartCoroutine(WaitAndSpawn(type, text, position, delayLevel, new(offset.x + Random.Range(xRandomMin, xRandomMax), offset.y + yLevel)));
        }
        else
        {
            StartCoroutine(WaitAndSpawn(type, text, position, delayGeneral, new(offset.x + Random.Range(xRandomMin, xRandomMax), offset.y + yGeneral)));
        }
    }

    private IEnumerator WaitAndSpawn(CombatInfoType type, string text, Vector3 position, float delay, Vector3 changedOffset)
    {
        yield return new WaitForSeconds(delay);
        GameObject info = Instantiate(prefab, position + changedOffset, Quaternion.identity);
        Text infoText = info.GetComponentInChildren<Text>();
        infoText.text = text;
        if (type == CombatInfoType.Health)
            infoText.color = healthDamageColor;
        else if (type == CombatInfoType.Toxicity)
            infoText.color = toxicityDamageColor;
        else if (type == CombatInfoType.Ap)
            infoText.color = apCostColor;
        else if (type == CombatInfoType.Level)
            infoText.color = levelUpColor;
        else
            infoText.color = generalColor;
        StartCoroutine(MoveUp(info));
        StartCoroutine(WaitAndDestroy(info));
    }

    private IEnumerator MoveUp(GameObject info)
    {
        while (true)
        {
            if (info != null)
            {
                info.transform.position = new Vector3(info.transform.position.x, info.transform.position.y + moveStepYValue);
                yield return new WaitForSeconds(moveStepTime);
            }
            else
                break;
        }
    }

    private IEnumerator WaitAndDestroy(GameObject info)
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(info);
    }
}
