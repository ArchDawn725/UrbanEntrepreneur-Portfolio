using System;
using TMPro;
using UnityEngine;

public class UITaskCharacter : MonoBehaviour
{
    /*
    private TMP_Text unitName;
    private TMP_Text unitActivity;
    private TMP_Dropdown dropDown;

    [Space(10)]
    [Header("Debugs")]
    public Employee2 unit;
    public void StartUp()
    {
        unitName = transform.GetChild(0).GetComponent<TMP_Text>();
        unitActivity = transform.GetChild(1).GetComponent<TMP_Text>();
        dropDown = transform.GetChild(2).GetComponent<TMP_Dropdown>();

        unitName.text = unit.birthName;

        unit.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        /*
        if (invSkill > 0) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "inventory_Management" }); }
        if (custSkill > 0) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "customer_Service" }); }
        if (janitorialSkill > 0) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "janitorial" }); }
        if (engineerSkill > 0) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "engineering" }); }
        if (managementSkill > 0) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "management" }); }
        *//*
        dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "stocker" });
        dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "cashier" });
        if (TransitionController.Instance.difficulty < 3) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "janitor" }); }
        if (TransitionController.Instance.difficulty < 3) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "engineer" }); }
        if (TransitionController.Instance.difficulty < 3) { dropDown.options.Add(new TMP_Dropdown.OptionData() { text = "manager" }); }
                
        

        unit.OnObjectiveValueChanged += ChangeObjective;
        unit.OnTaskValueChanged += UnitTaskChanged;
        unit.OnFired += DeleteMe;
        dropDown.onValueChanged.AddListener(delegate { ChangeOccupation(); });
    }

    private void ChangeObjective(object sender, System.EventArgs e)
    {
        unitActivity.text = unit.objective.ToString();
    }

    private void ChangeOccupation()
    {
        unit.SwitchTask(null, dropDown.options[dropDown.value].text, null);
    }

    private void UnitTaskChanged(object sender, System.EventArgs e)
    {
        string newTask = unit.task.ToString();

        dropDown.onValueChanged.RemoveAllListeners();

        for (int i = 0; i < dropDown.options.Count; i++)
        {
            if (dropDown.options[i].text == newTask)
            {
                dropDown.value = i; break;
            }
        }

        dropDown.onValueChanged.AddListener(delegate { ChangeOccupation(); });
    }

    private void DeleteMe(object sender, System.EventArgs e)
    {
        unit.OnObjectiveValueChanged -= ChangeObjective;
        unit.OnTaskValueChanged -= UnitTaskChanged;
        unit.OnFired -= DeleteMe;
        Destroy(gameObject, 0.1f);
    }
*/
}
