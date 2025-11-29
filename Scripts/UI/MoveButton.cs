using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

//[AddComponentMenu("UI/Button", 30)]
public class MoveButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    [Serializable]

    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    protected MoveButton()
    { }

    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        Press();

        if (!IsActive() || !IsInteractable())
            return;
    }
}
