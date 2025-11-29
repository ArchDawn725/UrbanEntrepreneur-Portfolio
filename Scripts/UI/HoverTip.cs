using UnityEngine;
using UnityEngine.EventSystems;
using static CodeMonkey.Utils.UI_TextComplex;
using UnityEngine.UIElements;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string popMessage;

    public void OnPointerEnter(PointerEventData eventData) { Activate(); }
    public void OnPointerExit(PointerEventData eventData) { DeActivate(); }

    private void Activate()
    {
        if (popMessage != "") { ToolTip.Instance?.ShowToolTip(popMessage); }
    }

    private void DeActivate()
    {
        ToolTip.Instance?.HideToolTip();
    }
}
