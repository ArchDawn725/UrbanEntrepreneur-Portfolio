using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Staff : MonoBehaviour
{
    [SerializeField] private List<Color> colorList = new List<Color>();
    private Image myColorImage;
    [SerializeField] private List<Sprite> emotions = new List<Sprite>();
    private Image myEmotion;
    private TextMeshProUGUI myName;
    private TextMeshProUGUI hiredTask;
    private TextMeshProUGUI totalLevel;
    private TMP_Dropdown taskDropDown;
    private TextMeshProUGUI unitActivity;
    private TextMeshProUGUI taskLevel;
    private TextMeshProUGUI objective;
    public TMP_Dropdown scheduleDropDown; 
    //days

    [HideInInspector] public Employee2 unit;
    private ScheduleController schCon;

    public string shiftName;
    [SerializeField] private Vector2 parentSize = new Vector2(189.941f, 48.285f);
    public int level;
    public int occupation;
    [SerializeField] private Sprite[] jobSprites;
    [SerializeField] private Color[] jobColors;
    [SerializeField] private SettingsButton[] workdays;
    private void Start()
    {
        parentSize = transform.parent.parent.parent.GetComponent<RectTransform>().anchoredPosition;
        //transform.parent.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0.001f, 0.001f);
    }
    public void StartUp()
    {
        myColorImage = transform.GetComponent<Image>();
        myEmotion = transform.GetChild(2).GetChild(0).GetComponent<Image>();
        myName = transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        hiredTask = transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        totalLevel = transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();
        taskDropDown = transform.GetChild(4).GetComponent<TMP_Dropdown>();
        unitActivity = transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        taskLevel = transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        objective = transform.GetChild(6).GetComponent<TextMeshProUGUI>();
        scheduleDropDown = transform.GetChild(8).GetComponent<TMP_Dropdown>();
        //days

        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(UnitSelectPressed);

        myName.text = unit.birthName;
        name = unit.birthName;


        taskDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("stocker") });
        if (TransitionController.Instance.tutorialLevel > 1) { taskDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("cashier") }); }
        if (TransitionController.Instance.tutorialLevel > 2) { taskDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("janitor") }); }
        if (TransitionController.Instance.tutorialLevel > 3) { taskDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("engineer") }); }
        if (TransitionController.Instance.tutorialLevel > 4) { taskDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("manager") }); }

        schCon = UIController.Instance.schCon;//transform.parent.parent.parent.GetComponent<ScheduleController>();
        schCon.characterSchedules.Add(this);

        scheduleDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("Never leave") });
        scheduleDropDown.options.Add(new TMP_Dropdown.OptionData() { text = Localizer.Instance.GetLocalizedText("Stay home") });

        foreach (ScheduleSet set in schCon.sets)
        {
            scheduleDropDown.options.Add(new TMP_Dropdown.OptionData() { text = set.myName });
        }
        scheduleDropDown.onValueChanged.AddListener(delegate { ChangeWorkDay(); });

        UpdateEmotion(null, null);
        TickSystem.Instance.On25Tick += UpdateEmotion;
        unit.OnObjectiveValueChanged += ChangeObjective;
        unit.OnTaskValueChanged += UnitTaskChanged;
        unit.OnFired += DeleteMe;
        taskDropDown.onValueChanged.AddListener(delegate { ChangeOccupation(); });
        Invoke("Delay", 0.15f);
        //transform.parent.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
    }
    private void Delay()
    {
        string taskInitial = "O";
        unit.hiredTask = 0;
        if (unit.status != Employee2.Status.owner)
        {
            switch (unit.task)
            {
                case Employee2.Task.stocker: taskInitial = "S"; unit.hiredTask = 1; occupation = 1; break;
                case Employee2.Task.cashier: taskInitial = "C"; unit.hiredTask = 2; occupation = 2; break;
                case Employee2.Task.janitor: taskInitial = "J"; unit.hiredTask = 3; occupation = 3; break;
                case Employee2.Task.engineer: taskInitial = "E"; unit.hiredTask = 4; occupation = 4; break;
                case Employee2.Task.manager: taskInitial = "M"; unit.hiredTask = 5; occupation = 5; break;
            }
        }

        hiredTask.text = "(" + taskInitial + ")";
        HideChecker();
        SetJobSprite();
        UpdateEmotion(null, null);
    }
    private void UpdateEmotion(object sender, System.EventArgs e)
    {
        unit.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        int totalSkill = invSkill + custSkill + janitorialSkill + engineerSkill + managementSkill;

        myEmotion.sprite = emotions[Mathf.RoundToInt(unit.stress / 20)];
        totalLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + totalSkill.ToString();
        switch (unit.task)
        {
            case Employee2.Task.stocker: taskLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + invSkill.ToString(); level = invSkill; break;
            case Employee2.Task.cashier: taskLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + custSkill.ToString(); level = custSkill; break;
            case Employee2.Task.janitor: taskLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + janitorialSkill.ToString(); level = janitorialSkill; break;
            case Employee2.Task.engineer: taskLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + engineerSkill.ToString(); level = engineerSkill; break;
            case Employee2.Task.manager: taskLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + managementSkill.ToString(); level = managementSkill; break;
        }

        if (unit.objective == Employee2.Objective.absent)
        {
            objective.text = "Off duty";
            if (unit.late) { objective.text = "Late"; }
            if (unit.calledOut) { objective.text = "Called out"; }
            if (unit.sentHome) { objective.text = "Sent Home"; }
            if (unit.workDays.Count > 0) { if (unit.workDays[UIController.Instance.weekday] == false) { objective.text = "Day off"; } }
        }
        if (unit.sentHome) { objective.text = "Sent Home"; }

        objective.GetComponent<AutoLocalizer>().UpdateLocalizedText();
        UpdateEmotionColor();
    }
    private void UnitTaskChanged(object sender, System.EventArgs e)
    {
        string newTask = Localizer.Instance.GetLocalizedText(unit.task.ToString());

        taskDropDown.onValueChanged.RemoveAllListeners();

        for (int i = 0; i < taskDropDown.options.Count; i++)
        {
            if (taskDropDown.options[i].text == newTask)
            {
                taskDropDown.value = i; occupation = i + 1; SetJobSprite(); break;
            }
        }

        taskDropDown.onValueChanged.AddListener(delegate { ChangeOccupation(); });
        taskDropDown.name = unit.task.ToString();
    }
    private void ChangeObjective(object sender, System.EventArgs e)
    {
        //update
        myColorImage.color = colorList[(int)unit.objective];


        if (unit.objective == Employee2.Objective.absent)
        {
            objective.text = "Off duty";
            if (unit.late) { objective.text = "Late"; }
            if (unit.calledOut) { objective.text = "Called out"; }
            if (unit.sentHome) { objective.text = "Sent Home"; }
            if (unit.workDays.Count > 0) { if (unit.workDays[UIController.Instance.weekday] == false) { objective.text = "Day off"; } }
        }
        else { objective.text = unit.objective.ToString(); }
        if (unit.sentHome) { objective.text = "Sent Home"; }
        objective.GetComponent<AutoLocalizer>().UpdateLocalizedText();
    }
    private void UnitSelectPressed()
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
    private void ChangeOccupation()
    {
        unit.SwitchTask(null, taskDropDown.options[taskDropDown.value].text, null);
    }
    public void ChangeWorkDay(string day)
    {
        if (unit.workDays[day] == true) { unit.workDays[day] = false; }
        else { unit.workDays[day] = true; }
    }
    public void ChangeWorkDay()
    {
        shiftName = scheduleDropDown.options[scheduleDropDown.value].text;
        if (shiftName == "Never leave" || shiftName == Localizer.Instance.GetLocalizedText("Never leave")) { unit.workStart = 0; unit.workEnd = 2400; }
        else if (shiftName == "Stay home" || shiftName == Localizer.Instance.GetLocalizedText("Stay home")) { unit.workStart = -1; unit.workEnd = -1; }
        else
        {
            unit.workStart = schCon.FindSet(scheduleDropDown.options[scheduleDropDown.value].text).startTime;
            unit.workEnd = schCon.FindSet(scheduleDropDown.options[scheduleDropDown.value].text).endTime;
        }
        scheduleDropDown.name = shiftName;
    }
    public void UpdateShiftOptions(string setName)
    {
        scheduleDropDown.options.Add(new TMP_Dropdown.OptionData() { text = setName });
    }
    public void UpdateSetName(string oldName, string newName)
    {
        for (int i = 0; i < scheduleDropDown.options.Count; i++)
        {
            if (scheduleDropDown.options[i].text == oldName)
            {
                scheduleDropDown.options[i].text = newName;
                //shiftName = newName;
                if (scheduleDropDown.value == i)
                {
                    scheduleDropDown.onValueChanged.RemoveAllListeners();
                    scheduleDropDown.value = 0;
                    scheduleDropDown.value = i;
                    scheduleDropDown.onValueChanged.AddListener(delegate { ChangeWorkDay(); });
                }
                break;
            }
        }
    }
    private void DeleteMe(object sender, System.EventArgs e)
    {
        schCon.characterSchedules.Remove(this);
        TickSystem.Instance.On25Tick -= UpdateEmotion;
        unit.OnObjectiveValueChanged -= ChangeObjective;
        unit.OnTaskValueChanged -= UnitTaskChanged;
        unit.OnFired -= DeleteMe;
        Destroy(this.gameObject);
    }
    private void HideChecker() { gameObject.SetActive(SpecializedSorter.Instance.employeeShows[occupation]); }
    private void SetJobSprite()
    {
        transform.GetChild(5).GetChild(0).GetComponent<Image>().sprite = jobSprites[occupation - 1];
        transform.GetChild(0).GetChild(0).GetComponent<Image>().color = jobColors[occupation - 1];
    }
    private void UpdateEmotionColor()
    {
        float lerpValue = (float)unit.stress / 100f;
        Color newColor = Color.Lerp(Color.green, Color.red, lerpValue);
        newColor.a = 0.4f;
        transform.GetChild(1).GetChild(0).GetComponent<Image>().color = newColor;
    }
    public void Load(List<bool> workDayBools, string newShiftName)
    {
        for (int i = 0; i < workdays.Length; i++)
        {
            if (!workDayBools[i]) { workdays[i].Disable(); }
        }

        Debug.Log(newShiftName);
        int index = scheduleDropDown.options.FindIndex(option => option.text == newShiftName);
        scheduleDropDown.value = index;
    }
}
