using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecializedSorter : MonoBehaviour
{
    [SerializeField] private Transform employeeZone;
    [SerializeField] private Transform applicantZone;
    [SerializeField] private Button[] employeeFilterButtons;
    [SerializeField] private Button[] applicantFilterButtons;
    public bool[] employeeShows;
    public bool[] applicantShows;

    public static SpecializedSorter Instance { get; private set; }
    private void Awake() { Instance = this; }

    public void FilterApplicants(int job)
    {
        //if -1 show all
        if (job == -1)
        {
            for (int i = 0; i < applicantZone.childCount; i++)
            {
                applicantZone.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < applicantFilterButtons.Length; i++)
            {
                applicantFilterButtons[i].GetComponent<SettingsButton>().Disable();
            }
            for (int i = 0; i < applicantShows.Length; i++) { applicantShows[i] = true; }
        }
        //if 0 show none
        else if (job == 0)
        {
            for (int i = 0; i < applicantZone.childCount; i++)
            {
                applicantZone.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < applicantFilterButtons.Length; i++)
            {
                applicantFilterButtons[i].GetComponent<SettingsButton>().Enable();
            }
            for (int i = 0; i < applicantShows.Length; i++) { applicantShows[i] = false; }
        }
        else
        {
            if (applicantShows[job]) { applicantShows[job] = false; }
            else { applicantShows[job] = true; }

            for (int i = 0; i < applicantZone.childCount; i++)
            {
                if (applicantZone.GetChild(i).GetComponent<StaffApplicant>().occupation == job) { applicantZone.GetChild(i).gameObject.SetActive(applicantShows[job]); }
            }
        }
    }
    public void FilterEmployees(int job)
    {
        //if -1 show all
        if (job == -1)
        {
            for (int i = 0; i < employeeZone.childCount; i++)
            {
                employeeZone.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < employeeFilterButtons.Length; i++)
            {
                employeeFilterButtons[i].GetComponent<SettingsButton>().Disable();
            }
            for (int i = 0; i < employeeShows.Length; i++) { employeeShows[i] = true; }
        }
        //if 0 show none
        else if (job == 0)
        {
            for (int i = 0; i < employeeZone.childCount; i++)
            {
                employeeZone.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < employeeFilterButtons.Length; i++)
            {
                employeeFilterButtons[i].GetComponent<SettingsButton>().Enable();
            }
            for (int i = 0; i < employeeShows.Length; i++) { employeeShows[i] = false; }
        }
        else
        {
            if (employeeShows[job]) { employeeShows[job] = false; }
            else { employeeShows[job] = true; }

            for (int i = 0; i < employeeZone.childCount; i++)
            {
                if (employeeZone.GetChild(i).GetComponent<Staff>().occupation == job) { employeeZone.GetChild(i).gameObject.SetActive(employeeShows[job]); }
            }
        }
    }
    public void SortByLevel(bool employee)
    {
        List<int> childrenNumbers = new List<int>();

        if (employee)
        {
            for (int x = 0; x < employeeZone.childCount; x++)
            { childrenNumbers.Add(employeeZone.GetChild(x).GetComponent<Staff>().level); }
            childrenNumbers.Sort();
            for (int i = 0; i < employeeZone.childCount; i++)
            { employeeZone.GetChild(i).SetSiblingIndex(childrenNumbers.IndexOf(employeeZone.GetChild(i).GetComponent<Staff>().level)); }
        }
        else
        {


            for (int x = 0; x < applicantZone.childCount; x++)
            {
                childrenNumbers.Add(applicantZone.GetChild(x).GetComponent<StaffApplicant>().level);
            }
            childrenNumbers.Sort();
            for (int i = 0; i < applicantZone.childCount; i++)
            { applicantZone.GetChild(i).SetSiblingIndex(childrenNumbers.IndexOf(applicantZone.GetChild(i).GetComponent<StaffApplicant>().level)); }
        }
    }
}
