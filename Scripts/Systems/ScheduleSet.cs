using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleSet : MonoBehaviour
{
    private TMP_InputField nameInput;
    public string myName = "New shift";
    private string oldName;
    private ScheduleController schCon;

    private TMP_Text startTimeText;
    private TMP_Text endTimeText;
    private Button startBackButton;
    private Button startNextButton;
    private Button startFastBackButton;
    private Button startFastNextButton;
    private Button endBackButton;
    private Button endNextButton;
    private Button endFastBackButton;
    private Button endFastNextButton;

    [SerializeField] private int startHour;
    [SerializeField] private int startMinute;
    [SerializeField] private int endHour;
    [SerializeField] private int endMinute;

    public int startTime;
    public int endTime;
    [SerializeField] private int set;

    private void Start() { nameInput = transform.GetChild(0).GetComponent<TMP_InputField>(); nameInput.onValueChanged.AddListener(NameUpdate); StartUp(); }
    private void NameUpdate(string newName)
    {
        oldName = myName;
        myName = newName;
        schCon.UpdateSetName(oldName, newName);
    }

    public void StartUp()
    {
        schCon = transform.parent.parent.parent.GetComponent<ScheduleController>();
        if (!schCon.sets.Contains(this)) { schCon.sets.Add(this); }

        oldName = myName;
        switch(set)
        {
            case 0: myName = "Morning?"; break;
            case 1: myName = "Day?"; break;
            case 2: myName = "Night?"; break;
            case 3: myName = "First 12?"; break;
            case 4: myName = "Last 12?"; break;
            case 5: myName = "18 hour?"; break;
        }
        myName = Localizer.Instance.GetLocalizedText(myName);
        nameInput.text = myName;
        //myName = nameInput.text;

        startTimeText = transform.GetChild(1).transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        endTimeText = transform.GetChild(1).transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();

        startBackButton = transform.GetChild(1).transform.GetChild(0).GetChild(1).GetComponent<Button>();
        startNextButton = transform.GetChild(1).transform.GetChild(0).GetChild(2).GetComponent<Button>();
        startFastBackButton = transform.GetChild(1).transform.GetChild(0).GetChild(3).GetComponent<Button>();
        startFastNextButton = transform.GetChild(1).transform.GetChild(0).GetChild(4).GetComponent<Button>();

        endBackButton = transform.GetChild(1).transform.GetChild(1).GetChild(1).GetComponent<Button>();
        endNextButton = transform.GetChild(1).transform.GetChild(1).GetChild(2).GetComponent<Button>();
        endFastBackButton = transform.GetChild(1).transform.GetChild(1).GetChild(3).GetComponent<Button>();
        endFastNextButton = transform.GetChild(1).transform.GetChild(1).GetChild(4).GetComponent<Button>();

        startBackButton.onClick.AddListener(() => ButtonPress(0));
        startNextButton.onClick.AddListener(() => ButtonPress(1));
        startFastBackButton.onClick.AddListener(() => ButtonPress(4));
        startFastNextButton.onClick.AddListener(() => ButtonPress(5));
        endBackButton.onClick.AddListener(() => ButtonPress(2));
        endNextButton.onClick.AddListener(() => ButtonPress(3));
        endFastBackButton.onClick.AddListener(() => ButtonPress(6));
        endFastNextButton.onClick.AddListener(() => ButtonPress(7));

        Controller con = Controller.Instance;

        UpdateUI();
        NameUpdate(myName);
    }
    public void LoadedStartUp(string newName, int startTime, int endTime)
    {
        myName = newName;
        this.startTime = startTime;
        startHour = startTime / 100;
        startMinute = startTime - (startHour * 100);
        this.endTime = endTime;
        endHour = endTime / 100;
        endMinute = endTime - (endHour * 100);
        Invoke("Delay", 1);
    }

    private void Delay()
    {
        UpdateUI();
    }

    private void ButtonPress(int value)
    {
        switch (value)
        {
            case 0:
                if (startHour == 23) { }
                else if (startMinute < 45) { startMinute += 15; }
                else if (startHour != endHour) { startHour++; startMinute = 0; }
                if (startHour > 24) { startHour = 24; }
                break;
            case 1:
                if (startMinute > 00) { startMinute -= 15; }
                else if (startHour == 0) { }
                else { startHour--; startMinute = 45; }
                if (startHour < 0) { startHour = 0; }
                break;
            case 2:
                if (endHour == 24) { }
                else if (endMinute < 45) { endMinute += 15; }
                else { endHour++; endMinute = 0; }
                if (endHour > 24) { endHour = 24; }
                break;
            case 3:
                if (endMinute > 00) { endMinute -= 15; }
                else if (endHour == 0) { }
                else { endHour--; endMinute = 45; }
                if (endHour < 0) { endHour = 0; }
                break;

            case 4:
                if (startHour == 23) { }
                else if (startHour != endHour) { startHour++; }
                if (startHour >= 24) { startHour = 24; startMinute = 0; }
                break;
            case 5:
                if (startHour == 0) { }
                else { startHour--; }
                if (startHour <= 0) { startHour = 0; startMinute = 0; }
                break;
            case 6:
                if (endHour == 24) { }
                else { endHour++; }
                if (endHour >= 24) { endHour = 24; endMinute = 0; }
                break;
            case 7:
                if (endHour == 0) { }
                else { endHour--; }
                if (endHour <= 0) { endHour = 0; endMinute = 0; }
                break;
        }

        if (startHour >= endHour)
        {
            startHour = endHour;
            if (startMinute > endMinute) { startMinute = endMinute; }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        string startMinuteString = ""; string startHourString = "";
        if (startMinute < 10) { startMinuteString = "0" + startMinute.ToString(); } else { startMinuteString = startMinute.ToString(); }
        if (startHour < 10) { startHourString = "0" + startHour.ToString(); } else { startHourString = startHour.ToString(); }

        string endMinuteString = ""; string endHourString = "";
        if (endMinute < 10) { endMinuteString = "0" + endMinute.ToString(); } else { endMinuteString = endMinute.ToString(); }
        if (endHour < 10) { endHourString = "0" + endHour.ToString(); } else { endHourString = endHour.ToString(); }

        if (startTimeText == null)
        {
            startTimeText = transform.GetChild(1).transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            endTimeText = transform.GetChild(1).transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            schCon = transform.parent.parent.parent.GetComponent<ScheduleController>();
        }
        startTimeText.text = startHourString + ":" + startMinuteString;
        endTimeText.text = endHourString + ":" + endMinuteString;

        startTime = (startHour * 100) + startMinute;
        endTime = (endHour * 100) + endMinute;

        schCon.SetUpdated(myName, this);
    }
    public void DeleteMe()
    {
        schCon.DeleteShift(myName);
        Destroy(gameObject, 0.01f);
    }
}
