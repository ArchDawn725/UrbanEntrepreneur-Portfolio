using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleController : MonoBehaviour
{
    public static ScheduleController Instance { get; private set; }
    private void Awake() { Instance = this; }
    //get fall schedule sets 
    //get all character schedulers

    //ability to update all character schedulars units
    public List<ScheduleSet> sets;
    public List<Staff> characterSchedules;

    [SerializeField] private GameObject shift;
    [SerializeField] private Transform shiftHolder;

    [SerializeField] private string lastUpdatedString;
    public void SetUpdated(string setName, ScheduleSet set)
    {
        //get every character with same setname and change each employee's time to new time
        foreach (Staff characterS in characterSchedules)
        {
            if (characterS.shiftName == setName) 
            { 
                characterS.unit.workStart = set.startTime; 
                characterS.unit.workEnd = set.endTime;
            }
        }
        lastUpdatedString = setName;
    }
    public ScheduleSet FindSet(string setName)
    {
        foreach(ScheduleSet set in sets) { if (set.myName == setName) { return set; } }
        return null;
    }
    public void NewShift()
    {
        if (NewShiftChecker())
        {
            GameObject newLog = Instantiate(shift, shiftHolder);
            foreach (Staff characterSchedule in characterSchedules) { characterSchedule.UpdateShiftOptions("New shift"); }
        }
    }
    public void NewShift(string newName, int startTime, int endTime)
    {
        if (NewShiftChecker())
        {
            GameObject newLog = Instantiate(shift, shiftHolder);
            newLog.GetComponent<ScheduleSet>().LoadedStartUp(newName, startTime, endTime);
            foreach (Staff characterSchedule in characterSchedules) { characterSchedule.UpdateShiftOptions(newName); }
        }
    }
    public void UpdateSetName(string oldName, string newName)
    {
        foreach (Staff characterSchedule in characterSchedules) { characterSchedule.UpdateSetName(oldName, newName); }
    }
    private bool NewShiftChecker()
    {
        foreach (ScheduleSet character in sets) { if (character.myName == Localizer.Instance.GetLocalizedText("New shift")) { return false; } }
        return true;
    }
    public void DeleteShift(string scheduleName)
    {
        foreach (Staff characterS in characterSchedules)
        {
            if (characterS.shiftName == scheduleName)
            {
                characterS.shiftName = Localizer.Instance.GetLocalizedText("Never Leave");
                characterS.unit.workStart = 0;
                characterS.unit.workEnd = 2400;
            }
            int lookingFor = 0;
            for (int i = 0; i < characterS.scheduleDropDown.options.Count; i++)
            {
                if (characterS.scheduleDropDown.options[i].text == scheduleName)
                {
                    lookingFor = i; break;
                }
            }
            characterS.scheduleDropDown.options.RemoveAt(lookingFor);
            characterS.scheduleDropDown.RefreshShownValue();
            characterS.scheduleDropDown.value = 0;
        }
        lastUpdatedString = Localizer.Instance.GetLocalizedText("Never Leave");
    }
}
