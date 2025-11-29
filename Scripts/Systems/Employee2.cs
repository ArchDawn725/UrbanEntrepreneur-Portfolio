using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using ArchDawn.Utilities;
using static MapController;
using Random = UnityEngine.Random;
using System.Net.Sockets;

public class Employee2 : MonoBehaviour
{
    public enum Objective
    {
        absent,
        idle,
        working,
    }
    public enum Task
    {
        nothing,
        stocker,
        cashier,
        janitor,
        engineer,
        manager,
    }
    public enum Status
    {
        employee,
        employeeOfTheMonth,
        employeeOfTheSeason,
        employeeOfTheYear,
        employeeOfTheDecade,
        owner
    }

    public event EventHandler OnObjectiveValueChanged;
    public event EventHandler OnTaskValueChanged;
    public event EventHandler OnFired;

    public string birthName;
    public string myName;
    public Objective objective;
    public Task task;
    public Status status;
    public float age;
    public bool isFemale;

    [Space(10)]
    [Header("Stats")]
    [SerializeField] private float wageDue;
    public float hourlyWage;
    public float desiredWage;
    public int workStart;
    public int workEnd;

    public float stress;
    public float longTermStress;

    private const float MAX_SPEED = 100;
    private const float MIN_SPEED = 3;
    private const float MAX_TICKS = 100;
    private const float MIN_TICKS = 5;
    private const float MAX_STRESS = 3.125f;
    private const float MIN_STRESS = 0.14881f;
    public float speedCalc = 30;


    //public float distanceTest = 10;
    //public float speedTest = 30;
    //public float GetSpeed(MapController.NewGrid newGrid) { return speedTest; }

    public float GetSpeed(MapController.NewGrid newGrid) { return Math.Clamp((stats["baseSpeed"] + GetAgeMultiplier() + GetTotalLevel() + GetStressMultiplier() + GetCleanMultiplier() + (Controller.Instance.musicSpeedBonus * stats["sensitive"]) + GetLightMultiplier() + newGrid.GetSpeed() + GetStatusBonus()), MIN_SPEED, MAX_SPEED) * TickSystem.Instance.timeMultiplier; }
    public int GetWorkTicks() { return (int)Math.Clamp((stats["baseWorkSpeed"] - (GetAgeMultiplier() + GetStressMultiplier() + GetCleanMultiplier() + (Controller.Instance.musicWorkBonus * stats["sensitive"]) + GetLightMultiplier() + GetStatusBonus())), MIN_TICKS, MAX_TICKS); }
    public float GetAgeMultiplier() { return ((float)(age * -1f) / 5f) + 10; } //-10 , +10
    //public int GetTotalLevel() { return Math.Clamp((managementSkill + customerServiceSkill + inventorySkill + janitorialSkill + engineerSkill + managementSkill) / 6, 0 , 20) ; }
    public int GetTotalLevel() { return (managementSkill + customerServiceSkill + inventorySkill + janitorialSkill + engineerSkill + managementSkill); }
    public int GetMyTotalLevel() { return (managementSkill + customerServiceSkill + inventorySkill + janitorialSkill + engineerSkill + managementSkill); }
    public float GetStressMultiplier() { return ((float)stress / -5f) + 10; }// - 10, + 10
    public float GetCleanMultiplier() { return ((GetGrid().GetCleanNormalized() * -1) * 15) + 10; } //-5, +10
    //music
    //audio sensitive
    public float GetLightMultiplier() { return (GetGrid().GetLightNormalized() * 15) - 5; } //-5 , +10
    //building speed reducer
    //manager
    //trained
    public int GetStatusBonus() { if (status != Status.owner) { return (int)status * 2; } else return 0; }//0, 8


    [Space(10)]
    [Header("Skills")]
    public List<float> trainingRequired = new List<float>();
    public int inventorySkill;
    public int customerServiceSkill;
    public int janitorialSkill;
    public int engineerSkill;
    public int managementSkill;

    private const float MAX_LEVEL = 20;
    private int levelTimesMultiplier = 50;

