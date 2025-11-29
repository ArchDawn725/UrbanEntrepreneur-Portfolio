using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EOTMController : MonoBehaviour
{
    private Transform list;
    private TMP_Text disc;
    private TMP_Text triats;
    private TMP_Text levels;
    private TMP_Text level;
    private Image emotion;
    [SerializeField] private Button promoteButton;
    [SerializeField] private Button dismissButton;

    public EOTM employeeOfTheMonth;
    [SerializeField] private List<Sprite> emotions = new List<Sprite>();
    public List<Sprite> borders = new List<Sprite>();

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform employeeHolder;

    private void Start()
    {
        list = transform.GetChild(0).GetChild(3);

        Transform DiscriptionDisplay = transform.GetChild(0).GetChild(2); 
        emotion = DiscriptionDisplay.GetChild(0).GetComponent<Image>();
        disc = DiscriptionDisplay.GetChild(1).GetComponent<TMP_Text>();
        triats = DiscriptionDisplay.GetChild(2).GetComponent<TMP_Text>();
        level = DiscriptionDisplay.GetChild(3).GetComponent<TMP_Text>();
        levels = DiscriptionDisplay.GetChild(4).GetComponent<TMP_Text>();

        promoteButton.onClick.AddListener(Promote);
        dismissButton.onClick.AddListener(CloseMenu);

        DismissDisc();
    }

    public void UpdateDisc(EOTM employeeMonth)
    {
        Employee2 employee = employeeMonth.employee;

        GetEmotionSprite(employee);
        disc.text =
            employee.myName + System.Environment.NewLine +
            employee.task.ToString() + System.Environment.NewLine +
            "Wage: " + employee.hourlyWage.ToString("f2") + "$" + System.Environment.NewLine +
            "Employeed for: "  + employee.employeedForDays.ToString() + " Days" + System.Environment.NewLine +
            "Times called-out: x" + employee.calledOutAmount.ToString() + System.Environment.NewLine +
            "Times late: x" + employee.lateAmount.ToString() + System.Environment.NewLine +
            "Overtime worked: "  + employee.overtimeWorked.ToString("f2") + " Hours"//
            ;
        List<string> traits = new List<string>();
        foreach (KeyValuePair<string, bool> pair in employee.traits) { if (pair.Value == true) { traits.Add(pair.Key); } }
        List<string> newTraits = RemoveUselessTraits(traits);
        string traitString = string.Join(Environment.NewLine, newTraits);
        triats.text =
            "Traits:" + System.Environment.NewLine +
            traitString
            ;
        level.text = "Lv." + employee.GetMyTotalLevel().ToString();
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        levels.text = invSkill + " " + custSkill + " " + janitorialSkill + " " + engineerSkill + " " + managementSkill;
    }
    public void DismissDisc()
    {
        if (!employeeOfTheMonth)
        {
            emotion.sprite = null;
            disc.text = "";
            triats.text = "";
            level.text = "";
            levels.text = "";
        }
        else { UpdateDisc(employeeOfTheMonth); }
    }

    public void SelectEOTM(EOTM employee)
    {
        //deselect old EOTM
        if (employeeOfTheMonth != null)
        {
            employeeOfTheMonth.UnPress();
        }

        //Select new EOTM
        employeeOfTheMonth = employee;
        UpdateDisc(employeeOfTheMonth);

        promoteButton.interactable = true;
    }
    private void Promote()
    {
        if (employeeOfTheMonth.employee.status == Employee2.Status.employee)
        {
            employeeOfTheMonth.employee.status = Employee2.Status.employeeOfTheMonth;
            employeeOfTheMonth.employee.myUICharacter.border.sprite = borders[1];
        }
        else if (employeeOfTheMonth.employee.status == Employee2.Status.employeeOfTheMonth)
        {
            employeeOfTheMonth.employee.status = Employee2.Status.employeeOfTheSeason;
            employeeOfTheMonth.employee.myUICharacter.border.sprite = borders[2];
        }
        else if(employeeOfTheMonth.employee.status == Employee2.Status.employeeOfTheSeason)
        {
            employeeOfTheMonth.employee.status = Employee2.Status.employeeOfTheYear;
            employeeOfTheMonth.employee.myUICharacter.border.sprite = borders[3];
        }

        TickSystem.Instance.UnPause();
        UIController.Instance.SetMainAnimatorString("CloseEOTM");
    }
    public void CloseMenu() { TickSystem.Instance.UnPause(); UIController.Instance.SetMainAnimatorString("CloseEOTM"); }
    private void GetEmotionSprite(Employee2 employee)
    {
        if (employee.averageStress < 20) { emotion.sprite = emotions[0]; }
        if (employee.averageStress >= 20 && employee.averageStress < 40) { emotion.sprite = emotions[1]; }
        if (employee.averageStress >= 40 && employee.averageStress < 60) { emotion.sprite = emotions[2]; }
        if (employee.averageStress >= 60 && employee.averageStress < 80) { emotion.sprite = emotions[3]; }
        if (employee.averageStress >= 80) { emotion.sprite = emotions[4]; }
    }
    public void EOTM()
    {
        titleText.text = "Employee of the Month award!";
        for (int i = 0; i < employeeHolder.childCount; i++)
        {
            if (employeeHolder.GetChild(i).GetComponent<EOTM>().employee.status == Employee2.Status.employee)
            {
                employeeHolder.GetChild(i).gameObject.SetActive(true);
            }
            else { employeeHolder.GetChild(i).gameObject.SetActive(false); }
        }
    }
    public void EOTS()
    {
        titleText.text = "Employee of the Season award!";
        for (int i = 0; i < employeeHolder.childCount; i++)
        {
            if (employeeHolder.GetChild(i).GetComponent<EOTM>().employee.status == Employee2.Status.employeeOfTheMonth)
            {
                employeeHolder.GetChild(i).gameObject.SetActive(true);
            }
            else { employeeHolder.GetChild(i).gameObject.SetActive(false); }
        }
    }
    public void EOTY()
    {
        titleText.text = "Employee of the Year award!";
        for (int i = 0; i < employeeHolder.childCount; i++)
        {
            if (employeeHolder.GetChild(i).GetComponent<EOTM>().employee.status == Employee2.Status.employeeOfTheSeason)
            {
                employeeHolder.GetChild(i).gameObject.SetActive(true);
            }
            else { employeeHolder.GetChild(i).gameObject.SetActive(false); }
        }
    }
    private List<string> RemoveUselessTraits(List<string> traits)
    {
        traits.Remove("procrastinator");
        traits.Remove("busy");
        traits.Remove("free");
        traits.Remove("frugal");
        traits.Remove("needy");
        traits.Remove("Poor");
        traits.Remove("Rich");
        traits.Remove("Stingy");
        traits.Remove("Spendful");

        return traits;
    }
}
