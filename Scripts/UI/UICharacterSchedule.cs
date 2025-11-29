using TMPro;
using UnityEngine;

public class UICharacterSchedule : MonoBehaviour
{
    private TMP_Text unitName;
    private TMP_Text unitJob;
    private TMP_Dropdown dropDown;

    [Space(10)]
    [Header("Debugs")]
    public Employee2 unit;
    private ScheduleController schCon;
    public string shiftName;
    private void Loaded(object sender, System.EventArgs e) { StartUp(); }
    public void StartUp()
    {
        schCon = transform.parent.parent.parent.GetComponent<ScheduleController>();
        //schCon.characterSchedules.Add(this);

        unitName = transform.GetChild(1).GetComponent<TMP_Text>();
        unitJob = transform.GetChild(2).GetComponent<TMP_Text>();
        dropDown = transform.GetChild(3).GetComponent<TMP_Dropdown>();

        Controller con = Controller.Instance;
        unitName.text = unit.birthName;

        dropDown.onValueChanged.AddListener(delegate { ChangeWorkDay(); });
        if (unit != null) { unit.OnFired += DeleteMe; }

        UpdateUI(this, null);
        unit.OnTaskValueChanged += UpdateUI;

        foreach (ScheduleSet set in schCon.sets)
        {
            dropDown.options.Add(new TMP_Dropdown.OptionData() { text = set.myName });
        }
    }

    private void ButtonPress(int value)
    {
        UpdateUI(this, null);
    }

    private void UpdateUI(object sender, System.EventArgs e)
    {
        unitJob.text = unit.task.ToString();
    }

    private void DeleteMe(object sender, System.EventArgs e)
    {
        //schCon.characterSchedules.Remove(this);
        unit.OnFired -= DeleteMe;
        unit.OnTaskValueChanged -= UpdateUI;
        Destroy(gameObject, 0.1f);
    }
    public void NewUpdateValues()
    {
        UpdateUI(this, null);
    }

    public void ChangeWorkDay()
    {
        shiftName = dropDown.options[dropDown.value].text;
        if (shiftName == "Never leave") { unit.workStart = 0; unit.workEnd = 2400; }
        else if (shiftName == "Stay home") { unit.workStart = -1; unit.workEnd = -1; }
        else
        {
            unit.workStart = schCon.FindSet(dropDown.options[dropDown.value].text).startTime;
            unit.workEnd = schCon.FindSet(dropDown.options[dropDown.value].text).endTime;
        }
    }
    public void UpdateShiftOptions(string setName)
    {
        dropDown.options.Add(new TMP_Dropdown.OptionData() { text = setName });
    }
    public void UpdateSetName(string oldName, string newName)
    {
        for (int i = 0; i < dropDown.options.Count; i++)
        {
            if (dropDown.options[i].text == oldName)
            {
                dropDown.options[i].text = newName;
                //shiftName = newName;
                if (dropDown.value == i) 
                {
                    dropDown.onValueChanged.RemoveAllListeners();
                    dropDown.value = 0;
                    dropDown.value = i;
                    dropDown.onValueChanged.AddListener(delegate { ChangeWorkDay(); });
                }
                break;
            }
        }
    }
    public void ChangeWorkDay(string day)
    {
        if (unit.workDays[day] == true) { unit.workDays[day] = false; }
        else { unit.workDays[day] = true; }
    }
}
