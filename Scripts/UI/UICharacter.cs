using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color[] colors;

    private TMP_Text unitName;
    private Image backgroundImage;
    private Image backgroundColor;
    //private Image personImage;
    private Button button;
    [Space(10)]
    [Header("Debugs")]
    public Employee2 unit;
    public PersonVisualCon vis;
    public Image border;
    [SerializeField] private List<Sprite> borders = new List<Sprite>();
    public void StartUp()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonPress);
        unitName = transform.GetChild(2).GetComponent<TMP_Text>();
        backgroundColor = transform.GetChild(0).GetComponent<Image>();
        backgroundImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        //personImage = transform.GetChild(2).GetComponent<Image>();
        vis = transform.GetChild(1).GetComponent<PersonVisualCon>();
        border = transform.GetChild(1).GetChild(2).GetComponent<Image>();

        backgroundImage.color = colors[0];
        unit.OnObjectiveValueChanged += ChangeBackgroundColor;
        TickSystem.Instance.On10Tick += ChangeBackgroundColor;
        unit.OnFired += DeleteMe;
        unitName.text = unit.birthName;
        ChangeBackgroundColor(null, null);
    }

    private void ButtonPress()
    {
        if (Controller.Instance.selectedEmployee == unit)
        {
            if (unit.objective != Employee2.Objective.absent)
            {
                Camera.main.GetComponent<CameraSystem2D>().CameraTarget = unit.gameObject.transform;
            }
        }
        else
        {
            if (Controller.Instance.selectedEmployee != null) { Controller.Instance.selectedEmployee.Deselected(); }
            UIController.Instance.NewSelectPopUp(unit.gameObject);
            Controller.Instance.selectedEmployee = unit;
        }

        unit.Selected();
    }

    public void ChangeBackgroundColor(object sender, System.EventArgs e)
    {
        backgroundImage.color = colors[(int)unit.objective];

        float lerpValue = (float)unit.stress / 100f;
        Color newColor = Color.Lerp(Color.green, Color.red, lerpValue);
        backgroundColor.color = newColor;
    }

    private void DeleteMe(object sender, System.EventArgs e)
    {
        unit.OnObjectiveValueChanged -= ChangeBackgroundColor;
        TickSystem.Instance.On10Tick -= ChangeBackgroundColor;
        unit.OnFired -= DeleteMe;
        Destroy(gameObject, 0.1f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        vis.myAni.SetBool("Hovered", true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        vis.myAni.SetBool("Hovered", false);
    }

    public void Promote(int level)
    {
        border.sprite = borders[level];
    }
}