    //private int GetTotalLevel() { return (managementSkill + customerServiceSkill + inventorySkill + janitorialSkill + engineerSkill + managementSkill) / 25; }
    public void OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill) { invSkill = inventorySkill; custSkill = customerServiceSkill; janitorialSkill = this.janitorialSkill; engineerSkill = this.engineerSkill; managementSkill = this.managementSkill; }
    //hide
    [SerializeField] private float inventoryXP;
    [SerializeField] private float customerXP;
    [SerializeField] private float janitorXP;
    [SerializeField] private float engineerXP;
    [SerializeField] private float managerXP;

    public int capacity;

    //Hidden
    //Transforms
    private BoxCollider2D myCollider;
    private BoxCollider2D myCollider2;
    private Transform visuals;
    private Transform container;
    public void OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont) { box = myCollider; vis = visuals; cont = container; }
    private GameObject progressBar;
    private BarController progressBarCon;
    private GameObject xpBar;
    private BarController xpBarCon;
    private GameObject trainingBar;
    private BarController trainingBarCon;
    [SerializeField] private List<Sprite> taskSprites = new List<Sprite>();
    private SpriteRenderer taskImage;
    [HideInInspector] public UICharacter myUICharacter;
    private UICharacterSchedule myUICharacterSchedule;
    private UITaskCharacter myUITask;
    private Staff uiStaff;
    [HideInInspector] public Animator animator;
    private PersonVisualCon personVis;
    public EOTM myEOTM;
    private ChatMessage bubble;
    public void OutProgressBar(out GameObject bar, out BarController barCon) { bar = progressBar; barCon = progressBarCon; }

    //pathfinding
    private int currentPathIndex;
    public List<Vector3> pathVectorList;
    public Vector3 targetPosition;
    public void OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos) { currentPath = currentPathIndex; pathList = pathVectorList; targetPos = targetPosition; }
    public void SetPathfinding(in int currentPath, in List<Vector3> pathList, in Vector3 targetPos) { currentPathIndex = currentPath; pathVectorList = pathList; targetPosition = targetPos; }

    //AI
    public Transform target;
    private int newTask;
    //private int newTargetItemID;

    public void OutAI(out Transform targ, out int newTsk) { targ = target; newTsk = newTask; }
    public void SetAI(in Transform targ, in int newTsk) { target = targ; newTask = newTsk; }

    public Item selectedItem;
    public int targetItemID;

    public Building targetBuilding;

    public Building targetRegistor;
    public Building targetShelf;
    public Building targetStockPile;
    public Employee2 targetEmployee;
    public Customer2 targetCustomer;

    public Building newTargetBuilding;
    public Employee2 newTargetEmployee;
    public Vector2Int targetTile;
    public Vector2Int newTargetTile;

    public void GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding) { selectItem = selectedItem; targItemID = targetItemID; targBuilding = targetBuilding; targRegistor = targetRegistor; targShelf = targetShelf; targStockPile = targetStockPile; newTargBuilding = newTargetBuilding; }
    public void SetTargets(in Item selectItem, in int targItemID, in Building targBuilding, in Building targRegistor, in Building targShelf, in Building targStockPile, in Building newTargBuilding) { selectedItem = selectItem; targetItemID = targItemID; targetBuilding = targBuilding; targetRegistor = targRegistor; targetShelf = targShelf; targetStockPile = targStockPile; newTargetBuilding = newTargBuilding; }

    [Space(10)]
    [Header("Debugs")]
    [HideInInspector] public bool messageCalled;
    [SerializeField] private bool selected;
    [SerializeField] private Dictionary<int, bool> mySchedule = new Dictionary<int, bool>();//int == time //bool == working?    //needs day of the week
    public bool scanning;
    public bool moveTo;
    private Vector2Int previousHover;
    private Vector2Int currentHover;
    public void SetApperance(List<int> appearance, List<float> sizes) { transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().SetSprites(appearance[0], appearance[1], appearance[2], appearance[3], appearance[4], appearance[5], appearance[6], isFemale, sizes); }
    //public void SetTraits(int prefTime, float stress, float stressless, float learningS, float audio, float callOut, float speed, float work, float tempPreferance, List<int> appearance, bool female, float social, int loyal, List<float> sizes) { prefferedTime = prefTime; stressfulness = stress; stressRelease = stressless; learningSpeed = learningS; auditorySensitivity = audio; callOutChance = callOut; baseSpeed = speed; baseWorkSpeed = work; tempPref = tempPreferance; transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().SetSprites(appearance[0], appearance[1], appearance[2], appearance[3], appearance[4], appearance[5], appearance[6], isFemale, sizes); isFemale = female; socialSkills = social; loyalty = loyal; }
    public Dictionary<string, bool> traits = new Dictionary<string, bool>();
    public Dictionary<string, float> stats = new Dictionary<string, float>();
    //private int prefferedTime;
    //private float stressfulness;
    //private float stressRelease;
    //private float learningSpeed;
    //private float auditorySensitivity;
    //private float callOutChance;
    //private float tempPref;
    //private float socialSkills;
    //private int loyalty;
    public bool late;
    public bool calledOut;
    public bool sentHome;
    public Employee2 manager;
    public Dictionary<string, bool> workDays = new Dictionary<string, bool>();
    public int employeedForDays;
    public List<Building> claimedBuildings = new List<Building>();
    public float hoursWorkedThisWeek;
    public float hoursWorkedToday;


    public float averageStress;
    public float overtimeWorked;
    public int calledOutAmount;
    public int lateAmount;
    public List<Color> taskColors = new List<Color>();
    public int hiredTask;
    public List<Building> Memory = new List<Building>();
    public bool beforeLine;
    public bool insideStore;

    private void Awake() { ScheduleStartUP(); }
    private void OnMouseEnter() { transform.GetChild(0).GetChild(0).gameObject.SetActive(true); }
    private void OnMouseExit() { if (!selected) { transform.GetChild(0).GetChild(0).gameObject.SetActive(false); } }
    public void Selected() { selected = true; OnMouseEnter(); ToolTip.Instance.DismissTutorial(20); }
    public void Deselected() { selected = false; OnMouseExit(); if (Controller.Instance.selectedEmployee == this) { Controller.Instance.selectedEmployee = null; } }
    public void XPIncrease(int type)
    {
        float xp = 0; float xpNeeded = 100;
        float learningAmount = stats["learningSpeed"];
        if ((int)task != hiredTask && hiredTask != 0) { learningAmount /= 2;  }

        if (trainingRequired[type] > 0)
        {
            trainingRequired[type] -= learningAmount;
            if (trainingRequired[type] < 0) { trainingRequired[type] = 0; }
            trainingBarCon.Activate(trainingRequired[type] * 1f / 250);
            trainingBar.SetActive(true);
            trainingBar.GetComponent<FadeController>().Activate();
        }
        else
        {
            switch (type)
            {
                case 1: inventoryXP += learningAmount; xp = inventoryXP; xpNeeded = (inventorySkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) - 20; break;
                case 2: customerXP += learningAmount; xp = customerXP; xpNeeded = (customerServiceSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 0; break;
                case 3: janitorXP += learningAmount; xp = janitorXP; xpNeeded = (janitorialSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) - 10; break;
                case 4: engineerXP += learningAmount; xp = engineerXP; xpNeeded = (engineerSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 20; break;
                case 5: managerXP += learningAmount; xp = managerXP; xpNeeded = (managementSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 10; break;
            }
        }

        xpBarCon.Activate(xp * 1f / xpNeeded);
        xpBar.SetActive(true);
        xpBar.GetComponent<FadeController>().Activate();

        LevelUpCheck();
    }
    private void LevelUpCheck()
    {
        if (inventoryXP >= (inventorySkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) - 20 && inventorySkill < MAX_LEVEL) { LevelUp(); }
        if (customerXP >= (customerServiceSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 0 && customerServiceSkill < MAX_LEVEL) { LevelUp(); }
        if (janitorXP >= (janitorialSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) - 10 && janitorialSkill < MAX_LEVEL) { LevelUp(); }
        if (engineerXP >= (engineerSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 20 && engineerSkill < MAX_LEVEL) { LevelUp(); }
        if (managerXP >= (managementSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 10 && managementSkill < MAX_LEVEL) { LevelUp(); }

        ShowTaskImage(animator.GetInteger("TaskEnum"), false);
    }

    private void LevelUp()
    {
        transform.GetChild(0).GetChild(6).GetComponent<AudioSource>().Play();
        transform.GetChild(0).GetChild(4).GetComponent<ParticleSystem>().Play();

        if (inventoryXP >= (inventorySkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) - 20) { inventoryXP -= inventorySkill * levelTimesMultiplier + (levelTimesMultiplier / 2); inventorySkill++; }
        if (customerXP >= (customerServiceSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 0) { customerXP -= customerServiceSkill * levelTimesMultiplier + (levelTimesMultiplier / 2); customerServiceSkill++; }
        if (janitorXP >= (janitorialSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) - 10) { janitorXP -= janitorialSkill * levelTimesMultiplier + (levelTimesMultiplier / 2); janitorialSkill++; }
        if (engineerXP >= (engineerSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 20) { engineerXP -= engineerSkill * levelTimesMultiplier + (levelTimesMultiplier / 2); engineerSkill++; }
        if (managerXP >= (managementSkill * levelTimesMultiplier + (levelTimesMultiplier / 2)) + 10) { managerXP -= managementSkill * levelTimesMultiplier + (levelTimesMultiplier / 2); managementSkill++; }
        xpBarCon.Reset();

        capacity = (int)((inventorySkill / 2f) + 0.5f);
        if (capacity <= 0) { capacity = 1; }
    }

    private void CheckTimeSchedule(object sender, System.EventArgs e)
    {
        if (!animator.GetBool("Fired"))
        {
            int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
            //attempt to come into work
            if (!animator.GetBool("OnShift") && (!calledOut || !sentHome))
            {
                //come into work day
                if (workDays[UIController.Instance.weekday] == true)
                {
                    //time
                    if (time == workStart - 100 || (time == 2300 && workStart == 0))
                    {
                        if (traits["weather_lover"] && Controller.Instance.dayType == Controller.DayType.weather) { ComeIntoWork(); }
                        else if (traits["workaholic"] && Controller.Instance.dayType == Controller.DayType.holiday) { ComeIntoWork(); }
                        else if (Random.Range(0, 100.01f) > stats["callOutChance"] * Controller.Instance.globalCalloutChanceMultiplier || employeedForDays < 2)
                        {
                            if (Controller.Instance.dayType == Controller.DayType.weather && employeedForDays > 1)
                            {
                                if (traits["weather_fearful"]) {
                                    UIController.Instance.CreateLog(1, "I'm calling out!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0); 
                                    calledOut = true; }
                                else if (!late) { UIController.Instance.CreateLog(1, "I'm going to be in late today.", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 3);
                                    late = true; lateAmount++; }
                            }
                            else if (Controller.Instance.dayType == Controller.DayType.holiday && employeedForDays > 1)
                            {
                                if (traits["vacationer"]) { UIController.Instance.CreateLog(1, "I'm calling out!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                                    calledOut = true; }
                                else { ComeIntoWork(); }
                            }
                            else { ComeIntoWork(); }
                        }
                        else { UIController.Instance.CreateLog(1, "I'm calling out!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                            calledOut = true; }

                    }
                    else if (time == workStart && late) 
                    {
                        ComeIntoWork();
                    }
                }
            }

            if (time >= workStart && time < workEnd)
            {
                if (!animator.GetBool("OnShift"))
                {
                    if (stress > 0)
                    {
                        AddStress(-1);
                        if (workDays[UIController.Instance.weekday] == false || sentHome) { AddStress(-1); }
                    }
                }
                else
                {
                    wageDue += (int)Math.Round(hourlyWage / 4f);
                    if (hoursWorkedThisWeek > 40) { wageDue += (int)Math.Round(hourlyWage / 8f); AddStress(1); overtimeWorked += 0.25f; }
                    if (!messageCalled)
                    {
                        if ((overtimeWorked == 1 || (overtimeWorked % 5 == 0 && overtimeWorked != 0)) && status != Status.owner) { messageCalled = true; TalkBubble("I'm working overtime!", 1, 2); }
                        if ((hoursWorkedToday >= 18 && hoursWorkedToday % 2 == 0) && status != Status.owner) { messageCalled = true; TalkBubble("I want to go home!", 1, 2); }
                    }
                    hoursWorkedThisWeek += 0.25f;
                    hoursWorkedToday += 0.25f;


                    float timeStressMultiplier = 1;
                    if (UIController.Instance.hour <= 8 && traits["early_bird"]) { timeStressMultiplier = 0.5f; }
                    if (UIController.Instance.hour >= 16 && traits["night_owl"]) { timeStressMultiplier = 0.5f; } 

                    AddStress(timeStressMultiplier);
                    if ((Controller.Instance.insideTemp > 76 && !traits["Heat_lover"]) || Controller.Instance.insideTemp < 70 && !traits["Cold_Lover"]) { AddStress(1); }

                    //if (stress >= 100 && status != Status.owner) { MentalBreak(); }
                    if (stress >= 95 && status != Status.owner) { TalkBubble("I'm fed up with this place!", 1, 3); }
                    if (workDays[UIController.Instance.weekday] == false) { animator.SetBool("OnShift", false); }
                    if ((int)task != hiredTask && hiredTask != 0) { AddStress(timeStressMultiplier); }
                }

                if (!animator.GetBool("OnShift") && !calledOut && !sentHome && !late && !animator.GetBool("Fired") && workDays[UIController.Instance.weekday] == true) { ComeIntoWork(); }
            }
            else if (time < workStart - 100 || (workStart == 0 && time < 2300))
            {
                if (animator.GetBool("OnShift")) { animator.SetBool("OnShift", false); objective = Objective.absent; }
                AddStress(-2);
                hoursWorkedToday = 0;
                //if (workDays[UIController.Instance.weekday] == false) { longTermStress -= stats["stressRelease"] * 0.2f * TransitionController.Instance.totalDifficulty; }
            }
            else if ((time > workEnd && workStart != 0) || (time > workEnd && workStart == 0 && time < 2300))
            {
                if (animator.GetBool("OnShift")) { animator.SetBool("OnShift", false); objective = Objective.absent; }
                AddStress(-2);
                hoursWorkedToday = 0;
            }
        }
        if (!animator.GetBool("Fired") && !calledOut && !mentalBreak) { if (stress >= 100 && status != Status.owner) { MentalBreak(); } }
        if (stuckCalls > 24) { stuckCalls = 0; StuckFailSafe(); return; }//6 hour in game hours of not moving
        if (atWork) { stuckCalls++; }
    }
    public bool atWork;
    public int stuckCalls;
    public void AddStress(float value)//
    {
        //stress is being reduced 10x faster than it is being added
        float statusHelp = ((int)status / 10f);
        if (status == Status.owner) { statusHelp = 0; }
        if (value > 0)
        {
            //this calculation works perfectly if it was on hard
            //stress += ((((float)Math.Clamp((4.17f + Controller.Instance.worldEmployeeStressIncrease + ((Controller.Instance.musicStressBonus * stats["sensitive"]) / 3)) * ((stats["stressfulness"] * GetWageStress()) - statusHelp), MIN_STRESS, MAX_STRESS) / TransitionController.Instance.totalDifficulty) * value) / 4);
            stress += ((float)Math.Clamp((4f + Controller.Instance.worldEmployeeStressIncrease + (Controller.Instance.musicStressBonus * stats["sensitive"] / 3)) * ((stats["stressfulness"] * GetWageStress()) - statusHelp) * value / TransitionController.Instance.totalDifficulty / 4, MIN_STRESS, MAX_STRESS) * Controller.Instance.globalEmployeeStressMultiplier);
            //print("Stress added: " + (float)Math.Clamp((4.17f + Controller.Instance.worldEmployeeStressIncrease + (Controller.Instance.musicStressBonus * stats["sensitive"] / 3)) * ((stats["stressfulness"] * GetWageStress()) - statusHelp) * value / TransitionController.Instance.totalDifficulty / 4, MIN_STRESS, MAX_STRESS));
            //longTermStress += ((((float)Math.Clamp((4.17f + Controller.Instance.worldEmployeeStressIncrease + ((Controller.Instance.musicStressBonus * stats["sensitive"]) / 3)) * ((stats["stressfulness"] * GetWageStress()) - statusHelp), MIN_STRESS, MAX_STRESS) / TransitionController.Instance.totalDifficulty) * value) / 3.475f) / 4;
            longTermStress += ((float)Math.Clamp((4f + Controller.Instance.worldEmployeeStressIncrease + (Controller.Instance.musicStressBonus * stats["sensitive"] / 3)) * ((stats["stressfulness"] * GetWageStress()) - statusHelp) * value / TransitionController.Instance.totalDifficulty / 4, MIN_STRESS, MAX_STRESS) / 3.475f) * Controller.Instance.globalEmployeeStressMultiplier;
            //print("Long term Stress added: " + (float)Math.Clamp((4.17f + Controller.Instance.worldEmployeeStressIncrease + (Controller.Instance.musicStressBonus * stats["sensitive"] / 3)) * ((stats["stressfulness"] * GetWageStress()) - statusHelp) * value / TransitionController.Instance.totalDifficulty / 4, MIN_STRESS, MAX_STRESS) / 3.475f);
            if (stress > 100) { stress = 100; }
            if (longTermStress > 100) { longTermStress = 100; }
        }
        else
        {
            //way too fast
            //stress -= (((TransitionController.Instance.totalDifficulty / 2) + 0.5f + (-value / 4)) * stats["stressRelease"]);/// 1.5f;
            stress -= (float)Math.Clamp(4.25f * -value / 2 * stats["stressRelease"] / 4, MIN_STRESS, MAX_STRESS);
            //print("Stress reduced:" + (float)Math.Clamp(4.17f * -value / 2 * stats["stressRelease"] / 4, MIN_STRESS, MAX_STRESS));
            //longTermStress -= ((((TransitionController.Instance.totalDifficulty / 2) + 0.5f + (-value / 4)) * stats["stressRelease"]));/// 6.66f) / 1.5f;
            longTermStress -= (float)Math.Clamp(4.25f * -value / 2 * stats["stressRelease"] / 4, MIN_STRESS, MAX_STRESS) / 3.475f / 3;
            //print("Long term stress reduced: " + (float)Math.Clamp(4.17f * -value / 2 * stats["stressRelease"] / 4, MIN_STRESS, MAX_STRESS) / 3.475f / 3);


            if (longTermStress < 0) { longTermStress = 0; }
        }

        if (stress > averageStress) { averageStress += 0.01f; }
        else { averageStress -= 0.01f; }
        if (stress < longTermStress) { stress = longTermStress; }
        //emotion
        personVis.UpdateEmotion((int)(stress / 20));
    }
    public void ManagerRange()
    {
        if (animator.GetInteger("TaskEnum") == 5)
        {
            GetMyGridPosXY(out int x, out int y);
            currentHover = new Vector2Int(x, y);
            if (previousHover != currentHover)
            {
                //clear old location
                MapController.Instance.ManagerArea(new Vector3(previousHover.x * 10 + 5, previousHover.y * 10 + 5, 0), this, false);

                //activate on hover
                MapController.Instance.ManagerArea(GetPosition(), this, true);

                previousHover = currentHover;
            }
        }
    }
    public void ResetManagerRange() {  MapController.Instance.ManagerArea(new Vector3(previousHover.x * 10 + 5, previousHover.y * 10 + 5, 0), this, false); previousHover = new Vector2Int(-1, -1); }//will this have complications to manager level is greater than 0?
    private void ComeIntoWork()
    {
        if (stress <= 50) { animator.SetBool("OnShift", true); late = false; if ((int)task != hiredTask && hiredTask != 0) { TalkBubble("This isn't the job I was hired to do!", 1, 2); } }
        else if (stress < 75 && !late) { UIController.Instance.CreateLog(3, "I'm going to be in late today.", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0); late = true; lateAmount++; }
        else { UIController.Instance.CreateLog(1, "I'm calling out!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0); calledOut = true; calledOutAmount++; }
    }

    private void CheckDaySchedule(object sender, UIController.OnDayEventArgs e)
    {
        //print("Stress after day is: " + stress);
        //print("Long Term Stress after day is: " + longTermStress);
        employeedForDays++;
        calledOut = false;
        sentHome = false;
        if (e.dayString == "Sunday") { hoursWorkedThisWeek = 0; }

        if (Controller.Instance.employeePaychecks == "Daily") { PayDay(); }
        if (Controller.Instance.employeePaychecks == "Weekly" && UIController.Instance.weekday == "Monday") { PayDay(); }
        if (Controller.Instance.employeePaychecks == "Biweekly" && (UIController.Instance.day == 1 || UIController.Instance.day == 15)) { PayDay(); }
        if (Controller.Instance.employeePaychecks == "Monthly" && UIController.Instance.day == 1) { PayDay(); }
        //RemoveAllClaims();
        //if day is sunday
        GetNewWageRequested();

        if (Controller.Instance.revertToHiredTasks) 
        {
            string hiredTaskString = null;
            switch(hiredTask)
            {
                //owner
                case 0:
                    switch (TransitionController.Instance.tutorialLevel)
                    {
                        default:
                            switch (TransitionController.Instance.difficulty)
                            {
                                default: hiredTaskString = Localizer.Instance.GetLocalizedText("engineer"); break;
                                case 3: hiredTaskString = Localizer.Instance.GetLocalizedText("stocker"); break;
                            }
                            break;
                        case 1:
                            hiredTaskString = Localizer.Instance.GetLocalizedText("stocker");
                            break;
                        case 2:
                            hiredTaskString = Localizer.Instance.GetLocalizedText("stocker");
                            break;
                        case 3:
                            hiredTaskString = Localizer.Instance.GetLocalizedText("stocker");
                            break;
                        case 4:
                            hiredTaskString = Localizer.Instance.GetLocalizedText("engineer");
                            break;
                        case 5:
                            switch (TransitionController.Instance.difficulty)
                            {
                                default: hiredTaskString = Localizer.Instance.GetLocalizedText("engineer"); break;
                                case 3: hiredTaskString = Localizer.Instance.GetLocalizedText("manager"); break;
                            }
                            break;
                    }

                    break;
                case 1: hiredTaskString = Localizer.Instance.GetLocalizedText("stocker"); break;
                case 2: hiredTaskString = Localizer.Instance.GetLocalizedText("cashier"); break;
                case 3: hiredTaskString = Localizer.Instance.GetLocalizedText("janitor"); break;
                case 4: hiredTaskString = Localizer.Instance.GetLocalizedText("engineer"); break;
                case 5: hiredTaskString = Localizer.Instance.GetLocalizedText("manager"); break;
            }
            SwitchTask(null, hiredTaskString, null); 
        }
    }

    public void RemoveAllClaims()//temp?
    {
        foreach (Building shelf in Controller.Instance.shelves) { if (shelf.employeeQueue.Contains(this)) { shelf.employeeQueue.Remove(this); shelf.allQueue.Remove(this.gameObject); } }
        foreach (Building shelf in Controller.Instance.stockPiles) { if (shelf.employeeQueue.Contains(this)) { shelf.employeeQueue.Remove(this); shelf.allQueue.Remove(this.gameObject); } }
        //foreach (Building shelf in Controller.Instance.shelves) { if (shelf.employeeQueue.Contains(this)) { shelf.employeeQueue.Remove(this); shelf.allQueue.Remove(this.gameObject); } }
    }

    private void PayDay()
    {
        Controller.Instance.MoneyValueChange(-wageDue, transform.position, true, false);
        Controller.Instance.dailyMoneyLostWages += wageDue;
        if (Controller.Instance.money < 0 && !UIController.Instance.gameOver && wageDue > 0) { longTermStress += wageDue; UIController.Instance.CreateLog(1, "I did not get my full paycheck!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0); }
        wageDue = 0;
        AddStress(-2);
    }
    private void GetNewWageRequested()
    {
        if (status != Status.owner)
        {
            /*
            float desiredWageBefore = desiredWage;
            float skills = 0;
            if (inventorySkill > 0) { skills += Controller.Instance.requestedWages[0] * ((TransitionController.Instance.wageLevelIncrease * inventorySkill) + 1); }
            if (customerServiceSkill > 0) { skills += Controller.Instance.requestedWages[1] * ((TransitionController.Instance.wageLevelIncrease * customerServiceSkill) + 1); }
            if (janitorialSkill > 0) { skills += Controller.Instance.requestedWages[2] * ((TransitionController.Instance.wageLevelIncrease * janitorialSkill) + 1); }
            if (engineerSkill > 0) { skills += Controller.Instance.requestedWages[3] * ((TransitionController.Instance.wageLevelIncrease * engineerSkill) + 1); }
            if (managementSkill > 0) { skills += Controller.Instance.requestedWages[4] * ((TransitionController.Instance.wageLevelIncrease * managementSkill) + 1); }

            desiredWage = ( skills +
                (employeedForDays * 0.01f) +
                (int)status) *
                desiredWageMultiplier;
            */
            float skills = 0;
            if (inventorySkill > 0) { skills += Controller.Instance.requestedWages[0] * ((TransitionController.Instance.wageLevelIncrease * inventorySkill) + 1); }
            if (customerServiceSkill > 0) { skills += Controller.Instance.requestedWages[1] * ((TransitionController.Instance.wageLevelIncrease * customerServiceSkill) + 1); }
            if (janitorialSkill > 0) { skills += Controller.Instance.requestedWages[2] * ((TransitionController.Instance.wageLevelIncrease * janitorialSkill) + 1); }
            if (engineerSkill > 0) { skills += Controller.Instance.requestedWages[3] * ((TransitionController.Instance.wageLevelIncrease * engineerSkill) + 1); }
            if (managementSkill > 0) { skills += Controller.Instance.requestedWages[4] * ((TransitionController.Instance.wageLevelIncrease * managementSkill) + 1); }
            if (skills < 7.25f) { skills = 7.25f; }

            desiredWage = skills + (employeedForDays * 0.01f) + (int)status;
            if (traits["Expensive"]) { desiredWage *= 1.05f; }
            if (traits["Cheap"]) { desiredWage *= 0.95f; }
        }
    }

    public void AddItem(Item item)
    {
        if (item != null)
        {
            item.stock = container.GetComponent<StockZone>();
            item.RandomLocation();
            item.transform.GetChild(1).GetComponent<AudioSource>().Play();
            item.refrigerated = true;
        }
    }

    public void FireMe()
    {
        animator.SetBool("Fired", true);
        OnFired?.Invoke(this, EventArgs.Empty);
        PayDay();
    }
    public void SendHome()
    {
        Deselected();
        sentHome = true;
        objective = Objective.absent;
        animator.SetBool("OnShift", false);
    }

    public void SwitchTask(Building selectedBuilding, string taskSelected, Employee2 selectedEmployee)
    {
        //if (working || waiting) { working = false; waiting = false; if (targetRegister != null) { targetRegister.customer?.Leave(); } FindJob(); progressBar.GetComponent<FadeController>().Hide(); }
        if (target != null) { target.TryGetComponent<Building>(out Building building); }
        if (targetBuilding != null) { }//if (building.employee = this) building.employee = null; }

        if (taskSelected != "")
        {
            if (taskSelected == Localizer.Instance.GetLocalizedText("stocker")) { task = Task.stocker; }
            if (taskSelected == Localizer.Instance.GetLocalizedText("Stocker")) {  task = Task.stocker; }
            if (taskSelected == "stocker") { task = Task.stocker; }
            if (taskSelected == Localizer.Instance.GetLocalizedText("cashier")) { task = Task.cashier; }
            if (taskSelected == Localizer.Instance.GetLocalizedText("janitor")) { task = Task.janitor; }
            if (taskSelected == Localizer.Instance.GetLocalizedText("engineer")) { task = Task.engineer; }
            if (taskSelected == Localizer.Instance.GetLocalizedText("manager")) { task = Task.manager; }
            /*
            switch (taskSelected)
            {
                case "stocker": task = Task.stocker; break;
                case "cashier": task = Task.cashier; break;
                case "janitor": task = Task.janitor; break;
                case "engineer": task = Task.engineer; break;
                case "manager": task = Task.manager; break;
            }
            */
        }

        if (selectedBuilding != null)
        {
            if (selectedBuilding.built)
            {
                if (task == Task.engineer)
                {
                    newTargetBuilding = selectedBuilding;
                }
                else
                {
                    if (selectedBuilding.type == BuildingSO.Type.register)
                    {
                        if (customerServiceSkill >= selectedBuilding.customerServiceSkillRequired)
                        {
                            task = Task.cashier;
                            if (Controller.Instance.registers.Contains(selectedBuilding))
                            {
                                newTargetBuilding = selectedBuilding;
                            }
                        }
                        else { TalkBubble("I cannot do that job", 1, 3); }
                    }

                    if (selectedBuilding.type == BuildingSO.Type.shelf || selectedBuilding.type == BuildingSO.Type.stockPile)
                    {
                        if (inventorySkill >= selectedBuilding.inventorySkillRequired)
                        {
                            task = Task.stocker;

                            if (Controller.Instance.stockPiles.Contains(selectedBuilding))
                            {
                                newTargetBuilding = selectedBuilding;
                            }
                            if (Controller.Instance.shelves.Contains(selectedBuilding))
                            {
                                newTargetBuilding = selectedBuilding;
                                //newTargetItemID = selectedBuilding.selectedItemTypeID;
                            }
                            //Returning();
                        }
                        else { TalkBubble("I cannot do that job", 1, 3); }
                    }
                }

            }
            else
            {
                if (task == Task.engineer && targetBuilding != null) { targetBuilding.engineerClaim = null; }
                task = Task.engineer;
                newTargetBuilding = selectedBuilding;
            }
        }

        if (selectedEmployee != null)
        {
            newTargetEmployee = selectedEmployee;
        }

        //if (animator.GetInteger("TaskEnum") == 5) { ResetManagerRange(); }
        newTask = (int)task;
        animator.SetBool("SwichTask", true);
        animator.SetInteger("TaskEnum", (int)task);
        OnTaskValueChanged?.Invoke(this, EventArgs.Empty);

        if (status != Status.owner)
        {
            switch (task)
            {
                case Task.stocker: myName = birthName + System.Environment.NewLine + "Stocker"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.green; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "S"; break;
                case Task.cashier: myName = birthName + System.Environment.NewLine + "Cashier"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.blue; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "C"; break;
                case Task.janitor: myName = birthName + System.Environment.NewLine + "Janitor"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.cyan; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "J"; break;
                case Task.engineer: myName = birthName + System.Environment.NewLine + "Engineer"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.yellow; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "E"; break;
                case Task.manager: myName = birthName + System.Environment.NewLine + "Manager"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.magenta; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "M"; break;
            }
        }
        else { myName = birthName + System.Environment.NewLine + "Owner"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.red; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "O"; }
        personVis.ChangeJobs((int)task);

        if ((int)task != hiredTask && hiredTask != 0) { TalkBubble("This isn't the job I was hired to do!", 1, 2); }
        gameObject.name = birthName + " " + task.ToString();
    }

    public void GoHere(Vector3 targetPos)
    {
        if (task == Task.janitor)
        {
            //new clean target
            MapController.Instance.grid.GetXY(targetPos, out int x, out int y);
            newTargetTile = new Vector2Int(x, y);
            newTask = (int)task;
            animator.SetBool("SwichTask", true);
        }
        if (task == Task.manager)
        {
            //move to
            MapController.Instance.grid.GetXY(targetPos, out int x, out int y);
            targetTile = new Vector2Int(x, y);
            targetPosition = new Vector3(targetTile.x * 10 + 5, targetTile.y * 10 + 5, 0);
            moveTo = true;
        }
    }

    public void Absent()
    {
        SwitchObjective(0);
        visuals.gameObject.SetActive(false);
        container.gameObject.SetActive(false);
        myCollider.enabled = false;
        myCollider2.enabled = false;
        RemoveAllClaims();

        if (animator.GetBool("Fired"))
        {
            UnSubScribe();
            Destroy(this.gameObject, 0.1f);//revert to customer
        }
        else if (workDays[UIController.Instance.weekday] == true)//for loaded
        {
            int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
            if (time >= workStart && time < workEnd && (!calledOut && !sentHome))
            {
                myCollider.enabled = true;
                myCollider2.enabled = true;
                visuals.gameObject.SetActive(true);
                container.gameObject.SetActive(true);
                animator.SetBool("OnShift", true);
            }
        }

        ResetManagerRange();
    }

    public void SwitchObjective(int value)
    {
        objective = (Objective)value;
        OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Tick20Delay(object sender, TickSystem.OnTickEventArgs e)
    {
        if (mentalBreak) { transform.GetChild(0).GetChild(8).GetComponent<Animation>()["Mad"].speed = TickSystem.Instance.timeMultiplier; }
    }
    private void Tick25Delay(object sender, TickSystem.OnTickEventArgs e)
    {
        messageCalled = false;
    }
    private bool walkingSoundEnabled;
    private bool walkingAuidoAdjusting;
    public void ToggleWalkingSounds(bool enable)
    {
        if (!enable) {  walkingSoundEnabled = false; }
        else { transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Play(); walkingSoundEnabled = true; }
        walkingAuidoAdjusting = true;
    }

    private void Update()
    {
        if (selected)
        {
            NewGrid newGrid = MapController.Instance.grid.GetGridObject(GetPosition());
            if (newGrid != null)
            {
                //MapController.Instance.grid.GetXY(GetPosition(), out int x, out int y);
                GetMyGridPosXY(out int myX, out int myY);
                GetTargetGridPosXY(out int x, out int y);

                List<NewGrid> path = MapController.Instance.FindPath(myX, myY, x, y, false, false);
                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.green);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) { Deselected(); OnMouseExit(); }

        if (walkingAuidoAdjusting)
        {
            if (Controller.Instance.fxVolume == 0) { walkingAuidoAdjusting = false; return; }

            if (walkingSoundEnabled)
            {
                if (transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().volume < 0.299f * Controller.Instance.fxVolume)
                {
                    transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().volume = Mathf.Lerp(transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().volume, 0.3f * Controller.Instance.fxVolume, Time.deltaTime * 10);
                }
                else { walkingAuidoAdjusting = false; }
            }
            else
            {
                if (transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().volume > 0.001f)
                {
                    transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().volume = Mathf.Lerp(transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().volume, 0f, Time.deltaTime * 10);
                }
                else { transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Stop(); walkingAuidoAdjusting = false; }
            }
        }


    }
    private void On10Tick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (animator.GetBool("OnShift")) { GenerateDirt(); }
    }
    private void OnTimeSpeedChange(object sender, TickSystem.OnTickEventArgs e)
    {
        speedCalc = GetSpeed(GetGrid());
        animator.speed = TickSystem.Instance.timeMultiplier;
    }
    private void GenerateDirt()
    {
        if (GetGrid() != null)
        {
            if (task != Task.janitor && TransitionController.Instance.difficulty == 1 && TransitionController.Instance.tutorialLevel > 2) 
            {
                int playableGridStart = TransitionController.Instance.playablegridstart;
                int playableGridSize = TransitionController.Instance.playablegridsize;
                GetMyGridPosXY(out int x, out int y);

                if ((x >= playableGridStart && x <= playableGridSize + playableGridStart) && y >= playableGridStart && y <= playableGridSize + playableGridStart) { GetGrid().IncreaseCleanValue(0.25f); }
            }
        }
        else { gameObject.transform.position = Controller.Instance.entrances[0].entranceNode.transform.position; }

    }
    public Vector3 StandNextToMe(Employee2 manager, Officer officer)
    {
        Vector3 thisTargetPos = new Vector3(0,0,0);
        if (manager != null) { thisTargetPos = manager.targetPosition; }
        if (officer != null) { thisTargetPos = officer.targetPosition; }

        Vector3 myPos = this.transform.position;

        List<Vector3> points = new List<Vector3>();

        Vector3 point1 = new Vector3(myPos.x + 10, myPos.y, myPos.z);
        Vector3 point2 = new Vector3(myPos.x - 10, myPos.y, myPos.z);
        Vector3 point3 = new Vector3(myPos.x, myPos.y + 10, myPos.z);
        Vector3 point4 = new Vector3(myPos.x, myPos.y - 10, myPos.z);
        points.Add(point1);
        points.Add(point2);
        points.Add(point3);
        points.Add(point4);

        float targDist = 9999999;
        int choosenPoint = 5;
        for (int i = 0; i < points.Count; i++)
        {
            float dist = Vector3.Distance(thisTargetPos, points[i]);

            bool sameAsAnother = false;

            //if point is not taken
            if (MapController.Instance.grid.GetGridObject(points[i]) != null)
            {
                NewGrid newGrid = MapController.Instance.grid.GetGridObject(points[i]);
                if (!newGrid.CanWalk()) { sameAsAnother = true; }
            }
            else { sameAsAnother = true; }

            if (dist < targDist && !sameAsAnother) { targDist = dist; choosenPoint = i; }
        }

        if (choosenPoint != 5) 
        {
            NewGrid newGrid = MapController.Instance.grid.GetGridObject(points[choosenPoint]);
            return new Vector3(newGrid.x * 10 + 5, newGrid.y * 10 + 5, 0);
            //return points[choosenPoint]; 
        }
        else {
            if (manager != null) { return manager.transform.position; }
            else if (officer != null) { return officer.transform.position; }
            else { return Vector3.zero; }
        }
    }

    public void ShowTaskImage(int type, bool activate)
    {
        if (activate)
        {
            taskImage.transform.parent.gameObject.SetActive(true);
            taskImage.transform.parent.gameObject.GetComponent<FadeController>().Activate();
            taskImage.sprite = taskSprites[type - 1];
            taskImage.color = taskColors[type - 1];
        }
        else
        {
            //taskImage.transform.parent.gameObject.SetActive(false);
            taskImage.transform.parent.gameObject.GetComponent<FadeController>().Hide();
        }
    }
    public void TalkBubble(string message, int type, int color)
    {
        bubble.NewMessage(message, type, color);
    }
    public void CustomerInteraction(Customer2 customer, float customerNumber)
    {
        float number = Random.Range(0, 50f);
        number += customerServiceSkill * 2.5f;
        number += customerNumber;
        number += stats["socialSkills"];

        int randomTalk = Random.Range(0, 3);

        if (number <= 50) 
        {
            switch(randomTalk)
            {
                default: TalkBubble("**** off", 0, 3); break;
                case 1: TalkBubble("Screw you", 0, 3); break;
                case 2: TalkBubble("Go! Get out!", 0, 3); break;
            }

            AddStress(1.5f); 
        }
        else if (number > 75 && number < 100) 
        {
            switch (randomTalk)
            {
                default: TalkBubble("Have a nice day", 0, 0); break;
                case 1: TalkBubble("Come again!", 0, 0); break;
                case 2: TalkBubble("See you later", 0, 0); break;
            }
            
        }
        else if (number >= 100) 
        {
            switch (randomTalk)
            {
                default: TalkBubble("You have a wonder day!!!", 0, 1); break;
                case 1: TalkBubble("Thank you! Come again!", 0, 1); break;
                case 2: TalkBubble("Have a good day!", 0, 1); break;
            }
            
            AddStress(-0.5f); 
        }

        customer.ContinueInteraction(number);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private bool mentalBreak;
    private void MentalBreak()
    {
        if (!mentalBreak)
        {
            GameObject.Find("MainAudio").transform.GetChild(10).GetComponent<AudioSource>().Play();
            if (traits["Lunatic"])
            {
                mentalBreak = true;
                Controller.Instance.MentalBreakOccuring();
                transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
                animator.SetInteger("MentalBreak", 1);
                TalkBubble("I AM GOING TO KILL YOU ALL!!!", 0, 3);
                UIController.Instance.CreateLog(1, "I AM GOING TO KILL YOU ALL!!!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                FireMe();
                foreach (Customer2 customer in Controller.Instance.customers) { customer.storePreferance[0] -= 20; }
                GameObject.Find("MainAudio").transform.GetChild(11).GetComponent<AudioSource>().Play();
            }
            else if (traits["Pyromaniac"])
            {
                mentalBreak = true;
                Controller.Instance.MentalBreakOccuring();
                transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
                animator.SetInteger("MentalBreak", 2);
                TalkBubble("FIRE, FIRE, FIRE!!!", 0, 3);
                UIController.Instance.CreateLog(1, "FIRE, FIRE, FIRE!!!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                FireMe();
                foreach(Customer2 customer in Controller.Instance.customers) { customer.storePreferance[0] -= 10; }
                foreach(Employee2 employee in Controller.Instance.employees) { if (employee.traits["Pyromaniac"] && employee.animator.GetBool("OnShift")) { MentalBreak(); } }
                GameObject.Find("MainAudio").transform.GetChild(11).GetComponent<AudioSource>().Play();
            }
            else if (traits["Angry"])
            {
                mentalBreak = true;
                Controller.Instance.MentalBreakOccuring();
                transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
                animator.SetInteger("MentalBreak", 3);
                TalkBubble("**** THIS, **** YOU, **** ME!", 0, 3);
                UIController.Instance.CreateLog(1, "**** THIS, **** YOU, **** ME!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                FireMe();
                foreach (Customer2 customer in Controller.Instance.customers) { customer.storePreferance[0] -= 10; }
                GameObject.Find("MainAudio").transform.GetChild(11).GetComponent<AudioSource>().Play();
            }
            else if (Random.Range(0, 100) <= stats["loyalty"] * Controller.Instance.globalLoyaltyMultiplier)
            {
                TalkBubble("Frick this, i'm going home!", 1, 3);
                UIController.Instance.CreateLog(1, "Frick this, i'm going home!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                SendHome();
                calledOut = true;
            }
            else
            {
                mentalBreak = true;
                transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
                TalkBubble("Screw this, I QUIT!!!", 1, 3);
                UIController.Instance.CreateLog(1, "Screw you, I QUIT!!!", birthName + " " + Localizer.Instance.GetLocalizedText(task.ToString()), 0);
                FireMe();
            }
        }
    }
    public void Insulted()
    {
        TalkBubble("**** off", 0, 3); AddStress(10);
    }

    public void EnterNewTile(MapController.NewGrid newGrid)
    {
        List<Building> NewBuildings = MapController.Instance.GetNearbyBuildings(newGrid);
        if (NewBuildings.Count > 0)
        {
            for (int i = 0; i < NewBuildings.Count; i++)
            {
                if (!Memory.Contains(NewBuildings[i])) { Memory.Add(NewBuildings[i]); }
            }
        }
        speedCalc = GetSpeed(newGrid);
        stuckCalls = 0;
    }

    /// //Build   ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private int startingTask;
    public static Employee2 Create(EmployeeSO employeeSO, int invSkill, int custSkill, int janSkill, int engSkill, int manSkill, float hiredWage, bool isManager, float age, string birthName, int startingTask, bool isFemale, Dictionary<string, bool> allTraits, bool loaded)
    {
        Transform unitTransform = Instantiate(employeeSO.prefab, Vector3.zero, Quaternion.Euler(0, 0, 0));

        Employee2 employee = unitTransform.GetComponent<Employee2>();
        if (isManager) { employee.status = Employee2.Status.owner; }
        employee.Setup(employeeSO, invSkill, custSkill, janSkill, engSkill, manSkill, hiredWage, age, birthName, startingTask, isFemale, allTraits, loaded);

        return employee;
    }

    [SerializeField] private EmployeeSO employeeSO;

    private void Setup(EmployeeSO employeeSO, int invSkill, int custSkill, int janSkill, int engSkill, int manSkill, float hiredWage, float age, string birthName, int startingTask, bool isFemale, Dictionary<string, bool> allTraits, bool loaded)
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;

        this.employeeSO = employeeSO;
        this.birthName = birthName;
        this.age = age;
        this.birthName = birthName;
        this.startingTask = startingTask;
        this.isFemale = isFemale;

        this.hourlyWage = hiredWage;
        this.inventorySkill = invSkill;
        this.customerServiceSkill = custSkill;
        this.janitorialSkill = janSkill;
        this.engineerSkill = engSkill;
        this.managementSkill = manSkill;

        if (allTraits != null) { this.traits = allTraits; }

        GetTransforms();
        CreateUICharacters();
        Subscribe();
        OnMouseExit();
        if (status == Status.owner) { NewAssignTraits(); }
        GetTrainingReuirements();
        SetUpWorkDays();
        if (!loaded) { SetStats(); }
        Invoke("StartTasking", 0.1f);
    }

    private void GetTrainingReuirements()
    {
        //if (diffictulyModifier == 2) { diffictulyModifier = 1; }

        trainingRequired.Add(0);//to default task #0
        if (inventorySkill > 0) { trainingRequired.Add(((inventorySkill * 10) + 35)); }
        else { trainingRequired.Add(150); }
        if (customerServiceSkill > 0) { trainingRequired.Add(((customerServiceSkill * 10) + 45)); }
        else { trainingRequired.Add(225); }
        if (janitorialSkill > 0) { trainingRequired.Add(((janitorialSkill * 10) + 40)); }
        else { trainingRequired.Add(200); }
        if (engineerSkill > 0) { trainingRequired.Add(((engineerSkill * 10) + 50)); }
        else { trainingRequired.Add(250); }
        trainingRequired.Add(0);
        //if (managementSkill > 0) { trainingRequired.Add((-managementSkill * 5) + 50); }
        //else { trainingRequired.Add(100); }
    }

    private void StartTasking()
    {
        capacity = (int)((inventorySkill / 2f) + 0.5f);
        if (capacity <= 0) { capacity = 1; }

        if (animator.GetInteger("TaskEnum") == 0)
        {
            if (janitorialSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("janitor"), null); }
            if (managementSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("manager"), null); }
            if (customerServiceSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("cashier"), null); }
            if (engineerSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); }
            if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); }

            switch(TransitionController.Instance.tutorialLevel)
            {
                default:
                    switch (TransitionController.Instance.difficulty)
                    {
                        default: if (engineerSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); } break;
                        case 3: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                    }
                    break;
                case 1:
                    switch (TransitionController.Instance.difficulty)
                    {
                        default: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                        case 3: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                    }
                    break;
                case 2:
                    switch (TransitionController.Instance.difficulty)
                    {
                        default: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                        case 3: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                    }
                    break;
                case 3:
                    switch (TransitionController.Instance.difficulty)
                    {
                        default: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                        case 3: if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); } break;
                    }
                    break;
                case 4:
                    switch (TransitionController.Instance.difficulty)
                    {
                        default: if (engineerSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); } break;
                        case 3: if (engineerSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); } break;
                    }
                    break;
                case 5:
                    switch (TransitionController.Instance.difficulty)
                    {
                        default: if (engineerSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); } break;
                        case 3: if (managementSkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("manager"), null); } break;
                    }
                    break;
            }

            /*
            if (janitorialSkill > 0 && TransitionController.Instance.tutorialLevel > 2) { SwitchTask(null, Localizer.Instance.GetLocalizedText("janitor"), null); }
            if (managementSkill > 0 && (TransitionController.Instance.tutorialLevel == 5 || TransitionController.Instance.difficulty == 1)) { SwitchTask(null, Localizer.Instance.GetLocalizedText("manager"), null); }
            if (customerServiceSkill > 0 && TransitionController.Instance.tutorialLevel > 1) { SwitchTask(null, Localizer.Instance.GetLocalizedText("cashier"), null); }
            if (engineerSkill > 0 && (TransitionController.Instance.tutorialLevel == 4 || TransitionController.Instance.difficulty == 1)) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); }
            if (inventorySkill > 0) { SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); }
            if (engineerSkill > 0 && TransitionController.Instance.tutorialLevel > 3) { SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); }
            */
            personVis.UpdateEmotion((int)(stress / 20));
        }

        if (startingTask != 0)
        {
            switch(startingTask)
            {
                case 1: SwitchTask(null, Localizer.Instance.GetLocalizedText("stocker"), null); break;
                case 2: SwitchTask(null, Localizer.Instance.GetLocalizedText("cashier"), null); break;
                case 3: SwitchTask(null, Localizer.Instance.GetLocalizedText("janitor"), null); break;
                case 4: SwitchTask(null, Localizer.Instance.GetLocalizedText("engineer"), null); break;
                case 5: SwitchTask(null, Localizer.Instance.GetLocalizedText("manager"), null); break;
            }
        }

        if (status != Status.owner)
        {
            switch (task)
            {
                case Task.stocker: myName = birthName + System.Environment.NewLine + "Stocker"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.green; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "S"; break;
                case Task.cashier: myName = birthName + System.Environment.NewLine + "Cashier"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.blue; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "C"; break;
                case Task.janitor: myName = birthName + System.Environment.NewLine + "Janitor"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.cyan; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "J"; break;
                case Task.engineer: myName = birthName + System.Environment.NewLine + "Engineer"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.yellow; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "E"; break;
                case Task.manager: myName = birthName + System.Environment.NewLine + "Manager"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.magenta;transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "M"; break;
            }
        }
        else { myName = birthName + System.Environment.NewLine + "Owner"; transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = Color.red; transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = "O"; }

        speedCalc = GetSpeed(GetGrid());
        animator.speed = TickSystem.Instance.timeMultiplier;
        if (TransitionController.Instance.tutorialLevel == 1) { ToolTip.Instance.ActivateTutorial(20); }
    }

    private void GetTransforms()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        myCollider = colliders[0];
        myCollider2 = colliders[1];
        visuals = transform.GetChild(0);
        container = transform.GetChild(0).GetChild(3);

        progressBar = transform.GetChild(1).GetChild(0).gameObject;
        progressBarCon = progressBar.GetComponent<BarController>();
        xpBar = transform.GetChild(1).GetChild(1).gameObject;
        xpBarCon = xpBar.GetComponent<BarController>();
        trainingBar = transform.GetChild(1).GetChild(2).gameObject;
        trainingBarCon = trainingBar.GetComponent<BarController>();
        taskImage = transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<SpriteRenderer>();

        progressBarCon.Reset();
        xpBarCon.Reset();

        personVis = transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>();
        bubble = transform.GetChild(1).GetChild(4).GetComponent<ChatMessage>();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void Subscribe()
    {
        UIController.Instance.OnTimeValueChanged += CheckTimeSchedule;
        UIController.Instance.OnDayValueChanged += CheckDaySchedule;
        TickSystem.Instance.OnAdjustedTick += Tick20Delay;
        TickSystem.Instance.On50Tick += Tick25Delay;
        TickSystem.Instance.On10Tick += On10Tick;
        TickSystem.Instance.OnSpeedChange += OnTimeSpeedChange;
        //OnTaskValueChanged += SwitchedTask;
        //OnObjectiveValueChanged += SwitchedObjective;
        Controller.Instance.employees.Add(this);
    }

    private void UnSubScribe()
    {
        UIController.Instance.OnTimeValueChanged -= CheckTimeSchedule;
        UIController.Instance.OnDayValueChanged -= CheckDaySchedule;
        TickSystem.Instance.OnAdjustedTick -= Tick20Delay;
        TickSystem.Instance.On50Tick -= Tick25Delay;
        TickSystem.Instance.On10Tick -= On10Tick;
        TickSystem.Instance.OnSpeedChange -= OnTimeSpeedChange;
        //OnTaskValueChanged -= SwitchedTask;
        //OnObjectiveValueChanged -= SwitchedObjective;
        Controller.Instance.employees.Remove(this);
    }

    private void ScheduleStartUP()
    {
        int i = 0;
        for (int h = 0; h + i < 2400;)
        {
            mySchedule.Add(h + i, false);
            if (i < 45) { i += 15; }
            else { h += 100; i = 0; }
        }
    }

    private void CreateUICharacters()
    {
        GameObject character = null;

        if (status != Status.owner) { character = Instantiate(UIController.Instance.uiCharacter, UIController.Instance.characterHolder); }
        else { character = Instantiate(UIController.Instance.uiCharacter, UIController.Instance.managerCharacterHolder); }
        myUICharacter = character.GetComponent<UICharacter>();
        myUICharacter.unit = this;
        myUICharacter.StartUp();

        /*
        character = Instantiate(UIController.Instance.uiCharacterSchedule, UIController.Instance.characterScheduleHolder);
        myUICharacterSchedule = character.GetComponent<UICharacterSchedule>();
        myUICharacterSchedule.unit = this;
        myUICharacterSchedule.StartUp();


        character = Instantiate(UIController.Instance.uiUnitTask, UIController.Instance.unitTaskHolder);
        myUITask = character.GetComponent<UITaskCharacter>();
        //myUITask.unit = this;
        //myUITask.StartUp();
        */

        character = Instantiate(UIController.Instance.staff, UIController.Instance.staffHolder);
        uiStaff = character.GetComponent<Staff>();
        uiStaff.unit = this;
        uiStaff.StartUp();

        //List<bool> workDayBool = new List<bool>();
        //foreach (KeyValuePair<string, bool> pair in workDays) { workDayBool.Add(pair.Value); }
        //uiStaff.Load(workDayBool);

        if (status != Status.owner) { UIController.Instance.SpawnEOTM(this); }
    }

    public void GetMyGridPosXY(out int x, out int y)
    {
        Vector3 pos = this.GetPosition();
        MapController.Instance.grid.GetXY(pos, out int newX, out int newY);
        x = newX; y = newY;
    }

    private void GetTargetGridPosXY(out int x, out int y)
    {
        Vector3 pos = targetPosition;
        MapController.Instance.grid.GetXY(pos, out int newX, out int newY);
        x = newX; y = newY;
    }

    public NewGrid claimedTile;
    public void ClaimTile()
    {
        GetGrid().taken = true;
        claimedTile = GetGrid();
    }

    public void RemoveClaim()
    {
        if (claimedTile != null)
        {
            claimedTile.taken = false;
            claimedTile = null;
        }
    }

    public NewGrid GetGrid() { return MapController.Instance.grid.GetGridObject(this.GetPosition()); }

    [System.Serializable]
    public class SaveObject
    {
        //appearnace
        public Vector3 position;

        public string myName;
        public string birthName;
        public float age;
        public bool isFemale;
        public Task task;
        public Status status;

        public float wageDue;
        public float wage;
        public int workStart;
        public int workEnd;
        public string shiftName;

        public float stress;
        public float longTermStress;

        public List<float> trainingRequired = new List<float>();
        public int inventorySkill;
        public int customerServiceSkill;
        public int janitorialSkill;
        public int engineerSkill;
        public int managementSkill;
        public float inventoryXP;
        public float customerServiceXP;
        public float janitorialXP;
        public float engineerXP;
        public float managementXP;

        public List<int> scheduleInt = new List<int>();
        public List<bool> scheduleBool = new List<bool>();
        public List<string> workDayInt = new List<string>();
        public List<bool> workDayBool = new List<bool>();

        public List<string> traitNames = new List<string>();
        public List<bool> traitValues = new List<bool>();
        public List<string> statNames = new List<string>();
        public List<float> statValues = new List<float>();

        public bool late;
        public bool calledOut;
        public bool sentHome;
        public bool onShift;
        public int employeedForDays;
        public float hoursWorkedThisWeek;
        public float hoursWorkedToday;

        public float averageStress;
        public float overtimeWorked;
        public int calledOutAmount;
        public int lateAmount;
        public int hiredTask;
        public bool insideStore;

        public List<float> characterSizes = new List<float>();
        public List<int> appearance = new List<int>();
    }

    public SaveObject Save()
    {
        List<int> scheduleInt = new List<int>();
        List<bool> scheduleBool = new List<bool>();
        foreach (KeyValuePair<int, bool> pair in mySchedule)
        {
            scheduleInt.Add(pair.Key);
            scheduleBool.Add(pair.Value);
        }
        List<string> workDayInt = new List<string>();
        List<bool> workDayBool = new List<bool>();
        foreach (KeyValuePair<string, bool> pair in workDays)
        {
            workDayInt.Add(pair.Key);
            workDayBool.Add(pair.Value);
        }

        List<string> traitNames = new List<string>();
        List<bool> traitValues = new List<bool>();
        foreach (KeyValuePair<string, bool> pair in traits)
        {
            traitNames.Add(pair.Key);
            traitValues.Add(pair.Value);
        }
        List<string> statNames = new List<string>();
        List<float> statValues = new List<float>();
        foreach (KeyValuePair<string, float> pair in stats)
        {
            statNames.Add(pair.Key);
            statValues.Add(pair.Value);
        }
        return new SaveObject
        {
            position = this.transform.position,

            myName = this.myName,
            birthName = this.birthName,
            age = this.age,
            isFemale = this.isFemale,
            task = this.task,
            status = this.status,

            wageDue = this.wageDue,
            wage = this.hourlyWage,
            workStart = this.workStart,
            workEnd = this.workEnd,
            shiftName = this.uiStaff.shiftName,

            stress = this.stress,
            longTermStress = this.longTermStress,

            trainingRequired = this.trainingRequired,
            inventorySkill = this.inventorySkill,
            customerServiceSkill = this.customerServiceSkill,
            janitorialSkill = this.janitorialSkill,
            engineerSkill = this.engineerSkill,
            managementSkill = this.managementSkill,
            inventoryXP = this.inventoryXP,
            customerServiceXP = this.customerXP,
            janitorialXP = this.janitorXP,
            engineerXP = this.engineerXP,
            managementXP = this.managerXP,

            scheduleInt = scheduleInt,
            scheduleBool = scheduleBool,
            workDayInt = workDayInt,
            workDayBool = workDayBool,

            traitNames = traitNames,
            traitValues = traitValues,
            statNames = statNames,
            statValues = statValues,

            late = this.late,
            calledOut = this.calledOut,
            sentHome = this.sentHome,
            onShift = this.animator.GetBool("OnShift"),
            employeedForDays = this.employeedForDays,
            hoursWorkedThisWeek = this.hoursWorkedThisWeek,
            hoursWorkedToday = this.hoursWorkedToday,

            averageStress = this.averageStress,
            overtimeWorked = this.overtimeWorked,
            calledOutAmount = this.calledOutAmount,
            lateAmount = this.lateAmount,
            hiredTask = this.hiredTask,

            characterSizes = this.personVis.sizes,
            appearance = this.personVis.set,
            insideStore = this.insideStore,
        };
    }

    public void Load(SaveObject saveObject, EmployeeSO employeeSO)
    {
        Dictionary<string, bool> employeeTraits = new Dictionary<string, bool>();
        for (int i = 0; i < saveObject.traitNames.Count; i++)
        {
            employeeTraits.Add(saveObject.traitNames[i], saveObject.traitValues[i]);
        }
        bool isManager = false;
        if (saveObject.status == Status.owner) { isManager = true; }
        Employee2 placedEmployee = Employee2.Create(employeeSO, saveObject.inventorySkill, saveObject.customerServiceSkill, saveObject.janitorialSkill, saveObject.engineerSkill, saveObject.managementSkill, saveObject.wage, isManager, saveObject.age, saveObject.birthName, (int)saveObject.task, saveObject.isFemale, employeeTraits, true);

        placedEmployee.transform.position = saveObject.position;
        placedEmployee.status = saveObject.status;
        placedEmployee.myUICharacter.Promote((int)placedEmployee.status); 
        placedEmployee.SetApperance(saveObject.appearance, saveObject.characterSizes);

        placedEmployee.wageDue = saveObject.wageDue;
        placedEmployee.workStart = saveObject.workStart;
        placedEmployee.workEnd = saveObject.workEnd;

        placedEmployee.stress = saveObject.stress;
        placedEmployee.longTermStress = saveObject.longTermStress;

        placedEmployee.trainingRequired = saveObject.trainingRequired;
        placedEmployee.inventoryXP = saveObject.inventoryXP;
        placedEmployee.customerXP = saveObject.customerServiceXP;
        placedEmployee.janitorXP = saveObject.janitorialXP;
        placedEmployee.engineerXP = saveObject.engineerXP;
        placedEmployee.managerXP = saveObject.managementXP;

        placedEmployee.mySchedule.Clear();
        for (int i = 0; i < saveObject.scheduleInt.Count; i++)
        {
            placedEmployee.mySchedule.Add(saveObject.scheduleInt[i], saveObject.scheduleBool[i]);
        }
        placedEmployee.workDays.Clear();
        for (int i = 0; i < saveObject.workDayInt.Count; i++)
        {
            placedEmployee.workDays.Add(saveObject.workDayInt[i], saveObject.workDayBool[i]);
        }


        placedEmployee.stats.Clear();
        for (int i = 0; i < saveObject.statNames.Count; i++)
        {
            placedEmployee.stats.Add(saveObject.statNames[i], saveObject.statValues[i]);
        }

        placedEmployee.late = saveObject.late;
        placedEmployee.calledOut = saveObject.calledOut;
        placedEmployee.sentHome = saveObject.sentHome;
        placedEmployee.employeedForDays = saveObject.employeedForDays;
        placedEmployee.hoursWorkedThisWeek = saveObject.hoursWorkedThisWeek;
        placedEmployee.hoursWorkedToday = saveObject.hoursWorkedToday;

        placedEmployee.averageStress = saveObject.averageStress;
        placedEmployee.overtimeWorked = saveObject.overtimeWorked;
        placedEmployee.calledOutAmount = saveObject.calledOutAmount;
        placedEmployee.lateAmount = saveObject.lateAmount;
        placedEmployee.hiredTask = saveObject.hiredTask;
        placedEmployee.insideStore = saveObject.insideStore;

        placedEmployee.GetComponent<Animator>().SetInteger("TaskEnum", (int)saveObject.task);
        placedEmployee.GetComponent<Animator>().SetBool("OnShift", saveObject.onShift);

        placedEmployee.uiStaff.Load(saveObject.workDayBool, saveObject.shiftName);
        //placedEmployee.myUICharacterSchedule.NewUpdateValues();

        //        placedObject.SetApperance(appearance, characterSizes);
    }

    private void NewAssignTraits()
    {
        if (traits.Count == 0)
        {
            bool boolen = false;
            float number = 0;

            //severe weather
            //if (Random.Range(0, 100) > 90) { boolen = true; } else { boolen = false; }
            //traits.Add("weather_fearful", boolen);
            traits.Add("weather_fearful", false);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("weather_lover", boolen);

            //shopping times
            number = Random.Range(3, 21);
            if (number < 7) { boolen = true; } else { boolen = false; }
            traits.Add("early_bird", boolen);
            if (number > 17) { boolen = true; } else { boolen = false; }
            traits.Add("night_owl", boolen);

            //holidays
            if (Random.Range(0, 100) > 75) { boolen = true; } else { boolen = false; }
            traits.Add("workaholic", boolen);
            //if (Random.Range(0, 100) > 90 && boolen == false) { boolen = true; } else { boolen = false; }
            //traits.Add("vacationer", boolen);
            traits.Add("vacationer", false);

            //stress speed
            number = Random.Range(0.5f, 1.5f);//1 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("stressless", boolen);
            //if (number > 1.25f) { boolen = true; } else { boolen = false; }
            //traits.Add("stressful", boolen);
            traits.Add("stressful", false);

            //How great everything impacts this customer
            number = Random.Range(0.5f, 1.5f);//24 average
            //if (number < 0.75f) { boolen = true; } else { boolen = false; }
            //traits.Add("slow_learner", boolen);
            traits.Add("slow_learner", false);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_learner", boolen);

            //how much stress is reduced during time off
            number = Random.Range(0.5f, 1.5f);//1 average
            //if (number < 0.75f) { boolen = true; } else { boolen = false; }
            //traits.Add("burnt_out", boolen);
            traits.Add("burnt_out", false);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("work_lover", boolen);

            //walking speed
            number = Random.Range(35, 45);//40 average
            //if (number < 37.5f) { boolen = true; } else { boolen = false; }
            //traits.Add("slow_walker", boolen);
            traits.Add("slow_walker", false);
            if (number > 42.5f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_walker", boolen);

            //working speed
            number = Random.Range(33, 43);//38 average//33 min
            if (number < 35.5f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_worker", boolen);
            //if (number > 40.5f) { boolen = true; } else { boolen = false; }
            //traits.Add("slow_worker", boolen);
            traits.Add("slow_worker", false);

            //age
            if (age < 21) { boolen = true; } else { boolen = false; }
            traits.Add("young", boolen);
            if (age > 65) { boolen = true; } else { boolen = false; }
            traits.Add("old", boolen);

            //music effect
            number = Random.Range(0.5f, 1.5f);//1 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("deaf", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("sensitive", boolen);

            //calling out chance //chance of not coming in
            number = Random.Range(0.01f, 10f);//5 average
            if (number < 2.5f) { boolen = true; } else { boolen = false; }
            traits.Add("reliable", boolen);
            //if (number > 7.5f) { boolen = true; } else { boolen = false; }
            //traits.Add("unreliable", boolen);
            traits.Add("unreliable", false);

            //temp preferance
            number = Random.Range(68f, 76f);//72 average
            if (number < 70f) { boolen = true; } else { boolen = false; }
            traits.Add("Cold_Lover", boolen);
            if (number > 74f) { boolen = true; } else { boolen = false; }
            traits.Add("Heat_lover", boolen);

            /*
            //wage preferance
            float levelSum = inventorySkill + customerServiceSkill + managementSkill + janitorialSkill + engineeringSkill;
            float normalPay = Controller.Instance.requestedWages[occupation - 1] * ((TransitionController.Instance.wageLevelIncrease * levelSum) + 1);
            if (requestedPay < normalPay / 1.05f) { boolen = true; } else { boolen = false; }
            traits.Add("Cheap", boolen);
            if (requestedPay > normalPay * 1.05f) { boolen = true; } else { boolen = false; }
            traits.Add("Exspensive", boolen);
            */
            /*
            //wage preferance
            if (Random.Range(0, 100) > 75) { boolen = true; } else { boolen = false; }
            traits.Add("Cheap", boolen);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("Exspensive", boolen);
            */
            //social Skills
            number = Random.Range(0f, 10f);//5 average
            //if (number <= 2f) { boolen = true; } else { boolen = false; }
            //traits.Add("Introvert", boolen);
            traits.Add("Introvert", false);
            if (number >= 7f) { boolen = true; } else { boolen = false; }
            traits.Add("Extrovert", boolen);

            //How mental breaks impact
            /*
            number = Random.Range(1, 75);//37.5 average
            if (number < 15) { boolen = true; } else { boolen = false; }
            traits.Add("Unfaithful", boolen);
            if (number > 60) { boolen = true; } else { boolen = false; }
            traits.Add("loyal", boolen);
            */
            traits.Add("Unfaithful", false);
            traits.Add("loyal", false);

            if (status != Status.owner)
            {
                if (Random.Range(0, 100) > 95) { traits.Add("Pyromaniac", true); } else { traits.Add("Pyromaniac", false); }
                if (Random.Range(0, 100) > 95) { traits.Add("Clumsy", true); } else { traits.Add("Clumsy", false); }
                if (Random.Range(0, 100) > 95) { traits.Add("Lunatic", true); } else { traits.Add("Lunatic", false); }
                if (Random.Range(0, 100) > 95) { traits.Add("Angry", true); } else { traits.Add("Angry", false); }
            }
            else
            {
                traits.Add("Pyromaniac", false);
                traits.Add("Clumsy", false);
                traits.Add("Lunatic", false);
                traits.Add("Angry", false);
            }
            /*
            //severe weather
            bool boolen = false;
            if (Random.Range(0, 100) > 90) { boolen = true; } else { boolen = false; }
            traits.Add("weather_phobia", boolen);
            if (Random.Range(0, 100) > 50 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("weather_philia", boolen);

            //shopping times
            prefferedTime = Random.Range(3, 21);
            if (prefferedTime < 7) { boolen = true; } else { boolen = false; }
            traits.Add("early_bird", boolen);
            if (prefferedTime > 17) { boolen = true; } else { boolen = false; }
            traits.Add("night_owl", boolen);

            //holidays
            if (Random.Range(0, 100) > 50) { boolen = true; } else { boolen = false; }
            traits.Add("workaholic", boolen);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("vacationer", boolen);

            //stress speed
            stressfulness = Random.Range(0.5f, 1.5f);//1 average
            if (stressfulness < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("stressless", boolen);
            if (stressfulness > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("stressful", boolen);

            //How great everything impacts this customer
            learningSpeed = Random.Range(0.5f, 1.5f);//24 average
            if (learningSpeed < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_learner", boolen);
            if (learningSpeed > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_learner", boolen);

            //how much stress is reduced during time off
            stressRelease = Random.Range(0.5f, 1.5f);//1 average
            if (stressRelease < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("burnt_out", boolen);
            if (stressRelease > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("work_philia", boolen);

            //walking speed
            baseSpeed = Random.Range(-10, 10);//40 average
            if (baseSpeed < -5f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_walker", boolen);
            if (baseSpeed > 5f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_walker", boolen);

            //working speed
            baseWorkSpeed = Random.Range(-10, 10);//38 average//33 min
            if (baseWorkSpeed < -5f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_worker", boolen);
            if (baseWorkSpeed > 5f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_worker", boolen);

            //age
            if (age < 21) { boolen = true; } else { boolen = false; }
            traits.Add("young", boolen);
            if (age > 65) { boolen = true; } else { boolen = false; }
            traits.Add("old", boolen);

            //music effect
            auditorySensitivity = Random.Range(0.5f, 1.5f);//1 average
            if (auditorySensitivity < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("auditory_deaf", boolen);
            if (auditorySensitivity > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("auditory_sensitivity", boolen);

            //calling out chance //chance of not coming in
            callOutChance = Random.Range(0.01f, 10f);//5 average
            if (callOutChance < 2.5f) { boolen = true; } else { boolen = false; }
            traits.Add("reliable", boolen);
            if (callOutChance > 7.5f) { boolen = true; } else { boolen = false; }
            traits.Add("unreliable", boolen);

            //temp preferance
            tempPref = Random.Range(68f, 76f);//72 average
            if (tempPref < 70f) { boolen = true; } else { boolen = false; }
            traits.Add("Cold Lover", boolen);
            if (tempPref > 74f) { boolen = true; } else { boolen = false; }
            traits.Add("Heat lover", boolen);

            //social Skills
            socialSkills = Random.Range(0f, 10f);//3 average
            if (socialSkills < 3.3f) { boolen = true; } else { boolen = false; }
            traits.Add("Introvert", boolen);
            if (socialSkills > 6.6f) { boolen = true; } else { boolen = false; }
            traits.Add("Extrovert", boolen);

            //How mental breaks impact
            loyalty = Random.Range(1, 75);//24 average
            if (loyalty < 15) { boolen = true; } else { boolen = false; }
            traits.Add("Unfaithful", boolen);
            if (loyalty > 60) { boolen = true; } else { boolen = false; }
            traits.Add("loyal", boolen);


            */

            transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().SetSprites(-1, 0, 0, 0, 0, 0, 0, isFemale, null);
        }
    }
    private void SetUpWorkDays()
    {
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0: workDays.Add("Sunday", true); break;
                case 1: workDays.Add("Monday", true); break;
                case 2: workDays.Add("Tuesday", true); break;
                case 3: workDays.Add("Wednesday", true); break;
                case 4: workDays.Add("Thursday", true); break;
                case 5: workDays.Add("Friday", true); break;
                case 6: workDays.Add("Saturday", true); break;
            }
        }
    }
    private float GetWageStress()
    {
        if (status != Status.owner)
        {
            if (hourlyWage >= desiredWage - 0.25f && hourlyWage < desiredWage + 0.25f) { return 1f; }

            if (hourlyWage >= desiredWage + 0.25f && hourlyWage < desiredWage + 0.5f) { return 0.9f; }
            if (hourlyWage >= desiredWage + 0.5f && hourlyWage < desiredWage + 1f) { return 0.75f; }
            if (hourlyWage >= desiredWage + 1) { return 0.5f; }

            if (hourlyWage <= desiredWage - 0.25f && hourlyWage > desiredWage - 0.5f) { return 1.25f; }
            if (hourlyWage <= desiredWage - 0.5f && hourlyWage > desiredWage - 1f) { return 1.5f; }
            if (hourlyWage <= desiredWage - 1) { return 2f; }
        }

        return 1;
    }
    private void SetStats()
    {
        float value = 0;

        value = Controller.Instance.baseWalkSpeed;
        if (traits["slow_walker"]) { value -= 15; }
        if (traits["fast_walker"]) { value += 15; }
        stats.Add("baseSpeed", value);

        value = Controller.Instance.baseWorkSpeed;
        if (traits["slow_worker"]) { value += 10; }
        if (traits["fast_worker"]) { value -= 15; }
        stats.Add("baseWorkSpeed", value);

        value = Controller.Instance.baseAudio;
        if (traits["deaf"]) { value -= 1f; }
        if (traits["sensitive"]) { value += 1f; }
        stats.Add("sensitive", value);

        value = Controller.Instance.baseLearning;
        if (traits["slow_learner"]) { value -= 0.5f; }
        if (traits["fast_learner"]) { value += 1f; }
        stats.Add("learningSpeed", value);

        value = Controller.Instance.baseCalloutChance;
        if (traits["reliable"]) { value -= 3f; }
        if (traits["unreliable"]) { value += 7f; }
        stats.Add("callOutChance", value);

        value = Controller.Instance.baseStressRelease;
        if (traits["burnt_out"]) { value -= 0.5f; }
        if (traits["work_lover"]) { value += 1f; }
        stats.Add("stressRelease", value);

        value = Controller.Instance.baseStressAccumulate;
        if (traits["stressless"]) { value -= 0.5f; }
        if (traits["stressful"]) { value += 1f; }
        stats.Add("stressfulness", value);

        value = Controller.Instance.baseLoaylty;
        if (traits["Unfaithful"]) { value -= 25f; }
        if (traits["loyal"]) { value += 50f; }
        stats.Add("loyalty", value);

        value = Controller.Instance.baseSocial;
        if (traits["Introvert"]) { value -= 25f; }
        if (traits["Extrovert"]) { value += 25f; }
        stats.Add("socialSkills", value);
    }

    public void StuckFailSafe() { StuckPositionFailSafe(); animator.Rebind(); RemoveAllClaims(); Debug.LogError("I got stuck!"); }
    public void StuckPositionFailSafe() { transform.position = Controller.Instance.entrances[0].entranceNode.transform.position; }
    public GameObject mentalBreakTarget;
}