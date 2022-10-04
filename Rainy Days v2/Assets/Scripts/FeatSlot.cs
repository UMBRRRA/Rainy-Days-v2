using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FeatSlotType
{
    Aquired, Available, Unavailable
}

public class FeatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FeatObject currentFeat;
    public FeatSlotType type;
    private Button btn;
    public Text text;
    public Color aquiredColor;
    public Color availableColor;
    public Color hoverColor;
    public Color unavailableColor;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => OnClick());
        SetColor();
    }

    public void SetColor()
    {
        if (type == FeatSlotType.Aquired)
            text.color = aquiredColor;
        else if (type == FeatSlotType.Available)
            text.color = availableColor;
        else
            text.color = unavailableColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetColor();
    }

    public void OnClick()
    {
        FindObjectOfType<CharacterMenuFunctions>().SetDetails(this);
    }


}
