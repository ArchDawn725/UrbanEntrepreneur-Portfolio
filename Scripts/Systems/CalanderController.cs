using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CalanderController : MonoBehaviour
{
    public static CalanderController Instance { get; private set; }
    private void Awake() { Instance = this; }
    [SerializeField] private List<Date> dates = new List<Date>();
    [SerializeField] private Transform buttonHolder;

    [SerializeField] private List<int> defualtList = new List<int>();
    [SerializeField] private List<string> defualtListnames = new List<string>();
    [SerializeField] private List<bool> dayListTest = new List<bool>();

    private void Start()
    {
        Invoke("Delay", 0.1f);
    }
    private void Delay()
    {
        if (UIController.Instance.dayOfTheYear == 365)
        {
            List<string> value2 = new List<string>();
            for (int i = 0; i < dates.Count; i++)
            {
                value2.Add("");
            }
            StartCoroutine(SetDates(defualtList, defualtListnames, value2));
        }
        else
        {
            List<string> value2 = new List<string>();
            List<string> value3 = new List<string>();
            List<int> value4 = new List<int>();
            for (int i = 0; i < dates.Count; i++)
            {
                value2.Add("");
                value3.Add("");
                value4.Add(0);
            }
            StartCoroutine(SetDates(value4, value3, value2));
        }

    }

    public IEnumerator SetDates(List<int> values, List<string> names, List<string> weatherList)
    {
        for (int i = 0; i < dates.Count; i++)
        {
            dates[i].SetDate(values[i], names[i], weatherList[i]);
            yield return new WaitForEndOfFrame();
        }
    }
    public List<int> GetValues()
    {
        List<int> values = new List<int>();
        for (int i = 0; i < dates.Count; i++)
        {
            values.Add(dates[i].value);
        }
        return values;
    }
    public List<string> GetNames()
    {
        List<string> values = new List<string>();
        for (int i = 0; i < dates.Count; i++)
        {
            values.Add(dates[i].myName);
        }
        return values;
    }
    public List<string> GetWeather()
    {
        List<string> values = new List<string>();
        for (int i = 0; i < dates.Count; i++)
        {
            values.Add(dates[i].weather);
        }
        return values;
    }
    public void SetPieCharts(int type)
    {
        List<float> values = new List<float>();

        if (type == 0) { for (int i = 0; i < dates.Count; i++) { values.Add(0); } }
        else
        {
            for (int i = 0; i < dates.Count; i++)
            {
                List<bool> hourList = new List<bool>() { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};


                for (int x = 0; x < Controller.Instance.employees.Count; x++)
                {
                    if (((int)Controller.Instance.employees[x].task) == type && Controller.Instance.employees[x].workDays[dates[i].dayOfTheWeek] == true)
                    {
                        if (Controller.Instance.employees[x].workStart <= 0 && Controller.Instance.employees[x].workEnd > 0) { hourList[0] = true; }
                        if (Controller.Instance.employees[x].workStart <= 100 && Controller.Instance.employees[x].workEnd > 100) { hourList[1] = true; }
                        if (Controller.Instance.employees[x].workStart <= 200 && Controller.Instance.employees[x].workEnd > 200) { hourList[2] = true; }
                        if (Controller.Instance.employees[x].workStart <= 300 && Controller.Instance.employees[x].workEnd > 300) { hourList[3] = true; }
                        if (Controller.Instance.employees[x].workStart <= 400 && Controller.Instance.employees[x].workEnd > 400) { hourList[4] = true; }
                        if (Controller.Instance.employees[x].workStart <= 500 && Controller.Instance.employees[x].workEnd > 500) { hourList[5] = true; }
                        if (Controller.Instance.employees[x].workStart <= 600 && Controller.Instance.employees[x].workEnd > 600) { hourList[6] = true; }
                        if (Controller.Instance.employees[x].workStart <= 700 && Controller.Instance.employees[x].workEnd > 700) { hourList[7] = true; }
                        if (Controller.Instance.employees[x].workStart <= 800 && Controller.Instance.employees[x].workEnd > 800) { hourList[8] = true; }
                        if (Controller.Instance.employees[x].workStart <= 900 && Controller.Instance.employees[x].workEnd > 900) { hourList[9] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1000 && Controller.Instance.employees[x].workEnd > 1000) { hourList[10] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1100 && Controller.Instance.employees[x].workEnd > 1100) { hourList[11] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1200 && Controller.Instance.employees[x].workEnd > 1200) { hourList[12] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1300 && Controller.Instance.employees[x].workEnd > 1300) { hourList[13] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1400 && Controller.Instance.employees[x].workEnd > 1400) { hourList[14] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1500 && Controller.Instance.employees[x].workEnd > 1500) { hourList[15] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1600 && Controller.Instance.employees[x].workEnd > 1600) { hourList[16] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1700 && Controller.Instance.employees[x].workEnd > 1700) { hourList[17] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1800 && Controller.Instance.employees[x].workEnd > 1800) { hourList[18] = true; }
                        if (Controller.Instance.employees[x].workStart <= 1900 && Controller.Instance.employees[x].workEnd > 1900) { hourList[19] = true; }
                        if (Controller.Instance.employees[x].workStart <= 2000 && Controller.Instance.employees[x].workEnd > 2000) { hourList[20] = true; }
                        if (Controller.Instance.employees[x].workStart <= 2100 && Controller.Instance.employees[x].workEnd > 2100) { hourList[21] = true; }
                        if (Controller.Instance.employees[x].workStart <= 2200 && Controller.Instance.employees[x].workEnd > 2200) { hourList[22] = true; }
                        if (Controller.Instance.employees[x].workStart <= 2300 && Controller.Instance.employees[x].workEnd > 2300) { hourList[23] = true; }
                    }


                }

                float amount = 0;
                for (int x = 0; x < hourList.Count; x++) { if (hourList[x] == true) { amount++; } }
                dayListTest = hourList;
                values.Add(amount/24);
            }

        }

        for (int i = 0; i < dates.Count; i++)
        {
            dates[i].SetAmount(values[i], type);
        }

        //reset all buttons as only one can be active at a time
        for (int i = 0; i < buttonHolder.childCount; i++)
        {
            if (i != type)
            {
                buttonHolder.GetChild(i).GetComponent<SettingsButton>().Disable();
            }

        }
    }
}
