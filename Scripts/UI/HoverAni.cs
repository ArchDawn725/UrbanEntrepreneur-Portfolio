using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAni : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform uiElement;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 hoverPosition;
    [SerializeField] private Vector2 lockPosition;

    [SerializeField] private Vector2 targetPosition;

    [SerializeField] private float speedMultiplier = 0.05f;
    private MoveButton button;

    [SerializeField] private bool lockable;
    [SerializeField] private bool locked;

    [SerializeField] private KeyCode KeyCode;

    private void Start()
    {
        uiElement = transform.GetChild(0).GetComponent<RectTransform>();
        button = GetComponent<MoveButton>();
        if (button != null) { button.onClick.AddListener(ButtonPressed); }
        startPosition = uiElement.anchoredPosition;

        DeActivate();
    }
    public void OnPointerEnter(PointerEventData eventData) { Activate(); }
    public void OnPointerExit(PointerEventData eventData) { DeActivate(); }

    private void Activate()
    {
        if (locked) { targetPosition = lockPosition; }
        else { targetPosition = hoverPosition; }
    }

    private void DeActivate()
    {
        if (locked) { targetPosition = lockPosition; }
        else { targetPosition = startPosition; }
    }

    private void Update()
    {
        uiElement.anchoredPosition = Vector3.Lerp(uiElement.anchoredPosition, targetPosition, (Time.deltaTime / speedMultiplier) * TickSystem.Instance.adjustedTimeSpeed);
        if (Input.GetKeyDown(KeyCode.LeftAlt) && KeyCode == KeyCode.LeftAlt) { button.onClick.Invoke(); }
    }

    private void ButtonPressed()
    {
        if (lockable)
        {
            if (locked) { locked = false; targetPosition = hoverPosition; }
            else { locked = true; targetPosition = lockPosition; }
        }
    }
}
