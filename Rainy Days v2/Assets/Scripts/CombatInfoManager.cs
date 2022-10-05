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

    public void GenerateCombatInfo(CombatInfoType type, string text, Vector3 position)
    {
        GameObject info = Instantiate(prefab, position + offset, Quaternion.identity);
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
