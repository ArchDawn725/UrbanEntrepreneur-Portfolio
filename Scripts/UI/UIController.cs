using ArchDawn.Utilities;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Goal;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public class OnDayEventArgs : EventArgs { public string dayString; }

    public event EventHandler<OnDayEventArgs> OnDayValueChanged;
    public event EventHandler OnTimeValueChanged;
    public event EventHandler onStoreOpened;

    [Space(10)]
    [Header("Animators")]
    [SerializeField] private Animator uiTopAnimatior;
    [SerializeField] private Animator uiRightAnimatior;
    [SerializeField] private Animator uiBottomAnimatior;
    [SerializeField] private Animator uiMainAnimatior;

    [Space(10)]
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI moneyText;
    public TextMeshProUGUI moneyGainedText;
    public TextMeshProUGUI moneyLostText;
    [SerializeField] private TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI weekDayText;
    [SerializeField] private TextMeshProUGUI specialDayText;
    [SerializeField] private TextMeshProUGUI hiringTextParams;
    [SerializeField] private TextMeshProUGUI newAddText;
    [SerializeField] private TextMeshProUGUI newHiringWageText;
    [SerializeField] private TextMeshProUGUI newHiringSkillText;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI buildingNameDisplayText;
    [SerializeField] private TextMeshProUGUI buildingDiscDisplayText;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI selectEmplyeeNameDisplayText;
    [SerializeField] private TMP_Dropdown selectedEmployeeTask;
    [SerializeField] private TextMeshProUGUI selectEployeeWageText;
    [SerializeField] private TextMeshProUGUI selectEployeeStatusText;
    [SerializeField] private TextMeshProUGUI selectEployeeObjectiveText;
    [SerializeField] private TextMeshProUGUI selectEployeeDiscText;
    [SerializeField] private TextMeshProUGUI selectEployeeDisc2Text;
    [SerializeField] private TextMeshProUGUI selectEployeeSkillsText;
    [SerializeField] private TextMeshProUGUI selectEployeeTraitsText;
    [SerializeField] private TextMeshProUGUI selectBuildingNameDisplayText;
    [SerializeField] private TextMeshProUGUI selectBuildingDiscDisplayText;
    [SerializeField] private TextMeshProUGUI selectCustomerNameDisplayText;
    [SerializeField] private TextMeshProUGUI selectCustomerDiscDisplayText;
    [SerializeField] private TextMeshProUGUI selectCustomerMoneyText;
    [SerializeField] private Slider selectEmployeeStress;
    [SerializeField] private Slider selectEmployeeLongTermStress;
    [SerializeField] private PersonVisualCon selectedEmployeeCharacter;
    [SerializeField] private PersonVisualCon selectedCustomerCharacter;
    [Space(5)]
    [SerializeField] public TextMeshProUGUI resumeName;
    [SerializeField] private TextMeshProUGUI resumeApplyingFor;
    [SerializeField] private TextMeshProUGUI resumeWage;
    [SerializeField] private TextMeshProUGUI resumeInfo;
    [SerializeField] private TextMeshProUGUI resumeSkills;
    [SerializeField] private TextMeshProUGUI resumeCerts;
    [SerializeField] private TextMeshProUGUI resumeExperiance;
    [SerializeField] private TextMeshProUGUI resumeStrengths;
    [SerializeField] private TextMeshProUGUI resumeWeaknesses;
    [SerializeField] private PersonVisualCon resumeCharacter;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI outdoorTemp;
    [SerializeField] private TextMeshProUGUI indoorTemp;

    [Space(10)]
    [Header("Holders")]
    public Transform managerCharacterHolder;
    public Transform characterHolder;
    public Transform characterScheduleHolder;
    public Transform staffHolder;
    [SerializeField] private Transform hiringApplicantHolder;
    public Transform unitTaskHolder;
    [SerializeField] private Transform logHolder;
    [SerializeField] private Transform phoneMessageHolder;
    [SerializeField] private Transform orderHolder;
    [SerializeField] private Transform storageOptionsHolder;
    [SerializeField] private Transform customerItemsPrefsHolder;
    [SerializeField] private Transform customerItemsPrefsAnalysisHolder;
    [SerializeField] private Transform map;
    [SerializeField] private Transform eotmZone;
    [SerializeField] private Transform goalZone;

    [Space(10)]
    [Header("Test")]
    public int minutes;//15 min incriments
    public int hour;
    public int day = 1;
    public int month = 1;
    public int year = 2000;
    [SerializeField] private int dayMax = 30;//needs pattern to predict day max?
    [SerializeField] private Transform gameOverPop;
    [HideInInspector] public string typeOfDay;
    private bool selected;
    public int dayOfTheYear;
    public int playedYears;
    public int playedMonths;
    public int playedDays;

    private float hiringAdCost;
    private const float baseHiringFee = 25.00f;
    private float MoneyDisplayCurrent;
    public float MoneyLost;
    public float MoneyGained;
    public int maxCustomerReviews = 10;
    public int todaysReviews;

    [Space(10)]
    [Header("Spawns")]
    public GameObject uiCharacter;
    public GameObject uiCharacterSchedule;
    [SerializeField] private GameObject uiApplicant;
    public GameObject uiUnitTask;
    public GameObject staff;
    [SerializeField] private GameObject log;
    [SerializeField] private GameObject phoneMessage;
    [SerializeField] private GameObject uiOrder;
    [SerializeField] private GameObject storageOptions;
    [SerializeField] private GameObject customerItemPrefs;
    [SerializeField] private GameObject customerItemPrefsAnalysis;
    [SerializeField] private GameObject mapPin;
    [SerializeField] private GameObject eotm;
    [SerializeField] private GameObject goal;

    //Hidden
    [HideInInspector] public bool displayingBuildPopUP;
    public int hiringLevel; public int newHiringLevel = 0;
    public float hiringWage; public float newHiringWage = 7.25f;
    public int hiringOccupation; public int newHiringOccupation;
    public bool isHiring;
    public Employee2 selectedEmployee;
    public Customer2 selectedCustomer;
    [HideInInspector] public Building selectedBuilding;
    [HideInInspector] public Wall selectedWall;
    public bool gameOver;
    private float targetNightShade;
    public float myMarketShare;
    [HideInInspector] public bool movingBuilding;

    [Space(10)]
    [Header("Referances")]
    [SerializeField] public Slider hiringWageSlider;
    [SerializeField] public TMP_Dropdown hiringOccupationSelection;
    [SerializeField] private Transform selectPop;
    [SerializeField] private Button employeeFireButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button openStoreButton;
    public int stockTickets;
    private int firstBuildingSelectedItem;
    [SerializeField] private Transform mainMusic;
    [SerializeField] private SpriteRenderer nightShade;
    [SerializeField] RectMask2D orderMask;
    private float orderMaskTarget;
    [SerializeField] GameObject[] LoanButtons;
    [SerializeField] Button[] musicOptions;
    [SerializeField] Transform musicMenu;
    [SerializeField] PieChart pieChart;
    [SerializeField] public ToDoListManager toDoListManager;
    [SerializeField] SpriteRenderer background;
    [SerializeField] Sprite[] backgroundSprites;
    [SerializeField] PieChart customerPieChart;
    [HideInInspector] public StaffApplicant applicant;
    [SerializeField] private TMP_Dropdown hiringJobDropDown;
    public ScheduleController schCon;
    [SerializeField] private HiringAvailability hireAv;
    [SerializeField] private Button goodOfferButton;
    [SerializeField] private Button neutralOfferButton;
    [SerializeField] private Button badOfferButton;
    [SerializeField] private TextMeshProUGUI offerAmountText;
    [SerializeField] private CalanderController calander;
    [SerializeField] private EOTMController eotmCON;
    [SerializeField] private TMP_InputField storeNamer;
    [SerializeField] private MapPin myStorePin;
    [SerializeField] private List<Button> buildingColorButtons = new List<Button>();
    [SerializeField] private Image mapImage;
    [SerializeField] private Button moveBuldingButton;
    [SerializeField] private Button sellBuldingButton;

    [Space(10)]
    [Header("Bools")]
    public bool buildGridEnabled;
    public bool lightMapEnabled;
    public bool cleanMapEnabled;
    public bool beautyMapEnabled;

    [Space(10)]
    [Header("QuickButtons")]
    [SerializeField] private List<Button> QuickMenus = new List<Button>();
    [SerializeField] private List<Button> QuickHeatMaps = new List<Button>();
    [SerializeField] private Button menuButton;
    public List<Button> QuickTimeButtons = new List<Button>();
    [SerializeField] Localizer localizer;
    public string weekday = "Monday";
    [HideInInspector] public Advertising advertising;

    private void Awake() { Instance = this; }
    private void Start()
    {
        day = TransitionController.Instance.day;
        dayOfTheYear = TransitionController.Instance.dayOfTheYear;
        month = TransitionController.Instance.month;

        localizer = Localizer.Instance;
        Controller.Instance.FinishedLoading += Loaded;
        Controller.Instance.OnMoneyValueChanged += UpdateUI;
        MapController.Instance.OnObjectPlaced += BoughtBuilding;
        OnTimeValueChanged += OutsideDarkness;
        TickSystem.Instance.OnTimeTick += TimeChange;
        TickSystem.Instance.On5Tick += UpdateSelectDisplay;
        if (!TransitionController.Instance.loadGame) { year = TransitionController.Instance.year; DayChange(); hiringAdCost = (newHiringLevel * 10) + (newHiringWage * 5) + baseHiringFee; isHiring = true; }


        mapImage.sprite = TransitionController.Instance.mapSprite;
        weekday = TransitionController.Instance.weekday;
    }
    private void Loaded(object sender, System.EventArgs e) 
    {
        if (TransitionController.Instance.tutorialLevel == 1) { ToolTip.Instance.ActivateTutorial(1); }
        if (TransitionController.Instance.tutorialLevel == 2) { ToolTip.Instance.ActivateTutorial(32); }
        if (ToolTip.Instance.highestToolTipAchieved == 50) { ToolTip.Instance.ActivateTutorial(50); }
        if (ToolTip.Instance.highestToolTipAchieved == 63) { ToolTip.Instance.ActivateTutorial(63); }
        if (ToolTip.Instance.highestToolTipAchieved == 66) { ToolTip.Instance.ActivateTutorial(66); }
        if (TransitionController.Instance.tutorialLevel == 4) { ToolTip.Instance.ActivateTutorial(69); }
        if (TransitionController.Instance.tutorialLevel == 5) { ToolTip.Instance.ActivateTutorial(72); }
        if (ToolTip.Instance.highestToolTipAchieved == 75) { ToolTip.Instance.ActivateTutorial(75); }
        if (ToolTip.Instance.highestToolTipAchieved == 77) { ToolTip.Instance.ActivateTutorial(77); }
        if (ToolTip.Instance.highestToolTipAchieved == 80) { ToolTip.Instance.ActivateTutorial(80); }

        StartCoroutine(Controller.Instance.SetSpecialDays());
        ChangeBackGround();

        hiringJobDropDown.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("Any Occupation") });
        hiringJobDropDown.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("Stocker") });
        if (TransitionController.Instance.tutorialLevel > 1) { hiringJobDropDown.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("Cashier") }); }
        if (TransitionController.Instance.tutorialLevel > 2) { hiringJobDropDown.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("Janitor") }); }
        if (TransitionController.Instance.tutorialLevel > 3) { hiringJobDropDown.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("Engineer") }); }
        if (TransitionController.Instance.tutorialLevel > 4) { hiringJobDropDown.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("Manager") }); }

        selectedEmployeeTask.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("stocker") });
        if (TransitionController.Instance.tutorialLevel > 1) { selectedEmployeeTask.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("cashier") }); }
        if (TransitionController.Instance.tutorialLevel > 2) { selectedEmployeeTask.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("janitor") }); }
        if (TransitionController.Instance.tutorialLevel > 3) { selectedEmployeeTask.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("engineer") }); }
        if (TransitionController.Instance.tutorialLevel > 4) { selectedEmployeeTask.options.Add(new TMP_Dropdown.OptionData() { text = localizer.GetLocalizedText("manager") }); }

        hiringJobDropDown.value = 0;
        hiringJobDropDown.gameObject.SetActive(true);
        hiringJobDropDown.onValueChanged.AddListener(delegate { ChangeHiringOccupation(); });


        if (TransitionController.Instance.loadGame) { UpdateHiringDisplay(); }
        UIDeleter();
        weekDayText.text = weekday;
        weekDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(weekDayText.text);
        if (typeOfDay == "Regular") { specialDayText.text = localizer.GetLocalizedText(typeOfDay) + " " + localizer.GetLocalizedText(weekDayText.text); }
        else { specialDayText.text = typeOfDay; }
        specialDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText();
        if (TransitionController.Instance.loadGame) { fistDaySetUp = true; }
    }
    public void ChangeBackGround()
    {
        background.sprite = backgroundSprites[TransitionController.Instance.mapSelection - 1];
        background.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = backgroundSprites[TransitionController.Instance.mapSelection - 1];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { }//prints true/false
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitAllTopUI("None");
            ExitAllBottomUI("None");
            ExitAllMainUI("None");
            ExitAllRightUI("None");
            Controller.Instance.DeselectAll();
            mainMusic.GetChild(3).GetComponent<AudioSource>().Play();
            selectedEmployee = null;
        }
        //if (selected) { UpdateSelectDisplay(); } //use as event later

        uiTopAnimatior.speed = TickSystem.Instance.adjustedTimeSpeed;
        uiBottomAnimatior.speed = TickSystem.Instance.adjustedTimeSpeed;
        uiMainAnimatior.speed = TickSystem.Instance.adjustedTimeSpeed;

        if (!Controller.Instance.storeOpened) { OpenChecker(); }

        float fadeTime;
        if (TickSystem.Instance.timeMultiplier == 0) { fadeTime = 0; }
        else { fadeTime = 0.001f * TickSystem.Instance.timeMultiplier; }

        // Calculate the alpha value based on the elapsed time and fade time
        float alpha = Mathf.Lerp(nightShade.material.color.a, targetNightShade, fadeTime);

        // Set the child object's alpha value
        Color objectColor = nightShade.material.color;
        objectColor.a = alpha;
        nightShade.material.color = objectColor;

        if (!IsAnyInputFieldFocused() && SaveController.Instance.finishedLoading) { HotKeys(); }
            


        Vector4 vector4 = orderMask.padding;
        vector4.x = Mathf.Lerp(vector4.x, orderMaskTarget, (Time.deltaTime / 0.05f) * TickSystem.Instance.adjustedTimeSpeed);
        orderMask.padding = vector4;

        if (moneyChanged)
        {
            if (Controller.Instance.money + 0.1f > MoneyDisplayCurrent && MoneyDisplayCurrent > Controller.Instance.money) { MoneyDisplayCurrent = Controller.Instance.money; moneyChanged = false; moneyText.color = Color.white; }
            else if (Controller.Instance.money - 0.1f < MoneyDisplayCurrent && MoneyDisplayCurrent < Controller.Instance.money) { MoneyDisplayCurrent = Controller.Instance.money; moneyChanged = false; moneyText.color = Color.white; }
            else
            {
                if (MoneyDisplayCurrent > Controller.Instance.money) { moneyText.color = Color.red; }
                if (MoneyDisplayCurrent < Controller.Instance.money) { moneyText.color = Color.green; }
                MoneyDisplayCurrent = Mathf.Lerp(MoneyDisplayCurrent, Controller.Instance.money, (Time.deltaTime / 0.11f) * TickSystem.Instance.adjustedTimeSpeed);
            }

            moneyText.text = "$" + MoneyDisplayCurrent.ToString("f2");
        }
    }

    private bool IsAnyInputFieldFocused()
    {
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject != null)
        {
            TMP_InputField inputField = selectedGameObject.GetComponent<TMP_InputField>();
            return inputField != null && inputField.isFocused;
        }
        return false;
    }
    private void HotKeys()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) && QuickHeatMaps[0] != null) { QuickHeatMaps[0].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad1) && QuickHeatMaps[1] != null) { QuickHeatMaps[1].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad2) && QuickHeatMaps[2] != null) { QuickHeatMaps[2].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad3) && QuickHeatMaps[3] != null) { QuickHeatMaps[3].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad4) && QuickHeatMaps[4] != null) { QuickHeatMaps[4].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad5) && QuickHeatMaps[5] != null) { QuickHeatMaps[5].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad6) && QuickHeatMaps[6] != null) { QuickHeatMaps[6].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad7) && QuickHeatMaps[7] != null) { QuickHeatMaps[7].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad8) && QuickHeatMaps[8] != null) { QuickHeatMaps[8].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Keypad9) && QuickHeatMaps[9] != null) { QuickHeatMaps[9].onClick.Invoke(); }

        if (Input.GetKeyDown(KeyCode.Alpha0) && QuickMenus[0] != null && QuickMenus[0].transform.parent.gameObject.activeSelf) { QuickMenus[0].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha1) && QuickMenus[1] != null && QuickMenus[1].transform.parent.gameObject.activeSelf) { QuickMenus[1].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha2) && QuickMenus[2] != null && QuickMenus[2].transform.parent.gameObject.activeSelf) { QuickMenus[2].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha3) && QuickMenus[3] != null && QuickMenus[3].transform.parent.gameObject.activeSelf) { QuickMenus[3].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha4) && QuickMenus[4] != null && QuickMenus[4].transform.parent.gameObject.activeSelf) { QuickMenus[4].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha5) && QuickMenus[5] != null && QuickMenus[5].transform.parent.gameObject.activeSelf) { QuickMenus[5].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha6) && QuickMenus[6] != null && QuickMenus[6].transform.parent.gameObject.activeSelf) { QuickMenus[6].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha7) && QuickMenus[7] != null && QuickMenus[7].transform.parent.gameObject.activeSelf) { QuickMenus[7].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha8) && QuickMenus[8] != null && QuickMenus[8].transform.parent.gameObject.activeSelf) { QuickMenus[8].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Alpha9) && QuickMenus[9] != null && QuickMenus[9].transform.parent.gameObject.activeSelf) { QuickMenus[9].onClick.Invoke(); }

        if (Input.GetKeyDown(KeyCode.BackQuote)) { menuButton.onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Space)) { if (Controller.Instance.storeOpened) { if (!TickSystem.Instance.paused) { TickSystem.Instance.PauseMenu(); } else { TickSystem.Instance.UnPause(); } } }

        if (Input.GetKeyDown(KeyCode.F1) && QuickTimeButtons[0].interactable == true) { QuickTimeButtons[0].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.F2) && QuickTimeButtons[1].interactable == true) { QuickTimeButtons[1].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.F3) && QuickTimeButtons[2].interactable == true) { QuickTimeButtons[2].onClick.Invoke(); }
        if (Input.GetKeyDown(KeyCode.F4) && QuickTimeButtons[3].interactable == true) { QuickTimeButtons[3].onClick.Invoke(); }

        if (Input.GetKeyDown(KeyCode.Y)) { SetSubMenu(0); }
        if (Input.GetKeyDown(KeyCode.R)) { SetSubMenu(1); }
        if (Input.GetKeyDown(KeyCode.T)) { SetSubMenu(2); }
        if (Input.GetKeyDown(KeyCode.U)) { SetSubMenu(4); }

        if (Input.GetMouseButtonDown(1)) { ExitAllTopUI(""); ExitAllRightUI(""); if (!movingBuilding) { MapController.Instance.DeselectObjectType(); } }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //first of options in menus
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //other tabs on activated ui
        }
    }
    private bool moneyChanged;
    private void UpdateUI(object sender, System.EventArgs e)
    {
        float money = (float)Controller.Instance.money;
        moneyChanged = true;
        if (Controller.Instance.money <= -25000 / TransitionController.Instance.totalDifficulty) { GameOver(false, "You lost... You have too much debt to stay open", false); }
    }

    public void SetReset(bool ifReset) { uiBottomAnimatior.SetBool("NoReset", ifReset); }

    private void ExitAllTopUI(string exeption)
    {
        if (exeption != "ToEmployee") { uiTopAnimatior.SetBool("ToEmployee", false); }
        if (exeption != "ToStore") { uiTopAnimatior.SetBool("ToStore", false); }
        if (exeption != "ToFinance") { uiTopAnimatior.SetBool("ToFinance", false); }
        if (exeption != "ToHire") { uiTopAnimatior.SetBool("ToHire", false); }
        if (exeption != "ToAutoTask") { uiTopAnimatior.SetBool("ToAutoTask", false); }
        uiTopAnimatior.SetInteger("SubMenu", 0);
        if (ToolTip.Instance.highestToolTipAchieved == 34) { ToolTip.Instance.ActivateTutorial(35); }
        else if (ToolTip.Instance.highestToolTipAchieved > 42) { ToolTip.Instance.ActivateTutorial(47); }
    }

    public void SetTopAnimatorString(string set)
    {
        if (uiTopAnimatior.GetBool(set) == false)
        {
            ExitAllTopUI(set); ExitAllMainUI(set);
            uiTopAnimatior.SetBool(set, true);

            if (set == "ToEmployee") { ToolTip.Instance.DismissTutorial(16); ToolTip.Instance.DismissTutorial(22); }//ToolTip.Instance.ActivateTutorial(11); }
            if (set == "ToStore") { ToolTip.Instance.DismissTutorial(53); ToolTip.Instance.ActivateTutorial(54); }//ToolTip.Instance.ActivateTutorial(12); }
            if (set == "ToFinance" && ToolTip.Instance.highestToolTipAchieved > 42) { ToolTip.Instance.DismissTutorial(43); ToolTip.Instance.DismissTutorial(33); ToolTip.Instance.ActivateTutorial(44); }//ToolTip.Instance.ActivateTutorial(13); }
            else if (set == "ToFinance") { ToolTip.Instance.DismissTutorial(43); ToolTip.Instance.DismissTutorial(33); ToolTip.Instance.ActivateTutorial(34); }//ToolTip.Instance.ActivateTutorial(13); }
            if (set == "ToHire") { ToolTip.Instance.DismissTutorial(17); ToolTip.Instance.ActivateTutorial(18); }//ToolTip.Instance.ActivateTutorial(13); }
        }
        else { ExitAllTopUI("None"); ExitAllMainUI("None"); }
    }
    private void ExitAllRightUI(string exeption)
    {
        if (exeption != "ToEmployee") { uiRightAnimatior.SetBool("ToEmployee", false); }
        uiRightAnimatior.SetInteger("SubMenu", 0);
        if (ToolTip.Instance.highestToolTipAchieved > 23) { ToolTip.Instance.ActivateTutorial(26); }
    }

    public void SetRightAnimatorString(string set)
    {
        if (uiRightAnimatior.GetBool(set) == false)
        {
            ExitAllRightUI(set); ExitAllMainUI(set);
            uiRightAnimatior.SetBool(set, true);

            if (set == "ToEmployee" && ToolTip.Instance.highestToolTipAchieved > 17) { ToolTip.Instance.ActivateTutorial(23); }
            else if (set == "ToEmployee") { ToolTip.Instance.ActivateTutorial(17); }

        }
        else { ExitAllRightUI("None"); ExitAllMainUI("None"); }
    }

    private void ExitAllBottomUI(string exeption)
    {
        if (!movingBuilding) { MapController.Instance.DeselectObjectType(); }
        
        if (uiBottomAnimatior.GetBool("Select")) { Controller.Instance.DeselectAll2(); }

        //if (exeption != "ToBuild") { uiBottomAnimatior.SetBool("ToBuild", false); }
        if (exeption != "ToHire" && exeption != "ToAd") { uiBottomAnimatior.SetBool("ToHire", false); }
        if (exeption != "ToAd") { uiBottomAnimatior.SetBool("ToAd", false); }
        if (exeption != "Select") { DeSelectPopUp(); }
        if (exeption != "ToBuild" && exeption != "ToSelectBuild" && exeption != "BuildHide") { uiBottomAnimatior.SetBool("ToBuild", false); }
        if (exeption != "ToSelectBuild") { uiBottomAnimatior.SetBool("ToSelectBuild", false); }
        if (exeption != "ToResume") { uiBottomAnimatior.SetBool("ToResume", false); }
        if (exeption != "BuildHide") { if (uiBottomAnimatior.GetBool("BuildHide")) { uiBottomAnimatior.SetBool("ToBuild", true); } uiBottomAnimatior.SetBool("BuildHide", false); }
    }

    public void SetBottomAnimatorString(string set)
    {
        if (set != "BuildHide")
        {
            if (uiBottomAnimatior.GetBool(set) == false || set == "Select" || set == "ToResume" || uiBottomAnimatior.GetBool("NoReset") == true)
            {
                uiBottomAnimatior.SetBool("NoReset", false);
                if (uiBottomAnimatior.GetBool("Select")) { uiBottomAnimatior.SetTrigger("Reset"); }
                if (uiBottomAnimatior.GetBool("ToResume")) { uiBottomAnimatior.SetTrigger("Reset"); }
                ExitAllBottomUI(set); ExitAllMainUI(set);
                if (set != "") { uiBottomAnimatior.SetBool(set, true); }

                if (set == "ToBuild") { if (ToolTip.Instance.highestToolTipAchieved > 50) { ToolTip.Instance.ActivateTutorial(57); } }//ToolTip.Instance.ActivateTutorial(15); }
                if (set == "ToHire") { }//ToolTip.Instance.ActivateTutorial(16); }
                if (set == "ToAd") { }//ToolTip.Instance.ActivateTutorial(17); }
            }
            else { ExitAllBottomUI("None"); ExitAllMainUI("None"); }
        }
        else if (uiBottomAnimatior.GetBool("BuildHide")) { uiBottomAnimatior.SetBool("BuildHide", false); }
        else { uiBottomAnimatior.SetBool("BuildHide", true); }
    }
    private bool uiBreak; private bool EOTMcalled;
    private void ExitAllMainUI(string exeption)
    {
        if (exeption != "ToOrder") { uiMainAnimatior.SetBool("ToOrder", false); }
        if (exeption != "MainMenu") { uiMainAnimatior.SetBool("MainMenu", false); }
        if (exeption != "ToMap") { uiMainAnimatior.SetBool("ToMap", false); }
        if (exeption == "CloseEOTM") 
        { 
            uiMainAnimatior.SetBool("ToEmployeeOfTheMonth", false);
            if (month == 12 && uiBreak) { EmployeeOfTheMonth(2); uiBreak = false; }
            if (month % 3 == 0 && !uiBreak && !EOTMcalled) { EmployeeOfTheMonth(1); uiBreak = true; EOTMcalled = true; }
        }
        if (exeption == "CloseOffer") { uiMainAnimatior.SetBool("ToSellOffer", false); }
        if (exeption == "CloseWin") { uiMainAnimatior.SetBool("ToWin", false); }
        if (exeption != "ToCalander") { uiMainAnimatior.SetBool("ToCalander", false); }
        //if (TickSystem.Instance.paused) { TickSystem.Instance.UnPause(); }
        if (ToolTip.Instance.highestToolTipAchieved > 35 && ToolTip.Instance.highestToolTipAchieved < 50) { ToolTip.Instance.ActivateTutorial(38); }
    }

    public void SetMainAnimatorString(string set)
    {
        if (uiMainAnimatior.GetBool(set) == false)
        {
            ExitAllMainUI(set); ExitAllBottomUI(set); ExitAllTopUI(set); ExitAllRightUI(set);
            uiMainAnimatior.SetBool(set, true);

            if (set == "ToOrder") { ToolTip.Instance.DismissTutorial(13); ToolTip.Instance.ActivateTutorial(14); }//ToolTip.Instance.ActivateTutorial(8); }
            if (set == "MainMenu") { ToolTip.Instance.DismissTutorial();}// ToolTip.Instance.ActivateTutorial(10); }
            if (set == "ToMap") { ToolTip.Instance.DismissTutorial(35); ToolTip.Instance.ActivateTutorial(36); }
        }
        else { ExitAllMainUI("None"); ExitAllBottomUI("None"); ExitAllTopUI("None"); ExitAllRightUI("None"); }
    }
    public void SetSubMenu(int set)
    {
        if (uiTopAnimatior.GetInteger("SubMenu") != set)
        {
            uiTopAnimatior.SetInteger("SubMenu", 0);
            uiTopAnimatior.SetInteger("SubMenu", set);
            if (uiTopAnimatior.GetBool("ToFinance"))
            {
                if (set == 1) { ToolTip.Instance.DismissTutorial(44); ToolTip.Instance.ActivateTutorial(45); }
                if (set == 2) { ToolTip.Instance.DismissTutorial(44); ToolTip.Instance.ActivateTutorial(46); }
            }
            if (uiTopAnimatior.GetBool("ToStore"))
            {
                if (set == 1) { ToolTip.Instance.ActivateTutorial(56); }
                if (set == 2) { ToolTip.Instance.ActivateTutorial(55); }
            }

        }
        else { uiTopAnimatior.SetInteger("SubMenu", 0); }
    }
    private bool fistDaySetUp;
    private void TimeChange(object sender, TickSystem.OnTickEventArgs e)
    {
        if (minutes < 45) { minutes += 15; }
        else { hour++; minutes = 0; }
        if (hour > 23) { hour = 0; day++; }
        if (day > dayMax) { day = 1; month++; playedMonths++; }
        if (month > 12) { month = 1; year++; if (!fistDaySetUp) { playedYears++; } Controller.Instance.NewYear(); }
        if (hour == 0 && minutes == 0) { if (!fistDaySetUp) { fistDaySetUp = true; } else { DayChange(); } }

        string hourString = ""; string minuteString = "";
        if (minutes < 10) { minuteString = "0" + minutes.ToString(); } else { minuteString = minutes.ToString(); }
        if (hour < 10) { hourString = "0" + hour.ToString(); } else { hourString = hour.ToString(); }

        timeText.text = hourString + ":" + minuteString;
        OnTimeValueChanged?.Invoke(this, EventArgs.Empty);
        //if (hour % 2 == 0 && minutes == 0) { MapController.Instance.ResetFloorClaims(); }
    }

    private void DayChange()
    {
        MapController.Instance.ResetFloorClaims();
        foreach(Employee2 employee in Controller.Instance.employees) { employee.RemoveAllClaims(); }
        //Controller.Instance.dailyMoneyLostProduct = 0; Controller.Instance.dailyMoneyLostWages = 0;
        string monthString = ""; string dayString = "";
        if (day < 10) { dayString = "0" + day.ToString(); } else { dayString = day.ToString(); }
        if (month < 10) { monthString = "0" + month.ToString(); } else { monthString = month.ToString(); }

        dateText.text = monthString + "/" + dayString + "/" + year.ToString();

        switch(weekday)
        {
            case "": weekDayText.text = "Monday"; weekday = "Monday"; break;
            case "Sunday": weekDayText.text = "Monday"; weekday = "Monday"; break;
            case "Monday": weekDayText.text = "Tuesday"; weekday = "Tuesday"; break;
            case "Tuesday": weekDayText.text = "Wednesday"; weekday = "Wednesday"; break;
            case "Wednesday": weekDayText.text = "Thursday"; weekday = "Thursday"; break;
            case "Thursday": weekDayText.text = "Friday"; weekday = "Friday"; break;
            case "Friday": weekDayText.text = "Saturday"; weekday = "Saturday"; break;
            case "Saturday": weekDayText.text = "Sunday"; weekday = "Sunday"; break;
        }
        weekDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(weekDayText.text);
        if (!Controller.Instance.storeOpenDays.ContainsKey(weekday)) { Controller.Instance.storeOpenDays.Add(weekday, true); }

        switch (month)
        {
            case 1: dayMax = 31; break;
            case 2: dayMax = 28; break;
            case 3: dayMax = 31; break;
            case 4: dayMax = 30; break;
            case 5: dayMax = 31; break;
            case 6: dayMax = 30; break;
            case 7: dayMax = 31; break;
            case 8: dayMax = 31; break;
            case 9: dayMax = 30; break;
            case 10: dayMax = 31; break;
            case 11: dayMax = 30; break;
            case 12: dayMax = 31; break;
        }

        dayOfTheYear++; if (dayOfTheYear > 365) { dayOfTheYear = 1; }
        StartCoroutine(Controller.Instance.GetSpecialDays(dayOfTheYear));

        /*
        if (typeOfDay == "Regular") { specialDayText.text = localizer.GetLocalizedText(typeOfDay) + " " + localizer.GetLocalizedText(weekDayText.text); specialDayText.color = Color.black; }
        else { specialDayText.text = typeOfDay; }
        specialDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText();
        Debug.Log(specialDayText.text);
        */

        OnDayValueChanged?.Invoke(this, new OnDayEventArgs { dayString = weekday });
        if (day == dayMax && playedDays > 0) { EOTMcalled = false; EmployeeOfTheMonth(0); }
        playedDays++;
        if (playedDays > TransitionController.Instance.BuyoutStartDate && !gameOver) { StartCoroutine(BuyoutAttempt()); }
        todaysReviews = 0;
        if (day == 1) { StartCoroutine(UpdateCalander()); }
        if (playedDays > 1) { RandomQuestChance(); }
    }
    private IEnumerator UpdateCalander()
    {
        List<int> dates = new List<int>();
        List<string> holidays = new List<string>();
        for (int i = 0; i <= dayMax; i++) { holidays.Add(""); }
        foreach (KeyValuePair<string, int> pair in Controller.Instance.specialYearlyDays)
        {
            if (pair.Value <= dayOfTheYear + dayMax && pair.Value >= dayOfTheYear) 
            { 
                holidays[pair.Value - dayOfTheYear + 1] = pair.Key;
            }
            yield return new WaitForEndOfFrame();
        }

        int firstDay = 0;
        switch (weekday)
        {
            case "Sunday": firstDay = 0; break;
            case "Monday": firstDay = 1; break;
            case "Tuesday": firstDay = 2; break;
            case "Wednesday": firstDay = 3; break;
            case "Thursday": firstDay = 4; break;
            case "Friday": firstDay = 5; break;
            case "Saturday": firstDay = 6; break;
        }

        int date = 1;
        List<string> holidays2 = new List<string>();
        List<string> weatherList = new List<string>();
        for (int i = 0; i < 42; i++)
        {
            if (i < firstDay) { dates.Add(0); holidays2.Add(""); weatherList.Add(""); }
            else if (i <= dayMax) { dates.Add(date); holidays2.Add(holidays[date]); weatherList.Add(Controller.Instance.thisMonthWeather[date]); date++; }
            else { dates.Add(0); holidays2.Add(""); weatherList.Add(""); }
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(calander.SetDates(dates, holidays2, weatherList));
    }
    public void ChangeSpecialDayDisplay(string dayName, Color color) 
    { 
        specialDayText.text = dayName;
        specialDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(dayName);
        specialDayText.color = color;
    }
    public void GetGainsAndLosses()
    {
        moneyGainedText.text = "+$" + MoneyGained.ToString("f2");
        MoneyGained = 0;
        moneyLostText.text = "-$" + MoneyLost.ToString("f2");
        MoneyLost = 0;
    }
    public void LoadedDay()
    {
        string monthString = ""; string dayString = "";
        if (day < 10) { dayString = "0" + day.ToString(); } else { dayString = day.ToString(); }
        if (month < 10) { monthString = "0" + month.ToString(); } else { monthString = month.ToString(); }

        dateText.text = monthString + "/" + dayString + "/" + year.ToString();

        switch (month)
        {
            case 1: dayMax = 31; break;
            case 2: dayMax = 28; break;
            case 3: dayMax = 31; break;
            case 4: dayMax = 30; break;
            case 5: dayMax = 31; break;
            case 6: dayMax = 30; break;
            case 7: dayMax = 31; break;
            case 8: dayMax = 31; break;
            case 9: dayMax = 30; break;
            case 10: dayMax = 31; break;
            case 11: dayMax = 30; break;
            case 12: dayMax = 31; break;
        }

        if (typeOfDay == "Regular") { specialDayText.text = localizer.GetLocalizedText(typeOfDay) + " " + localizer.GetLocalizedText(weekDayText.text); specialDayText.color = Color.black; }
        else { specialDayText.text = typeOfDay; }
        specialDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText();
        Debug.Log(specialDayText.text);

        string hourString = ""; string minuteString = "";
        if (minutes < 10) { minuteString = "0" + minutes.ToString(); } else { minuteString = minutes.ToString(); }
        if (hour < 10) { hourString = "0" + hour.ToString(); } else { hourString = hour.ToString(); }

        timeText.text = hourString + ":" + minuteString;
        weekDayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(weekDayText.text);
        if (!Controller.Instance.storeOpenDays.ContainsKey(weekday)) { Controller.Instance.storeOpenDays.Add(weekday, true); }
        //UpdateLoanDisplay();
    }
    public void UpdateBuildingDisplay(BuildButton buildButton, string tileName)
    {
        string buildingNameDisplay = "";
        string buildingDiscDisplay = "";
        if (buildButton.toBuild != null)
        {
            BuildingSO building = buildButton.toBuild;
            buildingNameDisplay = building.buildingName;

            string storageSize = "";
            if (building.type != BuildingSO.Type.register) { storageSize = localizer.GetLocalizedText("Storage Capacity: ") + building.itemCapacity; }

            buildingDiscDisplay =
                localizer.GetLocalizedText("Cost: ") + (building.cost).ToString("f2") + "$" + System.Environment.NewLine +
                localizer.GetLocalizedText("Electricity $/Hour: ") + (building.electricityCost).ToString("f2") + "$" + System.Environment.NewLine +
                localizer.GetLocalizedText("Size: ") + building.width.ToString() + "x" + building.height.ToString() + System.Environment.NewLine +
                storageSize + System.Environment.NewLine +
                localizer.GetLocalizedText(building.myDiscription)
                ;
        }
        else if (tileName != null)
        {
            buildingNameDisplay = tileName;
            buildingDiscDisplay =
                localizer.GetLocalizedText("Cost: ") + (MapController.Instance.floorTypes[tileName][4]).ToString("f2") + "$" + System.Environment.NewLine +
                localizer.GetLocalizedText("Beauty: ") + (MapController.Instance.floorTypes[tileName][1]).ToString() + System.Environment.NewLine +
                localizer.GetLocalizedText("Floor tiles are used to make the store more beautiful.")
                ;
        }
        UpdateBuildingDisplay(buildingNameDisplay, buildingDiscDisplay);
    }
    public void UpdateBuildingDisplay(string name, string disc)
    {
        buildingNameDisplayText.text = name;
        buildingNameDisplayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(name);
        buildingDiscDisplayText.text = disc;
        buildingDiscDisplayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(disc);
    }
    private void BoughtBuilding(object sender, System.EventArgs e)
    {
        if (MapController.Instance.placingBuilding != null && !movingBuilding)
        {
            Controller.Instance.MoneyValueChange(-MapController.Instance.placingBuilding.cost * Controller.Instance.inflationAmount, UtilsClass.GetMouseWorldPosition(), true, false);

            float newValue = (MapController.Instance.placingBuilding.cost * Controller.Instance.inflationAmount) * ((TransitionController.Instance.tax) + 1);
            UIController.Instance.MoneyLost -= newValue;
        }

        if (movingBuilding)
        {
            MapController.Instance.DeselectObjectType();
            movingBuilding = false;
        }
    }
    public void PlacingMovedBuilding(Building building) { buildMover.PlacedBuilding(building); }
    public void OpenStore()
    {
        onStoreOpened?.Invoke(this, EventArgs.Empty);
        Controller.Instance.storeOpened = true;
        playButton.onClick.Invoke();
        if (toDoListManager != null) { toDoListManager.Done(); }
        //gamertag?

        bool female = false;
        if (Random.Range(0, 100) > 51) { female = true; }
        string birthName = Names.Instance.GetName(female);

        Employee2 spawnedEmployee = Employee2.Create(Controller.Instance.employeeSO, 1, 1, 1, 1, 1, 0, true, 35, birthName, 0, female, null, false);
        if (TransitionController.Instance.tutorialLevel == 2) { ToolTip.Instance.ActivateTutorial(40); }
    }
    public void LoadOpenStore()
    {
        onStoreOpened?.Invoke(this, EventArgs.Empty);
        Controller.Instance.storeOpened = true;
        playButton.interactable = true;
        if (toDoListManager != null) { toDoListManager.Done(); }
    }
    public void UpdateHiring()
    {
        //daily fee?
        Controller.Instance.MoneyValueChange(-hiringAdCost - baseHiringFee, UtilsClass.GetMouseWorldPosition(), true, false);
        paidAdCosts = hiringAdCost;
        UpdateHiringDisplay();
    }
    public void UpdateHiringDisplay()
    {
        hiringLevel = newHiringLevel;
        hiringWage = newHiringWage;
        hiringOccupation = newHiringOccupation;
        String toHireOccupation = null;
        switch (hiringOccupation)
        {
            case 0: toHireOccupation = "Any Occupation"; break;
            case 1: toHireOccupation = "Stocker"; break;
            case 2: toHireOccupation = "Cashier"; break;
            case 3: toHireOccupation = "Janitor"; break;
            case 4: toHireOccupation = "Engineer"; break;
            case 5: toHireOccupation = "Manager"; break;
        }
        hiringTextParams.text = hiringWage.ToString("f2") + localizer.GetLocalizedText("$ level-") + hiringLevel.ToString() + " " + localizer.GetLocalizedText(toHireOccupation);
        //ChangeHiringLevel(0);//training can decrease this to 0
        //hiringOccupationSelection.value = 0;
        //ChangeHiringOccupation();
        //hiringWageSlider.value = 7.25f;
        //ChangeHiringWage();
        isHiring = true;
        //ToolTip.Instance.ActivateTutorial(6);
        if (toDoListManager != null) { toDoListManager.CheckOff(5); }
    }
    public void ChangeHiringLevel(int value)
    {
        newHiringLevel += value;
        if (newHiringLevel < 0) { newHiringLevel = 0; } //can go to 0 with training
        if (newHiringLevel > 20) { newHiringLevel = 20; }
        newHiringSkillText.text = newHiringLevel.ToString();

        hiringAdCost = (newHiringLevel * 10) + (newHiringWage * 5) + baseHiringFee;//occupations need to cost different
        newAddText.text = localizer.GetLocalizedText("Post Ad: $") + (hiringAdCost + baseHiringFee).ToString("f2");
        hireAv.UpdateAvailability();
    }
    public void ChangeHiringOccupation()
    {
        newHiringOccupation = hiringOccupationSelection.value;

        hiringAdCost = (newHiringLevel * 10) + (newHiringWage * 5) + baseHiringFee;//occupations need to cost different
        newAddText.text = localizer.GetLocalizedText("Post Ad: $") + (hiringAdCost + baseHiringFee).ToString("f2");
        hireAv.UpdateAvailability();
    }
    public void ChangeHiringWage()
    {
        //newHiringWage = hiringWageSlider.value / 100;
        newHiringWage = hiringWageSlider.value * 0.25f;
        newHiringWageText.text = newHiringWage.ToString("f2") + "$";

        hiringAdCost = (newHiringLevel * 10) + (newHiringWage * 5) + baseHiringFee;//occupations need to cost different
        newAddText.text = localizer.GetLocalizedText("Post Ad: $") + (hiringAdCost + baseHiringFee).ToString("f2");
        hireAv.UpdateAvailability();
    }
    private float paidAdCosts;
    public void HiringFees()
    {
        if (isHiring)
        {
            float mon = paidAdCosts / 10;
            if (mon > 0)
            {
                CreateLog(0, localizer.GetLocalizedText("Hiring Fees: ") + mon.ToString("f2"), "Accountant", 1);
                Controller.Instance.MoneyValueChange(-mon, UtilsClass.GetMouseWorldPosition(), true, false);
            }
        }
    }
    public void StopHiring() { isHiring = false; hiringTextParams.text = localizer.GetLocalizedText("Place a hiring ad"); }
    public StaffApplicant NewApplicent(Customer2 customer)
    {
        GameObject app = Instantiate(uiApplicant, hiringApplicantHolder);
        StaffApplicant applicant = app.GetComponent<StaffApplicant>();

        applicant.applicant = customer;
        applicant.StartUp();
        return applicant;
    }
    private void OutsideDarkness(object sender, System.EventArgs e)
    {
        int x = hour; // Replace with any value between 0 and 24
        float y = 1;

        switch (x)
        {
            case 0: y = 0.8f; break;
            case 1: y = 0.8f; break;
            case 2: y = 0.7f; break;
            case 3: y = 0.6f; break;
            case 4: y = 0.5f; break;
            case 5: y = 0.4f; break;
            case 6: y = 0.3f; break;
            case 7: y = 0.2f; break;
            case 8: y = 0.1f; break;
            case 9: y = 0; break;
            case 10: y = 0; break;
            case 11: y = 0; break;
            case 12: y = 0; break;
            case 13: y = 0; break;
            case 14: y = 0; break;
            case 15: y = 0; break;
            case 16: y = 0.1f; break;
            case 17: y = 0.2f; break;
            case 18: y = 0.3f; break;
            case 19: y = 0.4f; break;
            case 20: y = 0.5f; break;
            case 21: y = 0.6f; break;
            case 22: y = 0.7f; break;
            case 23: y = 0.8f; break;
            case 24: y = 0.8f; break;
        }

        targetNightShade = y;
    }
    public void NewSelectPopUp(GameObject obj)
    {
        if (obj != null)
        {
            if (MapController.Instance.placingBuilding == null)
            {
                DeActivateAll();
                Employee2 thisEmployee;
                Building thisBuilding;
                Customer2 thisCustomer;
                Wall thisWall;
                if (obj.TryGetComponent(out thisEmployee))
                {
                    selectPop.GetChild(1).gameObject.SetActive(true);
                    EmployeeSelect(thisEmployee);
                }
                if (obj.TryGetComponent(out thisBuilding))
                {
                    selectPop.GetChild(2).gameObject.SetActive(true);
                    BuildingSelect(thisBuilding);
                }
                if (obj.TryGetComponent(out thisCustomer))
                {
                    selectPop.GetChild(3).gameObject.SetActive(true);
                    CustomerSelect(thisCustomer);
                }
                if (obj.TryGetComponent(out thisWall))
                {
                    selectPop.GetChild(4).gameObject.SetActive(true);
                    WallSelect(thisWall);
                }
                SetBottomAnimatorString("Select");
            }
        }
        else { SetBottomAnimatorString(""); }
    }
    public void NewMulipleSelectPopUp(GameObject obj)//
    {
        if (!uiBottomAnimatior.GetBool("Select"))
        {
            //new pop up


            //Employee2 thisEmployee;
            Building thisBuilding;
            //Customer thisCustomer;
            /*
            if (obj.TryGetComponent(out thisEmployee))
            {
                selectPop.GetChild(0).gameObject.SetActive(true);
                EmployeeSelect(thisEmployee);
            }*/
            if (obj.TryGetComponent(out thisBuilding))
            {
                DeActivateAll();
                selectPop.GetChild(2).gameObject.SetActive(true);
                MultiBuildingSelect();
                //BuildingSelect(thisBuilding);
            }
            /*
            if (obj.TryGetComponent(out thisCustomer))
            {
                selectPop.GetChild(2).gameObject.SetActive(true);
                CustomerSelect(thisCustomer);
            }*/
            SetBottomAnimatorString("Select");
        }
        else
        {
            //keep current pop
            selectPop.GetChild(2).gameObject.SetActive(true);
            MultiBuildingSelect();
        }
    }
    private void EmployeeSelect(Employee2 thisEmployee)
    {
        selectEmplyeeNameDisplayText.text = thisEmployee.birthName;
        thisEmployee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);

        List<string> traits = new List<string>();
        foreach (KeyValuePair<string, bool> pair in thisEmployee.traits) { if (pair.Value == true) { traits.Add(localizer.GetLocalizedText(pair.Key)); } }
        List<string> newTraits = RemoveUselessEmployeeTraits(traits);
        string traitString = string.Join(Environment.NewLine, newTraits);
        string statusString = "";

        int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
        if (time >= thisEmployee.workStart && time < thisEmployee.workEnd)
        {
            if (thisEmployee.animator.GetBool("OnShift"))
            {
                statusString = "Working";
            }
            else
            {
                statusString = "Shift changed, will come in tomorrow";
                if (thisEmployee.workDays[UIController.Instance.weekDayText.text] == false) { statusString = "Day off"; }
                if (thisEmployee.late) { statusString = "Late"; }
                if (thisEmployee.calledOut) { statusString = "Called out"; }
                if (thisEmployee.sentHome) { statusString = "Sent home"; }
            }
        }
        else { statusString = "Off duty"; }

        //selectEployeeWageText.text = thisEmployee.hourlyWage.ToString("f2") + "$";
        selectEployeeObjectiveText.text = thisEmployee.objective.ToString();
        selectEployeeStatusText.text = statusString;
        selectEployeeObjectiveText.GetComponent<AutoLocalizer>().UpdateLocalizedText(statusString);
        selectEployeeStatusText.GetComponent<AutoLocalizer>().UpdateLocalizedText(statusString);
        string newTask = Localizer.Instance.GetLocalizedText(thisEmployee.task.ToString());
        selectedEmployeeTask.onValueChanged.RemoveAllListeners();

        for (int i = 0; i < selectedEmployeeTask.options.Count; i++)
        {
            if (selectedEmployeeTask.options[i].text == newTask)
            {
                selectedEmployeeTask.value = i; break;
            }
        }

        selectedEmployeeTask.onValueChanged.AddListener(delegate { UnitSelectChangeOccupation(); });

        selectEployeeDiscText.text =
         localizer.GetLocalizedText("Employeed for: ") + thisEmployee.employeedForDays.ToString() + localizer.GetLocalizedText(" Days") + System.Environment.NewLine +
         localizer.GetLocalizedText("Times called-out: x") + thisEmployee.calledOutAmount.ToString() + System.Environment.NewLine +
          localizer.GetLocalizedText("Times late: x") + thisEmployee.lateAmount.ToString() + System.Environment.NewLine +
           localizer.GetLocalizedText("Overtime worked: ") + thisEmployee.overtimeWorked.ToString("f2") + localizer.GetLocalizedText(" Hours") 
            ;
        selectEployeeDisc2Text.text =
            localizer.GetLocalizedText("Hourly Wage: ") + thisEmployee.hourlyWage.ToString("f2") + "$";
        /*
        selectEployeeDiscDisplayText.text =
            "Hourly Wage: " + thisEmployee.hourlyWage.ToString("f2") + "$" + System.Environment.NewLine +
            "Objective: " + thisEmployee.objective.ToString() + System.Environment.NewLine +
            "Task: " + thisEmployee.task.ToString() + System.Environment.NewLine +
            statusString
            ;
        */
        selectEployeeSkillsText.text =
            localizer.GetLocalizedText("Skills:") + System.Environment.NewLine +
            localizer.GetLocalizedText("Total Level: ") + thisEmployee.GetTotalLevel().ToString() + System.Environment.NewLine +
            localizer.GetLocalizedText("Customer Service: ") + custSkill.ToString() + System.Environment.NewLine +
            localizer.GetLocalizedText("Inventory: ") + invSkill.ToString() + System.Environment.NewLine +
            localizer.GetLocalizedText("Janitorial: ") + janitorialSkill.ToString() + System.Environment.NewLine +
            localizer.GetLocalizedText("Engineering: ") + engineerSkill.ToString() + System.Environment.NewLine +
            localizer.GetLocalizedText("Management: ") + managementSkill.ToString() + System.Environment.NewLine
            ;
        selectEployeeTraitsText.text =
            localizer.GetLocalizedText("Traits:") + System.Environment.NewLine +
            traitString
            ;
        selectedEmployee = thisEmployee;
        selected = true;
        if (thisEmployee.status != Employee2.Status.owner) { employeeFireButton.interactable = true; } else { employeeFireButton.interactable = false; }
        selectEmployeeStress.GetComponent<SliderController>().Activate(selectedEmployee.stress, true);
        selectEmployeeLongTermStress.GetComponent<SliderController>().Activate(selectedEmployee.longTermStress, true);

        List<int> character = thisEmployee.myUICharacter.vis.set;
        selectedEmployeeCharacter.SetSprites(character[0], character[1], character[2], character[3], character[4], character[5], character[6], thisEmployee.myUICharacter.vis.isFemale, thisEmployee.myUICharacter.vis.sizes);
        selectedEmployeeCharacter.UpdateEmotion((int)(thisEmployee.stress / 20));
        if (selectedEmployee.status != Employee2.Status.owner) { giveRaiseButton.gameObject.SetActive(true); UpdateEmployeeRaiseHappiness(); }
        else { giveRaiseButton.gameObject.SetActive(false); }
        ToolTip.Instance.ActivateTutorial(21);
        Invoke("CharacterDelay", 0.1f);
    }
    private List<string> RemoveUselessEmployeeTraits(List<string> traits)
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
    private void CharacterDelay() { if (selectedEmployee != null) { selectedEmployeeCharacter.UpdateEmotion((int)(selectedEmployee.stress / 20)); } if (selectedCustomer != null) { selectedCustomerCharacter.UpdateEmotion((((int)(selectedCustomer.storeOpinion / 40) * -1) + 4)); } }
    private void BuildingSelect(Building thisBuilding)
    {
        selectPop.GetChild(2).GetChild(2).GetChild(selectPop.GetChild(2).GetChild(2).childCount - 1).gameObject.SetActive(true);
        if (!thisBuilding.onFire) { moveBuldingButton.gameObject.SetActive(true); sellBuldingButton.gameObject.SetActive(true); }
        else { moveBuldingButton.gameObject.SetActive(false); sellBuldingButton.gameObject.SetActive(false); }
        selectPop.GetChild(2).gameObject.SetActive(true);
        //selectNameDisplayText.text = thisBuilding.type.ToString();
        selectBuildingNameDisplayText.text = thisBuilding.myName;
        selectBuildingNameDisplayText.GetComponent<AutoLocalizer>().UpdateLocalizedText(thisBuilding.myName);
        selectedBuilding = thisBuilding;

        string storageSize = "";
        if (thisBuilding.type != BuildingSO.Type.register) { storageSize = localizer.GetLocalizedText("Storage Capacity: ") + thisBuilding.transform.GetChild(1).childCount + "/" +thisBuilding.capacity; }
        selectBuildingDiscDisplayText.text =
         localizer.GetLocalizedText("Electricity $/Hour: ") + (thisBuilding.electricityCost).ToString("f2") + "$" + System.Environment.NewLine +
        storageSize + System.Environment.NewLine +
            localizer.GetLocalizedText(thisBuilding.myDiscription)
            ;

        if (thisBuilding.type == BuildingSO.Type.shelf || thisBuilding.type == BuildingSO.Type.stockPile) { selectPop.GetChild(2).GetChild(2).gameObject.SetActive(true); ListPossibleStockables(thisBuilding.selectedItemTypeID); }
        if (thisBuilding.type == BuildingSO.Type.stockPile) { foreach (int itemIDs in thisBuilding.allowedItemTypesID) { AllowStockedItems(itemIDs, true); } }
        //ToolTip.Instance.ActivateTutorial(19);
        if (thisBuilding.electricityCost > 0) 
        { 
            selectPop.GetChild(2).GetChild(4).gameObject.SetActive(true); 
            if (!thisBuilding.turnedOn) { selectPop.GetChild(2).GetChild(4).GetComponent<SettingsButton>().Disable(); } 
            else { selectPop.GetChild(2).GetChild(4).GetComponent<SettingsButton>().Enable(); }
        }
        else { selectPop.GetChild(2).GetChild(4).gameObject.SetActive(false); }
        buildingColorButtons[0].transform.parent.gameObject.SetActive(true);
        GetBuildingColors(thisBuilding.selectedColorChoice);
    }
    private void CustomerSelect(Customer2 thisCustomer)
    {
        selectCustomerNameDisplayText.text = thisCustomer.myName;
        selectedCustomer = thisCustomer;
        selected = true;

        List<string> traits = new List<string>();
        foreach (KeyValuePair<string, bool> pair in thisCustomer.traits) { if (pair.Value == true) { traits.Add(localizer.GetLocalizedText(pair.Key)); } }
        List<string> newTraits = RemoveUselessCustomerTraits(traits);
        string traitString = string.Join(Environment.NewLine, newTraits);

        selectCustomerMoneyText.text = thisCustomer.money.ToString("f2") + "$";
        selectCustomerDiscDisplayText.text =
            localizer.GetLocalizedText("Traits:") + System.Environment.NewLine +
            traitString
            ;

        float total = 0;
        foreach(float numb in thisCustomer.storePreferance) { total += numb; }
        List<float> storePreferedPrecentages = new List<float>();
        for (int i = 0; i < Controller.Instance.competitors.Count + 1; i++)
        {
            float percentage = thisCustomer.storePreferance[i] / total;
            storePreferedPrecentages.Add(percentage);
        }
        customerPieChart.UpdateChart(storePreferedPrecentages);
        List<int> character = thisCustomer.personVis.set;
        selectedCustomerCharacter.SetSprites(character[0], character[1], character[2], character[3], character[4], character[5], character[6], thisCustomer.personVis.isFemale, thisCustomer.personVis.sizes);
        selectedCustomerCharacter.UpdateEmotion((((int)(thisCustomer.storeOpinion / 40) * -1) + 4));
        ToolTip.Instance.ActivateTutorial(28);
        Invoke("CharacterDelay", 0.1f);
    }
    private List<string> RemoveUselessCustomerTraits(List<string> traits)
    {
        traits.Remove("workaholic");
        traits.Remove("slow_learner");
        traits.Remove("fast_learner");
        traits.Remove("burnt_out");
        traits.Remove("work_lover");
        traits.Remove("Cheap");
        traits.Remove("Expensive");

        traits.Remove("Pyromaniac");
        traits.Remove("Clumsy");
        traits.Remove("Lunatic");
        traits.Remove("Angry");

        return traits;
    }
    private void WallSelect(Wall thisWall)
    {
        selectedWall = thisWall;
        selectPop.GetChild(4).gameObject.SetActive(true);
        selectPop.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = thisWall.type.ToString();
        selectPop.GetChild(4).GetChild(0).GetComponent<AutoLocalizer>().UpdateLocalizedText(thisWall.type.ToString());

        if (thisWall.type == Wall.WallType.wall) { selectPop.GetChild(4).GetChild(3).gameObject.SetActive(true); }
        else { selectPop.GetChild(4).GetChild(3).gameObject.SetActive(false); }

        if (thisWall.type == Wall.WallType.entrance && Controller.Instance.entrances.Count > 1) { selectPop.GetChild(4).GetChild(4).gameObject.SetActive(true); selectPop.GetChild(4).GetChild(5).gameObject.SetActive(true); }
        else { selectPop.GetChild(4).GetChild(4).gameObject.SetActive(false); selectPop.GetChild(4).GetChild(5).gameObject.SetActive(false); }
        if (ToolTip.Instance.highestToolTipAchieved > 50) { ToolTip.Instance.ActivateTutorial(53); }
    }
    private void MultiBuildingSelect()
    {
        moveBuldingButton.gameObject.SetActive(false);
        selectPop.GetChild(2).GetChild(4).gameObject.SetActive(false);
        selectPop.GetChild(2).gameObject.SetActive(true);
        //serves as an update and get function
        Building firstBuilding = Controller.Instance.selectedBuildings[0];

        selectBuildingNameDisplayText.text = localizer.GetLocalizedText(firstBuilding.myName) + " x" + Controller.Instance.selectedBuildings.Count;

        string storageSize = "";
        if (firstBuilding.type != BuildingSO.Type.register) { storageSize = localizer.GetLocalizedText("Storage Capacity: ") + firstBuilding.capacity; }
        selectBuildingDiscDisplayText.text =
            localizer.GetLocalizedText("Electricity $/Hour: ") + (firstBuilding.electricityCost).ToString("f2") + "$" + System.Environment.NewLine +
        storageSize + System.Environment.NewLine +
            localizer.GetLocalizedText(firstBuilding.myDiscription)
            ;

        if (firstBuilding.type == BuildingSO.Type.shelf) { selectPop.GetChild(2).GetChild(2).gameObject.SetActive(true); firstBuildingSelectedItem = firstBuilding.selectedItemTypeID; ListPossibleStockables(firstBuilding.selectedItemTypeID); }

        GetBuildingColors(-1);

    }
    private void UpdateSelectDisplay(object sender, TickSystem.OnTickEventArgs e)
    {
        if (selectedEmployee != null)
        {
            string statusString = "";

            int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
            if (time >= selectedEmployee.workStart && time < selectedEmployee.workEnd)
            {
                if (selectedEmployee.animator.GetBool("OnShift"))
                {
                    statusString = "Working";
                }
                else
                {
                    statusString = "Shift changed, will come in tomorrow";
                    if (selectedEmployee.workDays[UIController.Instance.weekDayText.text] == false) { statusString = "Day off"; }
                    if (selectedEmployee.late) { statusString = "Late"; }
                    if (selectedEmployee.calledOut) { statusString = "Called out"; }
                    if (selectedEmployee.sentHome) { statusString = "Sent home"; }
                }
            }
            else { statusString = "Off duty"; }

            //selectEployeeWageText.text = selectedEmployee.hourlyWage.ToString("f2") + "$";
            selectEployeeObjectiveText.text = selectedEmployee.objective.ToString();
            selectEployeeStatusText.text = statusString;
            selectEployeeObjectiveText.GetComponent<AutoLocalizer>().UpdateLocalizedText(statusString);
            selectEployeeStatusText.GetComponent<AutoLocalizer>().UpdateLocalizedText(statusString);
            selectEployeeDisc2Text.text =
            localizer.GetLocalizedText("Hourly Wage: ") + selectedEmployee.hourlyWage.ToString("f2") + "$";
            /*
            selectEployeeDiscDisplayText.text =
                "Hourly Wage: " + selectedEmployee.hourlyWage.ToString("f2") + "$" + System.Environment.NewLine +
                "Objective: " + selectedEmployee.objective.ToString() + System.Environment.NewLine +
                "Task: " + selectedEmployee.task.ToString() + System.Environment.NewLine +
                statusString
                ;
            */
            selectedEmployeeCharacter.UpdateEmotion((int)(selectedEmployee.stress / 20));
            selectEmployeeStress.GetComponent<SliderController>().Activate(selectedEmployee.stress, false);
            selectEmployeeLongTermStress.GetComponent<SliderController>().Activate(selectedEmployee.longTermStress, false);
        }

        if (selectedCustomer != null)
        {
            selectedCustomerCharacter.UpdateEmotion((((int)(selectedCustomer.storeOpinion / 40) * -1) + 4));
            selectCustomerMoneyText.text = selectedCustomer.money.ToString("f2") + "$";
        }

        if (selectedBuilding != null)
        {
            string storageSize = "";
            if (selectedBuilding.type != BuildingSO.Type.register) { storageSize = localizer.GetLocalizedText("Storage Capacity: ") + selectedBuilding.transform.GetChild(1).childCount + "/" + selectedBuilding.capacity; }
            selectBuildingDiscDisplayText.text =
                localizer.GetLocalizedText("Electricity $/Hour: ") + (selectedBuilding.electricityCost).ToString("f2") + "$" + System.Environment.NewLine +
            storageSize + System.Environment.NewLine +
           localizer.GetLocalizedText(selectedBuilding.myDiscription)
            ;
        }
    }
    private void DeSelectPopUp()
    {
        uiBottomAnimatior.SetBool("Select", false);
    }
    public void DeActivateAll()
    {
        selected = false; selectedEmployee = null; selectedCustomer = null; selectedBuilding = null; selectedWall = null;
        selectPop.GetChild(1).gameObject.SetActive(false);
        selectPop.GetChild(2).gameObject.SetActive(false);
        selectPop.GetChild(3).gameObject.SetActive(false);
        selectPop.GetChild(4).gameObject.SetActive(false);
        selectPop.GetChild(5).gameObject.SetActive(false);
        selectPop.GetChild(2).GetChild(2).gameObject.SetActive(false);
        ToolTip.Instance.HideToolTip();
    }
    private void UnitSelectChangeOccupation()
    {
        if (selectedEmployee != null)
        {
            selectedEmployee.SwitchTask(null, selectedEmployeeTask.options[selectedEmployeeTask.value].text, null);
        }
    }
    private void ListPossibleStockables(int selectedObject)
    {
        if (Controller.Instance.selectedBuildings.Count > 1)
        {
            for (int i = 0; i < selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount; i++) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(true); }
            foreach (Building build in Controller.Instance.selectedBuildings)
            {
                if (!build.can_Storage.Contains("Anything"))
                {
                    for (int i = 0; i < selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount; i++)
                    {
                        if (Controller.Instance.items.Count > i)
                        {
                            if (Controller.Instance.items[i].year_Start <= year)
                            {
                                if (!build.can_Storage.Contains(selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.name)) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                                //if (Controller.Instance.items[i].special && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                                //if (Controller.Instance.items[i].seasonal != -1 && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                            }
                            else if (typeOfDay == "NewYears Eve!" && Controller.Instance.items[i].year_Start == year + 1)
                            {
                                if (!build.can_Storage.Contains(selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.name)) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                                //if (Controller.Instance.items[i].special && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                                //if (Controller.Instance.items[i].seasonal != -1 && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                            }
                            else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                            if (i == selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                        }

                    }
                }
            }
            foreach (Building build in Controller.Instance.selectedBuildings)
            {
                if (build.selectedItemTypeID != firstBuildingSelectedItem) { selectedObject = -1; break; }
            }

            for (int i = 0; i < selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount; i++)
            {
                selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.GetComponent<Button>().interactable = true;
            }
            if (selectedObject != -1) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(selectedObject).GetComponent<Button>().interactable = false; }
            //else { selectPop.GetChild(2).GetChild(2).GetChild(selectPop.GetChild(2).GetChild(2).transform.childCount - 1).GetComponent<Button>().interactable = false; }
        }
        else
        {
            for (int i = 0; i < selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount; i++)
            {
                selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                if (Controller.Instance.items.Count > i)
                {
                    if (Controller.Instance.items[i].year_Start <= year)
                    {
                        if (selectedBuilding.can_Storage.Contains("Anything") || selectedBuilding.can_Storage.Contains(selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.name))
                        {
                            selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(true);
                            //if (Controller.Instance.items[i].special && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                            //if (Controller.Instance.items[i].seasonal != -1 && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i]))
                            //{
                            //    selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false);
                            //}
                        }
                        else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                    }
                    else if (typeOfDay == "NewYears Eve!" && Controller.Instance.items[i].year_Start == year + 1)
                    {
                        if (selectedBuilding.can_Storage.Contains("Anything") || selectedBuilding.can_Storage.Contains(selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.name))
                        {
                            selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(true);
                            //if (Controller.Instance.items[i].special && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                            //if (Controller.Instance.items[i].seasonal != -1 && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[i])) { selectPop.GetChild(2).GetChild(2).GetChild(i).gameObject.SetActive(false); }
                        }
                        else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                    }
                    else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).gameObject.SetActive(false); }
                }
            }

            if (selectedBuilding.can_Storage.Contains("Anything")) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount - 1).gameObject.SetActive(false); }
            //else { selectPop.GetChild(2).GetChild(2).GetChild(selectPop.GetChild(2).GetChild(2).transform.childCount - 1).gameObject.SetActive(true); }
        }


        /*
        if (selectedObject != 0)
        {
            for (int i = 0; i < selectPop.GetChild(1).GetChild(2).transform.childCount; i++)
            {
                //if item != type of shelf?
                selectPop.GetChild(1).GetChild(2).GetChild(i).gameObject.GetComponent<Button>().interactable = true;
            }

            selectPop.GetChild(1).GetChild(2).GetChild(selectedObject - 1).GetComponent<Button>().interactable = false;
        }
        else
        {
            for (int i = 0; i < selectPop.GetChild(1).GetChild(2).transform.childCount; i++)
            {
                //if item != type of shelf?
                selectPop.GetChild(1).GetChild(2).GetChild(i).gameObject.GetComponent<Button>().interactable = true;
            }
        }
        */
        for (int i = 0; i < selectPop.GetChild(2).GetChild(2).GetChild(0).transform.childCount; i++)
        {
            if (selectedBuilding != null)
            {
                if (selectedBuilding.type == BuildingSO.Type.shelf)
                {
                    if (selectedObject == i) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(selectedObject).GetComponent<Button>().interactable = false; selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(selectedObject).GetChild(0).GetComponent<Image>().color = Color.white; }
                    else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().color = Color.black; }
                }
                else if (selectedBuilding.type == BuildingSO.Type.stockPile)
                {
                    if (selectedBuilding.allowedItemTypesID.Contains(i)) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white; }
                    else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().color = Color.black; }
                }
            }
        }
    }
    public void ChangeItemStockType(int itemID)
    {
        //Controller.Instance.selectedBuilding.selectedItemTypeID = itemID;

        if (Controller.Instance.selectedBuildings.Count > 1)
        {
            StartCoroutine(ChangingMultipleItemTypes(itemID));
        }
        else
        {
            Controller.Instance.selectedBuilding.ChangeItemType(itemID);
        }

        ListPossibleStockables(itemID);
        ToolTip.Instance.ActivateTutorial(13);
    }
    private IEnumerator ChangingMultipleItemTypes(int itemID)
    {
        foreach (Building build in Controller.Instance.selectedBuildings)
        {
            build.ChangeItemType(itemID);
            firstBuildingSelectedItem = itemID;
            yield return new WaitForEndOfFrame();
        }
    }
    public void AllowStockedItems(int itemID, bool enable)
    {
        if (enable) { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(itemID).transform.GetChild(0).GetComponent<Image>().color = Color.white; }
        else { selectPop.GetChild(2).GetChild(2).GetChild(0).GetChild(itemID).GetChild(0).GetComponent<Image>().color = Color.black; }
    }
    public void DelcareBankrupty() { GameOver(false, "You lost... You have declared bankruptcy", true); }
    public void GameOver(bool win, string reason, bool overriding)
    {
        if (playedDays > 1 || overriding)
        {
            if (!gameOver)
            {
                gameOver = true;
                SetMainAnimatorString("ToWin");
                //gameOverPop.gameObject.SetActive(true);
                gameOverPop.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = reason;
                gameOverPop.GetChild(1).GetChild(0).GetComponent<AutoLocalizer>().UpdateLocalizedText(reason);
                TickSystem.Instance.PauseMenu();
            }

            if (win) { TransitionController.Instance.Win(); }
        }
        else if (win) { foreach (Customer2 customer in Controller.Instance.customers) { customer.storePreferance[0] -= 1; } }
        else { foreach (Customer2 customer in Controller.Instance.customers) { customer.storePreferance[0] += 1; } }
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        if (SteamClient.IsValid)
        {
            Steamworks.SteamClient.Shutdown();
        }

        Application.Quit();
    }
    public void FireEmployee() { if(selectedEmployee != null) { selectedEmployee.FireMe(); employeeFireButton.interactable = false; DeSelectPopUp(); Controller.Instance.SpawnCustomer(true); } }
    public void SendEmployeeHome() { if (selectedEmployee != null) { selectedEmployee.SendHome(); DeSelectPopUp(); selectedEmployee = null; Controller.Instance.selectedEmployee = null; } }
    public void FireCustomer() { if (selectedCustomer != null) { selectedCustomer.Leave(); DeSelectPopUp(); } }
    public void DeleteBuilding() 
    { 
        if (selectedBuilding != null) 
        {
            if (Controller.Instance.selectedBuildings.Count > 1)
            {
                StartCoroutine(DeletingMultipleBuildings());
            }
            else
            {
                Controller.Instance.MoneyValueChange(selectedBuilding.sellValue, UtilsClass.GetMouseWorldPosition(), false, true); selectedBuilding.DestroyMe();
            }      
        }
        DeSelectPopUp();
    }
    private IEnumerator DeletingMultipleBuildings()
    {
        foreach (Building building in Controller.Instance.selectedBuildings) 
        { 
            Controller.Instance.MoneyValueChange(building.sellValue, UtilsClass.GetMouseWorldPosition(), false, true); 
            MoneyGained -= building.sellValue;
            yield return new WaitForEndOfFrame();
            building.DestroyMe(); 
        }
    }
    private BuildMover buildMover;
    public void MoveBuilding()
    {
        if (buildMover == null) { buildMover = transform.GetComponent<BuildMover>(); }

        if (selectedBuilding != null)
        {
            buildMover.MovingBuilding(selectedBuilding);
            movingBuilding = true;

            MapController.Instance.gridEnabled = true;
            MapController.Instance.BuildingLayerView(8, true, false);
            MapController.Instance.placingBuilding = selectedBuilding.buildingSO;
            MapController.Instance.RefreshSelectedObjectType();

            selectedBuilding.DestroyMe();
        }

        DeSelectPopUp();
    }

    [SerializeField] private int myCustomers;
    [SerializeField] private int competitionsCustomers;
    [SerializeField] private TextMeshProUGUI myCustText;
    [SerializeField] private TextMeshProUGUI compCustText;
    public Button buyoutCompetitorButton;
    public void CreateLog(int color, string message, string sender, int messageType)
    {
        GameObject newLog = Instantiate(log, logHolder);
        message = Localizer.Instance.GetLocalizedText(message);
        newLog.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        Color newColor = Color.black;
        switch(color)
        {
            case 1: newColor = Color.red; break;
            case 2: newColor = Color.blue; break;
            case 3: newColor = Color.yellow; break;
            case 4: newColor = Color.green; break;
        }
        newLog.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = newColor;

        string hourString = ""; string minuteString = "";
        if (minutes < 10) { minuteString = "0" + minutes.ToString(); } else { minuteString = minutes.ToString(); }
        if (hour < 10) { hourString = "0" + hour.ToString(); } else { hourString = hour.ToString(); }
        string monthString = ""; string dayString = "";
        if (day < 10) { dayString = "0" + day.ToString(); } else { dayString = day.ToString(); }
        if (month < 10) { monthString = "0" + month.ToString(); } else { monthString = month.ToString(); }

        string date = hourString + ":" + minuteString + " " + monthString + "/" + dayString + "/" + year.ToString();
        GameObject newMessage = Instantiate(phoneMessage, phoneMessageHolder);
        newMessage.GetComponent<PhoneMessage>().StartUp(color, message,date, sender, messageType);
    }
    private void OpenChecker()
    {
        if (Controller.Instance.shelves.Count > 0 && Controller.Instance.registers.Count > 0 && Controller.Instance.stockPiles.Count > 0 && stockTickets > 0)
        {
            openStoreButton.interactable = true;
        }
        else { if (openStoreButton != null) { openStoreButton.interactable = false; } }

        if (Controller.Instance.shelves.Count > 0 && Controller.Instance.registers.Count > 0 && Controller.Instance.stockPiles.Count > 0 && !PlayerPrefs.HasKey("T" + 5.ToString())) { PlayerPrefs.SetInt("T" + 5.ToString(), 0); }//ToolTip.Instance.ActivateTutorial(5); }
        if (stockTickets > 90 && !PlayerPrefs.HasKey("T" + 9.ToString())) { PlayerPrefs.SetInt("T" + 9.ToString(), 0); }//ToolTip.Instance.ActivateTutorial(9); }

        if (toDoListManager != null)
        {
            //loan
            if (Controller.Instance.shelves.Count > 0) { toDoListManager.CheckOff(1); }
            if (Controller.Instance.stockPiles.Count > 0) { toDoListManager.CheckOff(2); }
            if (Controller.Instance.registers.Count > 0) { toDoListManager.CheckOff(3); }
            if (stockTickets > 0) { toDoListManager.CheckOff(4); }
            //add
            //open store
        }
    }
    public void BuildVisualButton()
    {
        if(buildGridEnabled) { buildGridEnabled = false; MapController.Instance.BuildingLayerView(0, false, false); }
        else { buildGridEnabled = true; MapController.Instance.BuildingLayerView(0, true, false); }
    }
    public void LightMapButton()
    {
        if (lightMapEnabled) { lightMapEnabled = false; MapController.Instance.BuildingLayerView(2, false, true); }
        else { lightMapEnabled = true; MapController.Instance.BuildingLayerView(2, false, false); }
    }
    public void CleanVisualButton()
    {
        if (cleanMapEnabled) { cleanMapEnabled = false; MapController.Instance.BuildingLayerView(1, false, true); }
        else { cleanMapEnabled = true; MapController.Instance.BuildingLayerView(1, false, false); }
    }
    public void BeautyVisualButton()
    {
        if (beautyMapEnabled) { beautyMapEnabled = false; MapController.Instance.BuildingLayerView(3, false, true); }
        else { beautyMapEnabled = true; MapController.Instance.BuildingLayerView(3, false, false); }
    }
    public bool allowZoneSettings;
    public void ZoneSettingsButton()
    {
        if (allowZoneSettings) { allowZoneSettings = false; }
        else { allowZoneSettings = true; }
    }
    public bool allowManagerSettings;
    public void ManagerSettingsButton()
    {
        if (allowManagerSettings) { allowManagerSettings = false; MapController.Instance.BuildingLayerView(8, false, true); }
        else { allowManagerSettings = true; MapController.Instance.BuildingLayerView(8, false, false); }
    }
    public void ChangeOrderMask()
    {
        if (orderMaskTarget == 0) { orderMaskTarget = -390; }
        else { orderMaskTarget = 0; }
    }
    public void CreateOrder(ItemSO item)
    {
        GameObject order = Instantiate(uiOrder, orderHolder);
        UIItemOrder iOrder = order.GetComponent<UIItemOrder>();
        iOrder.item = item;
        iOrder.StartUp();

        GameObject customerPref2 = Instantiate(customerItemPrefsAnalysis, customerItemsPrefsAnalysisHolder);
        customerPref2.name = item.myName;
        customerPref2.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
    }
    public void CreateStorageOption(ItemSO item)
    {
        GameObject option = Instantiate(storageOptions, storageOptionsHolder);
        Button optionButton = option.GetComponent<Button>();
        if (item != null)
        {
            optionButton.onClick.AddListener(() => ChangeItemStockType(item.itemID));
            option.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
            option.gameObject.name = item.itemType;

            GameObject customerPref = Instantiate(customerItemPrefs, customerItemsPrefsHolder);
            CustomerItem customerItem = customerPref.GetComponent<CustomerItem>();
            customerItem.item = item;
            customerItem.StartUp();
        }
        else
        {
            optionButton.onClick.AddListener(() => ChangeItemStockType(-1));
            option.gameObject.name = "Nothing";
        }



        //UIItemOrder iOrder = option.GetComponent<UIItemOrder>();
        //iOrder.item = item;
        //iOrder.StartUp();
        //needs to generate buttons based on all items
        /*
        for (int i = 0; i < selectPop.GetChild(1).GetChild(2).transform.childCount; i++)
        {
            int number = i;
            CreateButtonList(number);
        }
        */
    }
    //private void CreateButtonList(int number) { selectPop.GetChild(1).GetChild(2).GetChild(number).gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeItemStockType(number + 1)); }
    public void ExitToMainMenu()
    {
        Destroy(TransitionController.Instance.gameObject);
        SceneManager.LoadSceneAsync("Start", LoadSceneMode.Single);
    }
    public void UpdateStoreMusicDisplay()
    {
        for (int i = 0; i < musicOptions.Length; i++)
        {
            if (i == Controller.Instance.previousMusicChoice) { musicOptions[i].interactable = false; }
            else { musicOptions[i].interactable = true; }
        }
        //work
        //speed
        //stress
        if (Controller.Instance.musicWorkBonus > 0) { musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.green; }
        else if (Controller.Instance.musicWorkBonus < 0) { musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red; }
        else { musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.black; }
        string localized = "Work";
        switch (Controller.Instance.musicWorkBonus)
        {
            case -6: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "------" + localizer.GetLocalizedText(localized); break;
            case -5: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "-----" + localizer.GetLocalizedText(localized); break;
            case -4: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "----" + localizer.GetLocalizedText(localized); break;
            case -3: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "---" + localizer.GetLocalizedText(localized); break;
            case -2: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "--" + localizer.GetLocalizedText(localized); break;
            case -1: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "-" + localizer.GetLocalizedText(localized); break;
            case 0: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = localizer.GetLocalizedText(localized); break;
            case 1: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + localizer.GetLocalizedText(localized); break;
            case 2: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "++" + localizer.GetLocalizedText(localized); break;
            case 3: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+++" + localizer.GetLocalizedText(localized); break;
            case 4: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "++++" + localizer.GetLocalizedText(localized); break;
            case 5: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+++++" + localizer.GetLocalizedText(localized); break;
            case 6: musicMenu.GetChild(1).GetComponent<TextMeshProUGUI>().text = "++++++" + localizer.GetLocalizedText(localized); break;
        }

        if (Controller.Instance.musicSpeedBonus > 0) { musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.green; }
        else if (Controller.Instance.musicSpeedBonus < 0) { musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.red; }
        else { musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.black; }
        localized = "Speed";
        switch (Controller.Instance.musicSpeedBonus)
        {
            case -6: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "------" + localizer.GetLocalizedText(localized); break;
            case -5: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "-----" + localizer.GetLocalizedText(localized); break;
            case -4: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "----" + localizer.GetLocalizedText(localized); break;
            case -3: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "---" + localizer.GetLocalizedText(localized); break;
            case -2: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "--" + localizer.GetLocalizedText(localized); break;
            case -1: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "-" + localizer.GetLocalizedText(localized); break;
            case 0: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = localizer.GetLocalizedText(localized); break;
            case 1: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "+" + localizer.GetLocalizedText(localized); break;
            case 2: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "++" + localizer.GetLocalizedText(localized); break;
            case 3: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "+++" + localizer.GetLocalizedText(localized); break;
            case 4: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "++++" + localizer.GetLocalizedText(localized); break;
            case 5: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "+++++" + localizer.GetLocalizedText(localized); break;
            case 6: musicMenu.GetChild(2).GetComponent<TextMeshProUGUI>().text = "++++++" + localizer.GetLocalizedText(localized); break;
        }

        if (Controller.Instance.musicStressBonus < 0) { musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.green; }
        else if (Controller.Instance.musicStressBonus > 0) { musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.red; }
        else { musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.black; }
        localized = "Stress";
        switch (Controller.Instance.musicStressBonus)
        {
            case -6: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "------" + localizer.GetLocalizedText(localized); break;
            case -5: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "-----" + localizer.GetLocalizedText(localized); break;
            case -4: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "----" + localizer.GetLocalizedText(localized); break;
            case -3: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "---" + localizer.GetLocalizedText(localized); break;
            case -2: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "--" + localizer.GetLocalizedText(localized); break;
            case -1: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "-" + localizer.GetLocalizedText(localized); break;
            case 0: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = localizer.GetLocalizedText(localized); break;
            case 1: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "+" + localizer.GetLocalizedText(localized); break;
            case 2: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "++" + localizer.GetLocalizedText(localized); break;
            case 3: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "+++" + localizer.GetLocalizedText(localized); break;
            case 4: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "++++" + localizer.GetLocalizedText(localized); break;
            case 5: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "+++++" + localizer.GetLocalizedText(localized); break;
            case 6: musicMenu.GetChild(3).GetComponent<TextMeshProUGUI>().text = "++++++" + localizer.GetLocalizedText(localized); break;
        }
    }
    public void UpdateCustomerPreferance(List<int> storePreferances)
    {
        List<float> storePreferedPrecentages = new List<float>();
        List<float> number = new List<float>();
        for (int i = 0; i < Controller.Instance.competitors.Count + 1; i++) { number.Add(0); }

        for (int i = 0; i < storePreferances.Count; i++)
        {
            number[storePreferances[i]] += 1;
        }

        for (int i = 0; i < Controller.Instance.competitors.Count + 1; i++)
        {
            float percentage = number[i] / storePreferances.Count;
            storePreferedPrecentages.Add(percentage);
        }

        myMarketShare = storePreferedPrecentages[0];
        pieChart.UpdateChart(storePreferedPrecentages);
        if (storePreferedPrecentages[0] <= (0.03 / TransitionController.Instance.totalDifficulty)) { GameOver(false, "You lost... Too many customers prefer to shop elsewhere", false); }
    }
    public void UpdateResume(string name, int job, float wage, float age, string gender, int custSkill, int invSkill, int janitorialSkill, int engineerSkill, int managementSkill, StaffApplicant applicant, List<int> character, List<float> characterSizes)
    {
        //name
        resumeName.text = name;

        //applying for
        string occupation = "";
        switch (job)
        {
            case 0: break;
            case 1: occupation = "Stocker"; break;
            case 2: occupation = "Cashier"; break;
            case 3: occupation = "Janitor"; break;
            case 4: occupation = "Engineer"; break;
            case 5: occupation = "Manager"; break;
        }
        resumeApplyingFor.text = localizer.GetLocalizedText("Applying for: ") + localizer.GetLocalizedText(occupation);

        //wage
        resumeWage.text = wage.ToString("f2") + "$";

        //info
        resumeInfo.text = localizer.GetLocalizedText("Age: ") + age.ToString("f0") + System.Environment.NewLine +
           localizer.GetLocalizedText("Gender: ") + localizer.GetLocalizedText(gender);

        //skills
        resumeSkills.text =
          localizer.GetLocalizedText("Cashier: ") + custSkill.ToString() + System.Environment.NewLine +
          localizer.GetLocalizedText("Stocker: ") + invSkill.ToString() + System.Environment.NewLine +
          localizer.GetLocalizedText("Janitorial: ") + janitorialSkill.ToString() + System.Environment.NewLine +
          localizer.GetLocalizedText("Engineering: ") + engineerSkill.ToString() + System.Environment.NewLine +
          localizer.GetLocalizedText("Management: ") + managementSkill.ToString() + System.Environment.NewLine
         ;

        //certs
        //if any skill is above 9?
        resumeCerts.text = "None";
        resumeCerts.GetComponent<AutoLocalizer>().UpdateLocalizedText("None");

        int randomTalk = Random.Range(0, 3);

        //work experiance
        string skillString = "";
        if (job == 1)
        {
            switch (invSkill)
            {
                case 0:
                    switch (randomTalk)
                    {
                        default: skillString = "Has not picked anything up in their life."; break;
                        case 1: skillString = "Has no pattern recognition."; break;
                        case 2: skillString = "Asked what a job is."; break;
                    }
                     break;
                case 1:
                    switch (randomTalk)
                    {
                        default: skillString = "Has a functional spine."; break;
                        case 1: skillString = "Saw someone do the job once."; break;
                        case 2: skillString = "Has a functional spine."; break;
                    }
                    break;
                case 2: skillString = "Has a doctors note saying that they are able to carry up to five pounds."; break;
                case 3: skillString = "Does imaginary curls in the break room."; break;
                case 4: skillString = "Lifted weights in middle school."; break;
                case 5: skillString = "The one mom calls when they need help carrying in the groceries."; break;
                case 6: skillString = "Can curl milk cartoons across the store. (They then proved they could)"; break;
                case 7: skillString = "Can carry more than two things in the same hand. Wow!"; break;
                case 8: skillString = "Can bench press more then you."; break;
                case 9: skillString = "Friends always call them when they need help moving."; break;
                case 10: skillString = "Can throw things from across the room."; break;
                case 11: skillString = "Known for carrying shelves instead of items."; break;
                case 12: skillString = "Weightlifts at a professional level."; break;
                case 13: skillString = "Has the ability to pick up your mom."; break;
                case 14: skillString = "Carries all the groceries in on one trip."; break;
                case 15: skillString = "Moves mountains to better-suited locations."; break;
            }
        }

        if (job == 2)
        {
            switch (custSkill)
            {
                case 0:
                    switch (randomTalk)
                    {
                        default: skillString = "Hasn't spoken to anyone since birth."; break;
                        case 1: skillString = "Can almost sing the alphabet."; break;
                        case 2: skillString = "Said many weird words during the interview."; break;
                    }
                    break;
                case 1:
                    switch (randomTalk)
                    {
                        default: skillString = "Calls Mom daily."; break;
                        case 1: skillString = "Talks to the mirror about glass."; break;
                        case 2: skillString = "Calls Mom daily."; break;
                    }
                     break;
                case 2: skillString = "Has a friend or two."; break;
                case 3: skillString = "Has made a person laugh once."; break;
                case 4: skillString = "Has the ability to smile for family photos. (But chooses not to)"; break;
                case 5: skillString = "Likes to touch other peoples hands with theirs and shake them around."; break;
                case 6: skillString = "They have the ability to smile when they need to."; break;
                case 7: skillString = "They claim they are dating someone from another city."; break;
                case 8: skillString = "They claim that they think of what to say before actually saying it."; break;
                case 9: skillString = "Claims they can speak to animals"; break;
                case 10: skillString = "They seem to actually enjoy talking to people"; break;
                case 11: skillString = "They claim that they're the go-to negotiator for snack choices."; break;
                case 12: skillString = "They claim that they are dating this famous movie star, but would not reveal whom."; break;
                case 13: skillString = "Claims they are so skilled that you could convince you to try alien food."; break;
                case 14: skillString = "Claims that they have people worship the ground they walk on"; break;
                case 15: skillString = "Negotiates with aliens on behalf of the human race."; break;
            }
        }

        if (job == 3)
        {
            switch (janitorialSkill)
            {
                case 0:
                    switch (randomTalk)
                    {
                        default: skillString = "Lives in a swamp."; break;
                        case 1: skillString = "Smells worse than they look."; break;
                        case 2: skillString = "Has flies following them around."; break;
                    }
                     break;
                case 1:
                    switch (randomTalk)
                    {
                        default: skillString = "Knows what a mop looks like."; break;
                        case 1: skillString = "Spit shines everything."; break;
                        case 2: skillString = "Knows what a mop looks like."; break;
                    }
                    break;
                case 2: skillString = "Has touched soap before."; break;
                case 3: skillString = "Knows what it is like to brush teeth."; break;
                case 4: skillString = "Has picked up their toys before."; break;
                case 5: skillString = "has cleaned under their finger nails."; break;
                case 6: skillString = "Bathes regularly."; break;
                case 7: skillString = "Has teeth that sparkle."; break;
                case 8: skillString = "Can still smell their perfume."; break;
                case 9: skillString = "Has never been told that they have bad breathe."; break;
                case 10: skillString = "Polishes the floor before they walk on it."; break;
                case 11: skillString = "Car has a mop and broom attached."; break;
                case 12: skillString = "Literaly cleaned on their way in."; break;
                case 13: skillString = "Known for being the cleanest in the room."; break;
                case 14: skillString = "Trash just seems to fall into the bin when they are around."; break;
                case 15: skillString = "Sterilizes equiptment on a molecular level."; break;
            }
        }

        if (job == 4)
        {
            switch (engineerSkill)
            {
                case 0:
                    switch (randomTalk)
                    {
                        default: skillString = "Has a basic understanding of elementary school math."; break;
                        case 1: skillString = "Thinks the earth is flat."; break;
                        case 2: skillString = "Claims they discovered gravity."; break;
                    }
                     break;
                case 1:
                    switch (randomTalk)
                    {
                        default: skillString = "Knows that if you bang two objects together hard enough..."; break;
                        case 1: skillString = "Knows the name of that one tool."; break;
                        case 2: skillString = "Knows that if you bang two objects together hard enough..."; break;
                    }
                     break;
                case 2: skillString = "Held a flashlight for dad once."; break;
                case 3: skillString = "Has heard of a hammer, but hasn't met him."; break;
                case 4: skillString = "Can sometimes tell the difference between a wrench and a hammer."; break;
                case 5: skillString = "Was there when their dad taught someone else how to change a tire."; break;
                case 6: skillString = "Knows the 'righty tighty, lefty loosy' by heart."; break;
                case 7: skillString = "Can take apart a remote controller and almost put it back together again."; break;
                case 8: skillString = "Can tell which way to rotate the screwdriver."; break;
                case 9: skillString = "Knows that one trick where they attach the thing to another thing."; break;
                case 10: skillString = "Knows when it is not safe to drive over a bridge."; break;
                case 11: skillString = "Was that one person in that viral video where they throw the hammer and nails the nail."; break;
                case 12: skillString = "Was the sole designer of those chairs which can hold over five hundred pounds."; break;
                case 13: skillString = "Builds robots that are better than you at everything."; break;
                case 14: skillString = "Was once a rocket scientiest."; break;
                case 15: skillString = "Builds enitre cities on their free time."; break;
            }
        }

        if (job == 5)
        {
            switch (managementSkill)
            {
                case 0:
                    switch (randomTalk)
                    {
                        default: skillString = "Was in charge of taking care of a gerbil once. (it died)"; break;
                        case 1: skillString = "Has a liberal arts degree."; break;
                        case 2: skillString = "Can sometimes take care of themselves."; break;
                    }
                    break;
                case 1:
                    switch (randomTalk)
                    {
                        default: skillString = "Saw someone on TV once."; break;
                        case 1: skillString = "Orders ants to march."; break;
                        case 2: skillString = "Saw someone on TV once."; break;
                    }
                     break;
                case 2: skillString = "Has a friend or two who might follow your management advice... if there's pizza involved."; break;
                case 3: skillString = "Has made someone laugh during a meeting, but it wasn't necessarily a good thing."; break;
                case 4: skillString = "They have experience at pointing at things for people to look at."; break;
                case 5: skillString = "They say they like to give high-fives to their team members, whether they want them or not."; break;
                case 6: skillString = "They have the most 'Not Bad' awards I have ever seen."; break;
                case 7: skillString = "Is a pet owner."; break;
                case 8: skillString = "They claimed they only applied so that they can make other peolple do their jobs."; break;
                case 9: skillString = "Claims they have trained a monkey to play monopoly."; break;
                case 10: skillString = "Has convinced a few colleagues to join their 'management fan club.'"; break;
                case 11: skillString = "Has not yet experianced a mutiny... yet."; break;
                case 12: skillString = "Was once a drill sergeant"; break;
                case 13: skillString = "Claims that people naturally follow their orders."; break;
                case 14: skillString = "Nation leaders ask them for advice."; break;
                case 15: skillString = "Was once president of the world."; break;
            }
        }

        resumeExperiance.text = skillString;
        resumeExperiance.GetComponent<AutoLocalizer>().UpdateLocalizedText(skillString);

        //strentghs
        string strengths = "";
        if (applicant.traits["weather_lover"]) { strengths += localizer.GetLocalizedText("They claim that extreme weather excites them.") + System.Environment.NewLine; }
        if (applicant.traits["early_bird"]) { strengths += localizer.GetLocalizedText("They prefer the morning shift.") + System.Environment.NewLine; }
        if (applicant.traits["night_owl"]) { strengths += localizer.GetLocalizedText("They say they often are up all night gaming.") + System.Environment.NewLine; }
        if (applicant.traits["work_lover"]) { strengths += localizer.GetLocalizedText("They prefer to be at work rather than at home becuase they hate their spouce") + System.Environment.NewLine; }
        if (applicant.traits["stressless"]) { strengths += localizer.GetLocalizedText("They say that they can meditate while they are working.") + System.Environment.NewLine; }
        if (applicant.traits["fast_learner"]) { strengths += localizer.GetLocalizedText("They claim that they learn so fast that they completed college at the age of seven.") + System.Environment.NewLine; }
        if (applicant.traits["workaholic"]) { strengths += localizer.GetLocalizedText("Earning money is their only goal in life.") + System.Environment.NewLine; }
        if (applicant.traits["fast_walker"]) { strengths += localizer.GetLocalizedText("They speed walk all day and night, even while they sleep.") + System.Environment.NewLine; }
        if (applicant.traits["fast_worker"]) { strengths += localizer.GetLocalizedText("They claim that they move their hands so fast that no-one can see them, and then they dimistrated, and it was true.") + System.Environment.NewLine; }
        if (applicant.traits["sensitive"]) { strengths += localizer.GetLocalizedText("Loud noises really make them feel at home.") + System.Environment.NewLine; }
        if (applicant.traits["reliable"]) { strengths += localizer.GetLocalizedText("They claimed that they haven't called out or came in late since they were born.") + System.Environment.NewLine; }
        if (applicant.traits["Cold_Lover"]) { strengths += localizer.GetLocalizedText("They say they enjoy cold showers.") + System.Environment.NewLine; }
        if (applicant.traits["Heat_lover"]) { strengths += localizer.GetLocalizedText("They claim they take bathes in lava.") + System.Environment.NewLine; }
        if (applicant.traits["Extrovert"]) { strengths += localizer.GetLocalizedText("I was not able to make them stop talking.") + System.Environment.NewLine; }
        if (applicant.traits["loyal"]) { strengths += localizer.GetLocalizedText("They married their highschool sweetheart.") + System.Environment.NewLine; }
        if (strengths == "") { strengths = localizer.GetLocalizedText("They could not come up with anything that makes them special."); }
        resumeStrengths.text = strengths;


        //weaknesses
        string weaknesses = "";
        if (applicant.traits["weather_fearful"]) { weaknesses += localizer.GetLocalizedText("They lose the ability to drive then it rains, even just a little.") + System.Environment.NewLine; }
        if (applicant.traits["vacationer"]) { weaknesses += localizer.GetLocalizedText("They say that they have already put PTO in for every holiday.") + System.Environment.NewLine; }
        if (applicant.traits["stressful"]) { weaknesses += localizer.GetLocalizedText("They claim that they get a massage everyday after work.") + System.Environment.NewLine; }
        if (applicant.traits["slow_learner"]) { weaknesses += localizer.GetLocalizedText("Had to repeat the questions several times.") + System.Environment.NewLine; }
        if (applicant.traits["burnt_out"]) { weaknesses += localizer.GetLocalizedText("They say that they turn their phone off once they get home.") + System.Environment.NewLine; }
        if (applicant.traits["slow_walker"]) { weaknesses += localizer.GetLocalizedText("They like to take their time getting from place to place.") + System.Environment.NewLine; }
        if (applicant.traits["slow_worker"]) { weaknesses += localizer.GetLocalizedText("They like to take their time to make sure they do it right.") + System.Environment.NewLine; }
        if (applicant.traits["young"]) { weaknesses += localizer.GetLocalizedText("They say that they are not as slow as old people.") + System.Environment.NewLine; }
        if (applicant.traits["old"]) { weaknesses += localizer.GetLocalizedText("They say they are not as dumb as young people.") + System.Environment.NewLine; }
        if (applicant.traits["deaf"]) { weaknesses += localizer.GetLocalizedText("They are deaf in their right ear... and their left.") + System.Environment.NewLine; }
        if (applicant.traits["unreliable"]) { weaknesses += localizer.GetLocalizedText("They called-in for their interview and showed up late to their second one.") + System.Environment.NewLine; }
        if (applicant.traits["Introvert"]) { weaknesses += localizer.GetLocalizedText("Talking seemed to have stressed them out.") + System.Environment.NewLine; }
        if (applicant.traits["Unfaithful"]) { weaknesses += localizer.GetLocalizedText("They bragged about how often they cheat on their partner.") + System.Environment.NewLine; }
        if (applicant.traits["Pyromaniac"]) { weaknesses += localizer.GetLocalizedText("They wouldn't put down their lighter.") + System.Environment.NewLine; }
        if (applicant.traits["Clumsy"]) { weaknesses += localizer.GetLocalizedText("They broke the chair that they sat on during the interview.") + System.Environment.NewLine; }
        if (applicant.traits["Angry"]) { weaknesses += localizer.GetLocalizedText("They were proud to admit that they are in anger management.") + System.Environment.NewLine; }
        if (applicant.traits["Lunatic"]) { weaknesses += localizer.GetLocalizedText("They said they like to smell flowers, perfume, people, and pigeons.") + System.Environment.NewLine; }
        if (weaknesses == "") { weaknesses = localizer.GetLocalizedText("They said that they do not have any weaknesses."); }
        resumeWeaknesses.text = weaknesses;

        bool isFemale = false;
        if (gender == "Female") { isFemale = true; }
        resumeCharacter.SetSprites(character[0], character[1], character[2], character[3], character[4], character[5], character[6], isFemale, characterSizes);

        this.applicant = applicant;
        ToolTip.Instance.ActivateTutorial(25);
    }
    public void DismissApplicant() { applicant.DismissMe(); }
    public void HireApplicant() { applicant.HireMe(); }
    public void BanApplicant() { applicant.applicant.applyBanned = true; applicant.DismissMe(); }
    public void MakeWallIntoEntrance()
    {
        if (selectedWall != null)
        {
            selectedWall.BecomeEntrance();
        }
        DeSelectPopUp();
    }
    public void MakeEntranceIntoWall()
    {
        if (selectedWall != null)
        {
            selectedWall.BecomeWall();
        }
        DeSelectPopUp();
    }
    public void ChangeEntranceType(int number)
    {
        if (selectedWall != null)
        {
            selectedWall.ChangeEntranceType(number);
        }
    }
    public void DisplayBuyZone(int number)
    {
        if (!MapController.Instance.boughtZones[number])
        {
            MapController.Instance.selectedZone = number;
            float price = MapController.Instance.GetZoneCosts(number);
            DeActivateAll();

            SetBottomAnimatorString("Select");

            selectPop.GetChild(5).gameObject.SetActive(true);
            selectPop.GetChild(5).GetChild(2).GetComponent<TextMeshProUGUI>().text = localizer.GetLocalizedText("Zone: ") + number.ToString();
            selectPop.GetChild(5).GetChild(3).GetComponent<TextMeshProUGUI>().text = 
                (price * 10).ToString("f2") + "$" + System.Environment.NewLine +
                localizer.GetLocalizedText("Per month: ") + price.ToString("f2") + "$"
                ;

            if (Controller.Instance.money >= price) { selectPop.GetChild(5).GetChild(4).GetComponent<Button>().interactable = true; }
            else { selectPop.GetChild(5).GetChild(4).GetComponent<Button>().interactable = false; }
            selectPop.GetChild(5).GetChild(4).gameObject.SetActive(true);
            selectPop.GetChild(5).GetChild(5).gameObject.SetActive(false);
            selectPop.GetChild(5).GetChild(0).GetComponent<Image>().color = MapController.Instance.zoneColors[number];
        }
    }
    public void DisplayZoneSettings(int number)
    {
        if (uiBottomAnimatior.GetBool("Select") == false)
        {
            if (MapController.Instance.boughtZones[number] && allowZoneSettings)
            {
                MapController.Instance.selectedZone = number;
                DeActivateAll();
                SetBottomAnimatorString("Select");
                selectPop.GetChild(5).gameObject.SetActive(true);
                selectPop.GetChild(5).GetChild(2).GetComponent<TextMeshProUGUI>().text = localizer.GetLocalizedText("Zone: ") + number.ToString();
                selectPop.GetChild(5).GetChild(3).GetComponent<TextMeshProUGUI>().text = "Customers allowed to enter this zone?";
                selectPop.GetChild(5).GetChild(4).gameObject.SetActive(false);
                selectPop.GetChild(5).GetChild(5).gameObject.SetActive(true);

                if (MapController.Instance.custoemrAllowedZones[MapController.Instance.selectedZone]) { selectPop.GetChild(5).GetChild(5).GetComponent<SettingsButton>().Enable(); }
                else { selectPop.GetChild(5).GetChild(5).GetComponent<SettingsButton>().Disable(); }

                selectPop.GetChild(5).GetChild(0).GetComponent<Image>().color = MapController.Instance.zoneColors[number];
            }
        }

    }
    public void ToggleCustomerAllowedZone()
    {
        if (MapController.Instance.custoemrAllowedZones[MapController.Instance.selectedZone]) 
        { 
            MapController.Instance.custoemrAllowedZones[MapController.Instance.selectedZone] = false;
        }
        else 
        { 
            MapController.Instance.custoemrAllowedZones[MapController.Instance.selectedZone] = true;
        }
    }
    public void SpawnMapCompetitor(Competitor comp)
    {
        GameObject uiComp = Instantiate(mapPin, map);
        uiComp.GetComponent<MapPin>().competitor = comp;
        uiComp.GetComponent<MapPin>().StartUp();
    }
    public MapPin CreateMapPin(Competitor comp)
    {
        GameObject uiComp = Instantiate(mapPin, map);
        return uiComp.GetComponent<MapPin>();
    }
    public void ChangePolicy(string urName, string setting, float value)
    {
        switch (urName)
        {
            case "Cost of entry:": Controller.Instance.customerEntry = value; break;
            case "Membership:": Controller.Instance.customerMemberships = value; break;
            case "Pay bills:": Controller.Instance.billpayments = setting; break;
            case "Pay employees:": Controller.Instance.employeePaychecks = setting; break;
        }
    }
    public void TurnOffBuidling()
    {
        if (selectedBuilding.turnedOff) { selectedBuilding.turnedOff = false; selectedBuilding.TurnOn(); }
        else { selectedBuilding.turnedOff = true; selectedBuilding.TurnOff(); }
    }
    private void EmployeeOfTheMonth(int cast)
    {
        TickSystem.Instance.PauseMenu();
        SetMainAnimatorString("ToEmployeeOfTheMonth");

        switch(cast)
        {
            case 0: eotmCON.EOTM(); break;
            case 1: eotmCON.EOTS(); break;
            case 2: eotmCON.EOTY(); break;
        }
    }
    public void SpawnEOTM(Employee2 employee)
    {
        GameObject uiEOTM = Instantiate(eotm, eotmZone);
        uiEOTM.GetComponent<EOTM>().employee = employee;
        employee.myEOTM = uiEOTM.GetComponent<EOTM>();
        uiEOTM.GetComponent<EOTM>().StartUp();
    }

    [Space(20)]
    [Header("Deleters")]
    [SerializeField] private GameObject temp;
    [SerializeField] private GameObject comp;
    [SerializeField] private GameObject fin;
    [SerializeField] private GameObject store;
    //[SerializeField] private GameObject tasks;
    [SerializeField] private GameObject maps;
    [SerializeField] private GameObject hMaps;
    private void UIDeleter()
    {
        if (TransitionController.Instance.numberOfCompetitors == 0 && TransitionController.Instance.numberOfSpecialCompetitors == 0)
        {
            comp.SetActive(false);
            maps.SetActive(false);
        }
        if (TransitionController.Instance.jobAmount == 1)
        {
            //tasks.SetActive(false);
        }
        switch(TransitionController.Instance.tutorialLevel)
        {
            case 0: break;
            case 1: store.SetActive(false); fin.SetActive(false); hMaps.SetActive(false); break;
            case 2: store.SetActive(false); hMaps.SetActive(false); break;
            case 3: break;
            case 4: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 8: break;
            case 9: break;
        }
        if (TransitionController.Instance.highTemp <= 80 && TransitionController.Instance.lowTemp >= 70)
        {
            temp.SetActive(false);
        }

        //builds
    }
    [Space(10)]
    [Header("Other")]
    [SerializeField] private Button giveRaiseButton;
    [SerializeField] private Image employeeRaiseHappyImage;
    [SerializeField] private List<Sprite> emotions = new List<Sprite>();
    public void GiveRaise()
    {
        if (selectedEmployee != null)
        {
            selectedEmployee.hourlyWage += 0.25f;
            UpdateEmployeeRaiseHappiness();
            EmployeeSelect(selectedEmployee);
        }
    }
    private void UpdateEmployeeRaiseHappiness()
    {
        if (selectedEmployee != null)
        {
            if (selectedEmployee.status != Employee2.Status.owner)
            {
                if (selectedEmployee.hourlyWage >= selectedEmployee.desiredWage - 0.25f && selectedEmployee.hourlyWage < selectedEmployee.desiredWage + 0.25f) { SetEmployeeRaiseHappiness(3); return;  }

                if (selectedEmployee.hourlyWage >= selectedEmployee.desiredWage + 0.25f && selectedEmployee.hourlyWage <= selectedEmployee.desiredWage + 0.5f) { SetEmployeeRaiseHappiness(3); return; }
                if (selectedEmployee.hourlyWage > selectedEmployee.desiredWage + 0.5f) { SetEmployeeRaiseHappiness(4); return; }

                if (selectedEmployee.hourlyWage <= selectedEmployee.desiredWage - 0.25f && selectedEmployee.hourlyWage > selectedEmployee.desiredWage - 0.5f) { SetEmployeeRaiseHappiness(2); return; }
                if (selectedEmployee.hourlyWage <= selectedEmployee.desiredWage - 0.5f && selectedEmployee.hourlyWage > selectedEmployee.desiredWage - 1f) { SetEmployeeRaiseHappiness(1); return; }
                if (selectedEmployee.hourlyWage <= selectedEmployee.desiredWage - 1) { SetEmployeeRaiseHappiness(0); return; }
            }
        }
    }
    private void SetEmployeeRaiseHappiness(int spriteNum) { employeeRaiseHappyImage.sprite = emotions[spriteNum]; }
    [HideInInspector] public Competitor activeCompetitor;
    public void BuyoutCompetitor()
    {
        Controller.Instance.MoneyValueChange(-activeCompetitor.mapPin.buyoutFloat, UtilsClass.GetMouseWorldPosition(), true, true);
        activeCompetitor.DeclareBankrupty();
        activeCompetitor.mapPin.BuyoutInteractableChecker();
    }
    private int daysSinceLastAttempt = 100;
    private IEnumerator BuyoutAttempt()
    {
        daysSinceLastAttempt++;

        if (daysSinceLastAttempt >= 7)
        {
            float offer = Controller.Instance.money;
            offer += (Controller.Instance.employees.Count * 100);
            foreach (Item item in FindObjectsOfType<Item>())
            {
                offer += item.itemSO.cost; yield return new WaitForEndOfFrame();
            }
            foreach (Building building in Controller.Instance.buildings)
            {
                offer += building.sellValue; yield return new WaitForEndOfFrame();
            }

            if (offer >= (TransitionController.Instance.moneyWinAmount / TransitionController.Instance.totalDifficulty))
            {
                //good ending
                goodOfferButton.interactable = true;
                neutralOfferButton.interactable = false;
                badOfferButton.interactable = false;
            }
            else if (offer >= (TransitionController.Instance.moneyWinAmount / TransitionController.Instance.totalDifficulty) / 2)
            {
                //nutrel ending
                goodOfferButton.interactable = false;
                neutralOfferButton.interactable = true;
                badOfferButton.interactable = false;
            }
            else { goodOfferButton.interactable = false; neutralOfferButton.interactable = false; badOfferButton.interactable = true; }

            offerAmountText.text = offer.ToString("f2");
            TickSystem.Instance.PauseMenu();
            SetMainAnimatorString("ToSellOffer");
            daysSinceLastAttempt = 0;
        }
    }
    public void acceptOffer(int number)
    {
        switch(number)
        {
            case 1: GameOver(false, "You have sold the store at a loss.", false); break;
            case 2: GameOver(true, "You broke even on your sale.", false); break;
            case 3: GameOver(true, "You sold the store for a huge profit!", false); break;
        }
        TickSystem.Instance.UnPause();
    }
    public void ChangeWorkDay(string day)
    {
        if (Controller.Instance.storeOpenDays.ContainsKey(day))
        {
            if (Controller.Instance.storeOpenDays[day] == true) { Controller.Instance.storeOpenDays[day] = false; }
            else { Controller.Instance.storeOpenDays[day] = true; }
        }
        else
        {
            Controller.Instance.storeOpenDays.Add(day, false);
        }
    }

    public void CreateGoal(string goalString, string disc, string reward, float amount, ItemSO item, Goals myGoal, Rewards myReward, float rewardAmount, float deadline, float progress)
    {
        if (myReward != Rewards.win)
        {
            if (TransitionController.Instance.tutorialLevel == 1) { return; }
            else { ToolTip.Instance.ActivateTutorial(42); }
        }
        GameObject spawn = Instantiate(goal, goalZone);
        spawn.GetComponent<Goal>().StartUp(goalString, disc, reward, amount, item, myGoal, myReward, rewardAmount, deadline, progress);
        activeGoals.Add(spawn.GetComponent<Goal>());
    }
    public void ChangeStoreName()
    {
        Controller.Instance.storeName = storeNamer.text;
        myStorePin.storeName = storeNamer.text;
    }
    public void RandomQuestChance()
    {
        if (TransitionController.Instance.tutorialLevel == 1) { return; }
        if (Random.Range(0, 100) > (100 - TransitionController.Instance.chanceOfQuests))
        {
            int type = Random.Range(0, 8);
            float amount = 0;
            ItemSO item = null;
            int itemIs = 0;
            List<ItemSO> possibleItems = new List<ItemSO>();
            switch (type)
            {
                //money
                case 0:
                    amount = Controller.Instance.itemsSoldDaily * 10;//Controller.Instance.money / 2;
                    if (amount < 1000) { amount = 1000; }
                    CreateGoal("Make money bet", localizer.GetLocalizedText("Make ") + amount, "Double that money!", amount, null, Goals.moneyMade, Rewards.money, amount, 24, 0);
                    break; 

                //items
                case 1:
                    foreach (ItemSO items in Controller.Instance.items)
                    {
                        //years
                        if (items.year_Start <= year && items.year_End > year)
                        {
                            //special item  //seasons fruit
                            if (!items.special && !items.seasonal) { possibleItems.Add(items); }
                            else if (Controller.Instance.unlockedSpecialItems.Contains(items)) { possibleItems.Add(items); }
                        }
                    }
                    itemIs = Random.Range(0, possibleItems.Count);
                    item = possibleItems[itemIs];
                    if (Controller.Instance.items[itemIs].special && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[itemIs])) { return; }
                    amount = Controller.Instance.itemsSoldDaily / 3;
                    if (amount < 25) { amount = 25; }
                    CreateGoal("Sell Item Bet", localizer.GetLocalizedText("Sell 100 ") + item.myName, "Double reimbursed costs", 100, item, Goals.itemsSold, Rewards.money, amount, 24, 0);
                    break;

                //customers
                case 2:
                    amount = UIController.Instance.myMarketShare + 0.04f;
                    CreateGoal("Customer Interest Bet", localizer.GetLocalizedText("Increase market share to: ") + (amount * 100) + localizer.GetLocalizedText(" points"), "10,000$!", amount, null, Goals.marketShare, Rewards.money, 10000, 24, 0);
                    break;

                //money Total
                case 3:
                    amount = Controller.Instance.money * 1.25f;
                    if (amount < 5000) { amount = 5000; }
                    CreateGoal("Collect Money", localizer.GetLocalizedText("Have ") + amount + localizer.GetLocalizedText(" in your account"), "more money!", amount, null, Goals.moneyTotal, Rewards.money, amount / 2, 24, 0);
                    break;

                //spend
                case 4:
                    amount = Controller.Instance.money * 1.1f;
                    if (amount < 2500) { amount = 2500; }
                    CreateGoal("Spend money bet", localizer.GetLocalizedText("Spend ") + amount, "Double reimbursed costs!", amount, null, Goals.moneySpent, Rewards.money, amount * 2, 24, 0);
                    break;

                //special Item
                case 5:
                    foreach(ItemSO items in Controller.Instance.items)
                    {
                        if (items.special && !Controller.Instance.unlockedSpecialItems.Contains(items)) { possibleItems.Add(items); }
                    }
                    if (possibleItems.Count > 0)
                    {
                        itemIs = Random.Range(0, possibleItems.Count);
                        item = possibleItems[itemIs];
                        amount = Controller.Instance.itemsSoldDaily * 5;
                        if (amount < 250) { amount = 250; }
                        CreateGoal("New special item looking for sellers", localizer.GetLocalizedText("Sell ") + amount.ToString() + localizer.GetLocalizedText(" of ") + item.myName, "New unique item to sell", amount, item, Goals.itemsSold, Rewards.specialItem, 0, 168, 0);
                        Controller.Instance.NewItemCheck(item);
                        foreach (Customer2 customer in Controller.Instance.customers) {; }
                    }
                    break;

                //manufactoror
                case 6:
                    foreach (ItemSO items in Controller.Instance.items)
                    {
                        //years
                        if (items.year_Start <= year && items.year_End > year)
                        {
                            //special item  //seasons fruit
                            if (!items.special && !items.seasonal) { possibleItems.Add(items); }
                            else if (Controller.Instance.unlockedSpecialItems.Contains(items)) { possibleItems.Add(items); }
                        }
                    }
                    itemIs = Random.Range(0, possibleItems.Count);
                    item = possibleItems[itemIs];
                    if (Controller.Instance.items[itemIs].special && !Controller.Instance.unlockedSpecialItems.Contains(Controller.Instance.items[itemIs])) { return; }
                    amount = Controller.Instance.itemsSoldDaily;
                    if (amount < 50) { amount = 50; }
                    int rewardAmount = Random.Range(0, 4);//amount of special manufactorers
                    //check to see if already won this manufacterer
                    CreateGoal("New manufacturer looking for distributer", localizer.GetLocalizedText("Sell ") + amount.ToString() + localizer.GetLocalizedText(" of ") + item.myName, localizer.GetLocalizedText("New manufacturer option for: ") + localizer.GetLocalizedText(item.myName), amount, item, Goals.itemsSold, Rewards.specialManufacturer, rewardAmount, 72, 0);
                    break;

                //special Customer
                case 7:
                    amount = UIController.Instance.myMarketShare * 2.35f;
                    if (amount < 50) { amount = 50; }
                    CreateGoal("Celeberty store impression", "Impress a local celeberty", "Celeberty WILL blog about their experience!", 100, null, Goals.specialCustomer, Rewards.increaseMarketShare, 0, 24, 0);
                    break;
                    //customer satisfaction?
                    //cleanliness?
            }
        }
    }
    public List<Goal> activeGoals = new List<Goal>();
    public void LoadQuest(int type, float amount, string itemName, float progress, float timeRemaining, int rewardInt, bool winCondition)
    {
        if (!winCondition)
        {
            ItemSO item = null;
            foreach (ItemSO items in Controller.Instance.items)
            {
                if (items.myName == itemName) { item = items; break; }
            }
            switch (type)
            {
                case 0: CreateGoal("Make money bet", localizer.GetLocalizedText("Make ") + amount, "Double that money!", amount, null, Goals.moneyMade, Rewards.money, amount, timeRemaining, progress); break;
                case 1: CreateGoal("Sell Item Bet", localizer.GetLocalizedText("Sell 100 ") + itemName, "Double reimbursed costs", 100, item, Goals.itemsSold, Rewards.money, amount, timeRemaining, progress); break;
                case 2: CreateGoal("Customer Interest Bet", localizer.GetLocalizedText("Increase market share to: ") + (amount * 100) + localizer.GetLocalizedText(" points"), "10,000$!", amount, null, Goals.marketShare, Rewards.money, 10000, timeRemaining, progress); break;
                case 3: CreateGoal("Collect Money", localizer.GetLocalizedText("Have ") + amount + localizer.GetLocalizedText(" in your account"), "more money!", amount, null, Goals.moneyTotal, Rewards.money, amount / 2, timeRemaining, progress); break;
                case 4: CreateGoal("Spend money bet", localizer.GetLocalizedText("Spend ") + amount, "Double reimbursed costs!", amount, null, Goals.moneySpent, Rewards.money, amount * 2, timeRemaining, progress); break;
                case 5: CreateGoal("New special item looking for sellers", localizer.GetLocalizedText("Sell ") + amount.ToString() + localizer.GetLocalizedText(" of ") + itemName, "New unique item to sell", amount, item, Goals.itemsSold, Rewards.specialItem, 0, timeRemaining, progress); break;
                case 6: CreateGoal("New manufacturer looking for distributer", localizer.GetLocalizedText("Sell ") + amount.ToString() + localizer.GetLocalizedText(" of ") + itemName, localizer.GetLocalizedText("New manufacturer option for: ") + localizer.GetLocalizedText(itemName), amount, item, Goals.itemsSold, Rewards.specialManufacturer, rewardInt, timeRemaining, progress); break;
                case 7: CreateGoal("Celeberty store impression", "Impress a local celeberty", "Celeberty WILL blog about their experience!", 100, null, Goals.specialCustomer, Rewards.increaseMarketShare, 0, timeRemaining, progress); break;
            }
        }
        else
        {
           CreateGoal(TransitionController.Instance.mapName, TransitionController.Instance.goalDisc, TransitionController.Instance.goalreward, TransitionController.Instance.goalAmount, null, TransitionController.Instance.goal, Goal.Rewards.win, 0, -1, progress);
        }

    }
    public void GetBuildingColors(int colorSelect)
    {
        List<Color> baseColors = CharacterVisualCon.Instance.GeneratreBaseColors(selectedBuilding.baseColorType);
        List<Color> mainColors = CharacterVisualCon.Instance.GeneratreMainColors(selectedBuilding.mainColorType);

        if (colorSelect != -1)
        {
            for (int i = 0; i < buildingColorButtons.Count; i++)
            {
                buildingColorButtons[i].transform.GetChild(0).GetComponent<Image>().color = baseColors[i];
                buildingColorButtons[i].transform.GetChild(1).GetComponent<Image>().color = mainColors[i];

                if (i == colorSelect) { buildingColorButtons[i].interactable = false; }
                else { buildingColorButtons[i].interactable = true; }
            }

            foreach(Building build in Controller.Instance.selectedBuildings) { build.ChangeColors(buildingColorButtons[colorSelect].transform.GetChild(0).GetComponent<Image>().color, buildingColorButtons[colorSelect].transform.GetChild(1).GetComponent<Image>().color); build.selectedColorChoice = colorSelect; }
        }
        else
        {
            for (int i = 0; i < buildingColorButtons.Count; i++)
            {
                buildingColorButtons[i].transform.GetChild(0).GetComponent<Image>().color = baseColors[i];
                buildingColorButtons[i].transform.GetChild(1).GetComponent<Image>().color = mainColors[i];

                buildingColorButtons[i].interactable = true;
            }
        }

        buildingColorButtons[0].transform.parent.parent.gameObject.SetActive(true);

    }
    public void ToggleStoreCredit()
    {
        if (Controller.Instance.storeCredit) { Controller.Instance.storeCredit = false; }
        else { Controller.Instance.storeCredit = true; }
    }
    public void GetModdedSettings(float minWage, float maxWage, int minAdvert, int maxAdvert)
    {
        hiringWageSlider.minValue = (int)(minWage * 4); hiringWageSlider.maxValue = (int)(maxWage * 4);
        advertising.GetModdedValues(minAdvert, maxAdvert);
    }
    [SerializeField] private GameObject newLoan;
    [SerializeField] private Transform loanHolder;
    [SerializeField] private List<Transform> allLoans = new List<Transform>();
    public void SpawnLoan(int amount, float interest, string name)
    {
        GameObject loan = Instantiate(newLoan, loanHolder);
        Loan thisLoan = newLoan.GetComponent<Loan>();
        thisLoan.StartUp(amount, interest, name);
        allLoans.Add(loan.transform);
    }
    public void LoadLoans(float amount, string name)
    {
        foreach(Transform loant in allLoans)
        {
            Loan loan = loant.GetComponent<Loan>();
            if (loan.myName == name)
            {
                loan.LoadLoan(amount);
            }
        }
    }
    [SerializeField] private GameObject spawnButton;
    [SerializeField] private Transform buildingOptionsHolder;
    public void SpawnBuildOption(BuildingSO toBuild)
    {
        GameObject option = Instantiate(spawnButton, buildingOptionsHolder);
        BuildButton buildOption = option.transform.GetChild(0).GetComponent<BuildButton>();

        buildOption.toBuild = toBuild;
        buildOption.floorName = "";

        if (toBuild != null)
        {
            if (toBuild.type == BuildingSO.Type.lights) { buildOption.buildLayer = 2; }
            else { buildOption.buildLayer = 8; }
        }
        buildOption.StartUp();
    }
    public void SpawnTileBuildOption(Sprite sprite, string tileName)
    {
        GameObject option = Instantiate(spawnButton, buildingOptionsHolder);
        BuildButton buildOption = option.transform.GetChild(0).GetComponent<BuildButton>();

        buildOption.toBuild = null;
        buildOption.floorName = tileName;

        buildOption.StartUp();
        buildOption.myImage.sprite = sprite;
    }

    [SerializeField] private TextMeshProUGUI stockingAutoTaskText;
    [SerializeField] private TextMeshProUGUI casheringAutoTaskText;
    [SerializeField] private TextMeshProUGUI cleaningAutoTaskText;
    [SerializeField] private TextMeshProUGUI buildingAutoTaskText;
    [SerializeField] private TextMeshProUGUI managingAutoTaskText;
    public void RevertTasks()
    {
        if (Controller.Instance.revertToHiredTasks) { Controller.Instance.revertToHiredTasks = false; }
        else { Controller.Instance.revertToHiredTasks = true; }
    }
    public void AutoTasker(int task)
    {
        switch(task)
        {
            case 1:
                switch(Controller.Instance.ifDoneStocking)
                {
                    default: Controller.Instance.ifDoneStocking = "Go home"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Go home"); break;
                    case "Go home":
                        if (TransitionController.Instance.tutorialLevel > 1) { Controller.Instance.ifDoneStocking = "cashier"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("cashier"); }
                        else { Controller.Instance.ifDoneStocking = "Do nothing"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "cashier":
                        if (TransitionController.Instance.tutorialLevel > 2) { Controller.Instance.ifDoneStocking = "janitor"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("janitor"); }
                        else { Controller.Instance.ifDoneStocking = "Do nothing"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "janitor":
                        if (TransitionController.Instance.tutorialLevel > 3) { Controller.Instance.ifDoneStocking = "engineer"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("engineer"); }
                        else { Controller.Instance.ifDoneStocking = "Do nothing"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "engineer":
                        if (TransitionController.Instance.tutorialLevel > 4) { Controller.Instance.ifDoneStocking = "manager"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("manager"); }
                        else { Controller.Instance.ifDoneStocking = "Do nothing"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "manager":
                        Controller.Instance.ifDoneStocking = "Do nothing"; stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing");
                        break;
                }
                break;

            case 2:
                switch (Controller.Instance.ifDoneCashiering)
                {
                    default: Controller.Instance.ifDoneCashiering = "Go home"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("Go home"); break;
                    case "Go home": Controller.Instance.ifDoneCashiering = "stocker"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("stocker"); break;
                    case "stocker":
                        if (TransitionController.Instance.tutorialLevel > 2) { Controller.Instance.ifDoneCashiering = "janitor"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("janitor"); }
                        else { Controller.Instance.ifDoneCashiering = "Do nothing"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "janitor":
                        if (TransitionController.Instance.tutorialLevel > 3) { Controller.Instance.ifDoneCashiering = "engineer"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("engineer"); }
                        else { Controller.Instance.ifDoneCashiering = "Do nothing"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "engineer":
                        if (TransitionController.Instance.tutorialLevel > 4) { Controller.Instance.ifDoneCashiering = "manager"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("manager"); }
                        else { Controller.Instance.ifDoneCashiering = "Do nothing"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "manager":
                        Controller.Instance.ifDoneCashiering = "Do nothing"; casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing");
                        break;
                }
                break;
            case 3:
                switch (Controller.Instance.ifDoneCleaning)
                {
                    default: Controller.Instance.ifDoneCleaning = "Go home"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("Go home"); break;
                    case "Go home": Controller.Instance.ifDoneCleaning = "stocker"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("stocker"); break;
                    case "stocker":
                        if (TransitionController.Instance.tutorialLevel > 1) { Controller.Instance.ifDoneCleaning = "cashier"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("cashier"); }
                        else { Controller.Instance.ifDoneCleaning = "Do nothing"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "cashier":
                        if (TransitionController.Instance.tutorialLevel > 3) { Controller.Instance.ifDoneCleaning = "engineer"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("engineer"); }
                        else { Controller.Instance.ifDoneCleaning = "Do nothing"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "engineer":
                        if (TransitionController.Instance.tutorialLevel > 4) { Controller.Instance.ifDoneCleaning = "manager"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("manager"); }
                        else { Controller.Instance.ifDoneCleaning = "Do nothing"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "manager":
                        Controller.Instance.ifDoneCleaning = "Do nothing"; cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing");
                        break;
                }
                break;
            case 4:
                switch (Controller.Instance.ifDoneBuilding)
                {
                    default: Controller.Instance.ifDoneBuilding = "Go home"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Go home"); break;
                    case "Go home": Controller.Instance.ifDoneBuilding = "stocker"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("stocker"); break;
                    case "stocker":
                        if (TransitionController.Instance.tutorialLevel > 1) { Controller.Instance.ifDoneBuilding = "cashier"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("cashier"); }
                        else { Controller.Instance.ifDoneBuilding = "Do nothing"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "cashier":
                        if (TransitionController.Instance.tutorialLevel > 2) { Controller.Instance.ifDoneBuilding = "janitor"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("janitor"); }
                        else { Controller.Instance.ifDoneBuilding = "Do nothing"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "janitor":
                        if (TransitionController.Instance.tutorialLevel > 4) { Controller.Instance.ifDoneBuilding = "manager"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("manager"); }
                        else { Controller.Instance.ifDoneBuilding = "Do nothing"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "manager":
                        Controller.Instance.ifDoneBuilding = "Do nothing"; buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing");
                        break;
                }
                break;
            case 5:
                switch (Controller.Instance.ifDoneManaging)
                {
                    default: Controller.Instance.ifDoneManaging = "Go home"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Go home"); break;
                    case "Go home": Controller.Instance.ifDoneManaging = "stocker"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("stocker"); break;
                    case "stocker":
                        if (TransitionController.Instance.tutorialLevel > 1) { Controller.Instance.ifDoneManaging = "cashier"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("cashier"); }
                        else { Controller.Instance.ifDoneManaging = "Do nothing"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "cashier":
                        if (TransitionController.Instance.tutorialLevel > 2) { Controller.Instance.ifDoneManaging = "janitor"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("janitor"); }
                        else { Controller.Instance.ifDoneManaging = "Do nothing"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "janitor":
                        if (TransitionController.Instance.tutorialLevel > 3) { Controller.Instance.ifDoneManaging = "engineer"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("engineer"); }
                        else { Controller.Instance.ifDoneManaging = "Do nothing"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing"); }
                        break;
                    case "engineer":
                        Controller.Instance.ifDoneManaging = "Do nothing"; managingAutoTaskText.text = Localizer.Instance.GetLocalizedText("Do nothing");
                        break;
                }
                break;
        }
    }
    [SerializeField] SettingsButton revertButton;
    [SerializeField] SettingsButton[] priorityButtons;
    [SerializeField] UIScheduler[] storeSchedues;
    [SerializeField] SettingsButton[] storeOpenDaysButtons;
    [SerializeField] SettingsButton storeCreditButton;
    [SerializeField] Policy[] policies;
    public void UpdateAutoTaskUI()
    {
        stockingAutoTaskText.text = Localizer.Instance.GetLocalizedText(Controller.Instance.ifDoneStocking);
        casheringAutoTaskText.text = Localizer.Instance.GetLocalizedText(Controller.Instance.ifDoneCashiering);
        cleaningAutoTaskText.text = Localizer.Instance.GetLocalizedText(Controller.Instance.ifDoneCleaning);
        buildingAutoTaskText.text = Localizer.Instance.GetLocalizedText(Controller.Instance.ifDoneBuilding);
        managingAutoTaskText.text = Localizer.Instance.GetLocalizedText(Controller.Instance.ifDoneManaging);
        if (Controller.Instance.revertToHiredTasks) { revertButton.Enable(); }
        foreach(string task in Controller.Instance.priorityTask)
        {
            switch(task)
            {
                case "stocker": priorityButtons[0].Enable(); break;
                case "cashier": priorityButtons[1].Enable(); break;
                case "janitor": priorityButtons[2].Enable(); break;
                case "engineer": priorityButtons[3].Enable(); break;
                case "manager": priorityButtons[4].Enable(); break;
            }
        }
        UITempController.Instance.LoadedUpdateTemp();
        foreach (UIScheduler storeSCH in storeSchedues) { storeSCH.LoadedUpdate(); }
        foreach (KeyValuePair<string, bool> pair in Controller.Instance.storeOpenDays)
        {
            foreach(SettingsButton dayButton in storeOpenDaysButtons)
            {
                if (dayButton.gameObject.name == pair.Key) { if (pair.Value == false) { dayButton.Disable(); } }
            }
        }
        storeNamer.text = Controller.Instance.storeName;
        myStorePin.storeName = storeNamer.text;
        if (Controller.Instance.storeCredit) { storeCreditButton.Enable(); }
        foreach (Policy policy in policies) { policy.LoadedUpdate(); }
    }
    public void PriorityTask(string task)
    {
        if (Controller.Instance.priorityTask.Contains(task)) { Controller.Instance.priorityTask.Remove(task); }
        else { Controller.Instance.priorityTask.Add(task); }
    }
}