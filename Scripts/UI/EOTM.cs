using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EOTM : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Employee2 employee;

    public PersonVisualCon vis;
    private Button myButton;
    private EOTMController con;
    private Image border;
    private TextMeshProUGUI nameText;
    
    public void StartUp()
    {
        employee.OnFired += DeleteMe;

        //get transforms
        vis = transform.GetChild(1).GetComponent<PersonVisualCon>();
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = employee.myName;
        myButton = GetComponent<Button>();
        con = transform.parent.parent.parent.GetComponent<EOTMController>();
        border = transform.GetChild(2).GetComponent<Image>();
        nameText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        myButton.onClick.AddListener(ButtonPress);
        nameText.text = employee.birthName;
    }
    private void ButtonPress()
    {
        if (employee.status == Employee2.Status.employee)
        {
            border.sprite = con.borders[1];
        }
        else if (employee.status == Employee2.Status.employeeOfTheMonth)
        {
            border.sprite = con.borders[2];
        }
        else if (employee.status == Employee2.Status.employeeOfTheSeason)
        {
            border.sprite = con.borders[3];
        }
        con.SelectEOTM(this);
    }
    public void UnPress()
    {
        if (employee.status == Employee2.Status.employee)
        {
            border.sprite = con.borders[0];
        }
        else if (employee.status == Employee2.Status.employeeOfTheMonth)
        {
            border.sprite = con.borders[1];
        }
        else if (employee.status == Employee2.Status.employeeOfTheSeason)
        {
            border.sprite = con.borders[2];
        }
    }
    private void DeleteMe(object sender, System.EventArgs e)
    {
        Destroy(gameObject, 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //size increase
        transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //size Decrease
        transform.localScale = new Vector3(1, 1f, 1);
    }
}
