using ArchDawn.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapController;
using static Wall;

public class SaveController : MonoBehaviour
{
    public static SaveController Instance { get; private set; }
    private string SAVE_FOLDER;
    private string CONFIG_FOLDER;
    private string MODS_FOLDER;
    private string ITEMS_FOLDER;
    private string BUILDINGS_FOLDER;
    private string BUILDINGS_SPRITES_FOLDER;
    private string TILES_FOLDER;
    private string SPRITES_FOLDER;
    private string GAMEPLAY_FOLDER;
    private string EVENTS_FOLDER;
    private string LOANS_FOLDER;
    private string CHARACTER_VISUALS_FOLDER;

    [SerializeField] ItemSO itemSOBase;
    [SerializeField] Sprite tileExample;

    [Space(10)]
    [Header("Settings")]
    public bool pauseAtMidnight;

    [Space(10)]
    [Header("Other")]
    [SerializeField] private Wall[] walls;
    private List<bool> wallBools = new List<bool>();
    private List<int> wallTypes = new List<int>();
    [SerializeField] public GameObject loading;
    public bool willAutosave;
    public bool finishedLoading;
    [SerializeField] private SettingsButton autosaveButton;

    private void Awake()
    {
        Instance = this;
        SAVE_FOLDER = Application.dataPath + "/Saves/";
        CONFIG_FOLDER = Application.dataPath + "/Configs/";
        MODS_FOLDER = Application.dataPath + "/Mods/";
        ITEMS_FOLDER = MODS_FOLDER + "/Items/";
        BUILDINGS_FOLDER = MODS_FOLDER + "Buildings/";
        BUILDINGS_SPRITES_FOLDER = BUILDINGS_FOLDER + "Sprites/";
        TILES_FOLDER = BUILDINGS_FOLDER + "Tiles/";
        SPRITES_FOLDER = MODS_FOLDER + "/Sprites/";
        GAMEPLAY_FOLDER = MODS_FOLDER + "Gameplay/";
        EVENTS_FOLDER = GAMEPLAY_FOLDER + "/Events/";
        LOANS_FOLDER = GAMEPLAY_FOLDER + "/Loans/";
        CHARACTER_VISUALS_FOLDER = GAMEPLAY_FOLDER + "Character_Sprites/"; 
    }
    private void Start()
    {
        StartMods();
        StartConfigs();
        //StartDelay
    }
    private void StartDelay() { Controller.Instance.StartUp(); }
    //-------------------------------------------------------------------------------------------------------Saving
    private bool autoSaving;
    public void AutoSave()
    {
        if (willAutosave)
        {
            autoSaving = true;
            TickSystem.Instance.PauseMenu();
            Invoke("SaveGame", 0.1f);
        }
    }
    public void SaveGame()
    {
        loading.SetActive(true);
        if (!Directory.Exists(SAVE_FOLDER)) { Directory.CreateDirectory(SAVE_FOLDER); }

        ToSaveVariables();
        ToSaveTiles();
        ToSaveCustomers();
        ToSaveEmployees();
        ToSaveBuildings();
        ToSaveItems();
        ToSaveUI();
        ToSaveCompetitors();

        UIController.Instance.CreateLog(4, "Game Saved!", "Manager", 4);//"Game Saved!", Color.green);
    }
    private void PauseAtMidnight() { TickSystem.Instance.PauseMenu(); }
    public void SetPauseAtMidnight() { if (!pauseAtMidnight) { pauseAtMidnight = true; } else { pauseAtMidnight = false; } }
    public void SetAutoSave() { if (!willAutosave) { willAutosave = true; } else { willAutosave = false; } }
    public void MouseSavePopUp() { UtilsClass.CreateWorldTextPopup("Game Saved!", UtilsClass.GetMouseWorldPosition()); }
    private void ToSaveVariables()
    {
        string reducedString1 = UIController.Instance.moneyGainedText.text.Substring(2);
        string reducedString2 = UIController.Instance.moneyLostText.text.Substring(2);

        List<string> unlockedSpecialItems = new List<string>();
        List<string> unlockedSpecialSuppliers = new List<string>();
        List<string> unlockedSpecialSupplierItems = new List<string>();
        for(int i = 0; i < Controller.Instance.unlockedSpecialItems.Count; i++) { unlockedSpecialItems.Add(Controller.Instance.unlockedSpecialItems[i].myName); }
        foreach (KeyValuePair<string, string> pair in Controller.Instance.unlockedSpecialManufactorers) { unlockedSpecialSuppliers.Add(pair.Key); unlockedSpecialSupplierItems.Add(pair.Value); }

        List<string> days = new List<string>();
        List<bool> openbools = new List<bool>();
        foreach (KeyValuePair<string, bool> pair in Controller.Instance.storeOpenDays) { days.Add(pair.Key); openbools.Add(pair.Value); }

        SaveVariables variables = new SaveVariables
        {

            money = Controller.Instance.money,
            population = Controller.Instance.customers.Count,
            populationGrowth = TransitionController.Instance.cityGrowth,
            minutes = UIController.Instance.minutes,
            hour = UIController.Instance.hour,
            day = UIController.Instance.day,
            month = UIController.Instance.month,
            year = UIController.Instance.year,
            dayOfTheYear = UIController.Instance.dayOfTheYear,
            activeEvent = UIController.Instance.typeOfDay,
            chosenMusic = Controller.Instance.previousMusicChoice,
            pitchSet = Controller.Instance.setPitch,
            volumeSet = Controller.Instance.setVolume,
            numberOfCompetitors = TransitionController.Instance.numberOfCompetitors,
            weekDay = UIController.Instance.weekday,
            played_Months = UIController.Instance.playedMonths,

            global_Callout_Chance_Multiplier = Controller.Instance.globalCalloutChanceMultiplier,
            global_Loyalty_Multiplier = Controller.Instance.globalLoyaltyMultiplier,
            competitor_Bailouts = Controller.Instance.competitorBailouts,
            global_Employee_Stress_Multiplier = Controller.Instance.globalEmployeeStressMultiplier,
            global_Item_Demand_Swing_Multiplier = Controller.Instance.globalItemDemandSwingMultiplier,
            global_Customer_Wage_Multiplier = Controller.Instance.globalCustomerWageMultiplier,
            competitor_Free_Money = Controller.Instance.competitorFreeMoney,
            game_Over = UIController.Instance.gameOver,

            MoneyGainedText = float.Parse(reducedString1),
            MoneyLostText = float.Parse(reducedString2),
            MoneyGained = UIController.Instance.MoneyGained,
            MoneyLost = UIController.Instance.MoneyLost,

            storeOpen = Controller.Instance.storeOpen,
            storeClose = Controller.Instance.storeClose,
            shutdownOpen = Controller.Instance.shutdownOpen,
            shutdownClose = Controller.Instance.shutdownClose,
            shipmentTime = Controller.Instance.shipmentTime,
            shipmentArrived = Controller.Instance.shipmentArrived,
            weekdays = days,
            storeOpenDays = openbools,
            playedDays = UIController.Instance.playedDays,

            tax = TransitionController.Instance.tax,
            inflation = TransitionController.Instance.inflation,
            items = TransitionController.Instance.items,
            jobAmount = TransitionController.Instance.jobAmount,
            mapSelection = TransitionController.Instance.mapSelection,
            leasePricePerSquareFoot = TransitionController.Instance.leasePricePerSquareFoot,
            averageTemp = TransitionController.Instance.averageTemp,
            highTemp = TransitionController.Instance.highTemp,
            lowTemp = TransitionController.Instance.lowTemp,
            coldestDayOfTheYear = TransitionController.Instance.coldestDayOfTheYear,
            HotestDayOfTheYear = TransitionController.Instance.HotestDayOfTheYear,
            tileZones = TransitionController.Instance.tileZones,
            difficulty = TransitionController.Instance.difficulty,
            levelDifficulty = TransitionController.Instance.levelDifficulty,
            totalDifficulty = TransitionController.Instance.totalDifficulty,
            BuyoutStartDate = TransitionController.Instance.BuyoutStartDate,
            moneyWinAmount = TransitionController.Instance.moneyWinAmount,
            tutorialLevel = TransitionController.Instance.tutorialLevel,
            goal = TransitionController.Instance.goal,
            goalAmount = TransitionController.Instance.goalAmount,
            mapName = TransitionController.Instance.mapName,
            wageLevelIncrease = TransitionController.Instance.wageLevelIncrease,
            averageCustomerHourlyIncome = TransitionController.Instance.averageCustomerHourlyIncome,
            goalreward = TransitionController.Instance.goalreward,
            goalDisc = TransitionController.Instance.goalDisc,

            boughtZones = MapController.Instance.boughtZones,
            unlockedSpecialItems = unlockedSpecialItems,
            unlockedSpecialSuppliers = unlockedSpecialSuppliers,
            unlockedSpecialSupplierItems = unlockedSpecialSupplierItems,

            MerchandiceExpiredMoneyLost = Controller.Instance.MerchandiceExpiredMoneyLost,
            itemsSoldTotal = Controller.Instance.itemsSoldTotal,
            itemsSoldDaily = Controller.Instance.itemsSoldDaily,
            itemsSoldPast = Controller.Instance.itemsSoldPast,
            electricityCost = Controller.Instance.electricityCost,
            leaseDue = Controller.Instance.leaseDue,
            ownedTilesCount = MapController.Instance.ownedTilesCount,
        };

        string jsonVariables = JsonUtility.ToJson(variables, true);

        File.WriteAllText(SAVE_FOLDER + "/savedVariables.text", jsonVariables);
    }
    private void ToSaveTiles()
    {
        StartCoroutine(SavingTiles());
    }
    private IEnumerator SavingTiles()
    {
        MyGrid<NewGrid> grid = MapController.Instance.grid;
        List<NewGrid.SaveObject> tilemapObjectSaveObjectList = new List<NewGrid.SaveObject>();
        /*
        //this works?!
        foreach (Vector2Int marker in MapController.Instance.savedTiles)
        {
            Vector3 spot = new Vector3(marker.x * 10, marker.y * 10, 0);
            NewGrid tilemapObject = grid.GetGridObject(spot);
            tilemapObjectSaveObjectList.Add(tilemapObject.Save());
        }
        */
        int playableGridStart = TransitionController.Instance.playablegridstart;
        int playableGridSize = TransitionController.Instance.playablegridsize;

        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                NewGrid tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
                yield return new WaitForEndOfFrame();
            }
        }
            /*
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    Vector2Int test = new Vector2Int(x,y); bool found = false;
                    foreach (Vector2Int marker in MapController.Instance.savedTiles) { if (test == marker) { Debug.Log("Found " + test); found = true; break; } }
                    if (found)
                    {
                        NewGrid tilemapObject = grid.GetGridObject(x, y);
                        tilemapObjectSaveObjectList.Add(tilemapObject.Save());
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            */


        SaveGrid saveGrid = new SaveGrid { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        //SaveSystem.SaveObject(saveGrid);
        string jsonGrid = JsonUtility.ToJson(saveGrid, true);
        File.WriteAllText(SAVE_FOLDER + "/savedGrid.text", jsonGrid);
        yield return (null);
        loading.SetActive(false);

        if (autoSaving)
        {
            autoSaving = false;
            TickSystem.Instance.UnPause();
            if (pauseAtMidnight) { Invoke("PauseAtMidnight", 1); }
        }
    }
    private void ToSaveBuildings()
    {
        List<Building.SaveObject> buildings = new List<Building.SaveObject>();
        foreach(Building building in FindObjectsOfType<Building>())
        {
            buildings.Add(building.Save());
        }

        SaveBuildings saveBuildings = new SaveBuildings { buildingsArray = buildings.ToArray() };

        string jsonGrid = JsonUtility.ToJson(saveBuildings, true);
        File.WriteAllText(SAVE_FOLDER + "/savedBuildings.text", jsonGrid);
    }
    private void ToSaveItems()
    {
        List<Item.SaveObject> items = new List<Item.SaveObject>();
        foreach (Item item in FindObjectsOfType<Item>())
        {
            items.Add(item.Save());
        }

        SaveItems saveItems = new SaveItems { itemsArray = items.ToArray() };

        string jsonGrid = JsonUtility.ToJson(saveItems, true);
        File.WriteAllText(SAVE_FOLDER + "/savedItems.text", jsonGrid);
    }
    private void ToSaveCustomers()
    {
        List<Customer2.SaveObject> customers = new List<Customer2.SaveObject>();
        foreach (Customer2 customer in FindObjectsOfType<Customer2>())
        {
            customers.Add(customer.Save());
        }

        SaveCustomers savedCustomers = new SaveCustomers { CustomersArray = customers.ToArray() };

        string jsonGrid = JsonUtility.ToJson(savedCustomers, true);
        File.WriteAllText(SAVE_FOLDER + "/savedCustomers.text", jsonGrid);
    }
    private void ToSaveEmployees()
    {
        List<Employee2.SaveObject> employees = new List<Employee2.SaveObject>();
        foreach (Employee2 employee in FindObjectsOfType<Employee2>())
        {
            employees.Add(employee.Save());
        }

        SaveEmployees savedEmployees = new SaveEmployees { EmployeesArray = employees.ToArray() };

        string jsonGrid = JsonUtility.ToJson(savedEmployees, true);
        File.WriteAllText(SAVE_FOLDER + "/savedEmployees.text", jsonGrid);
    }
    private void ToSaveCompetitors()
    {
        List<Competitor.SaveObject> competitors = new List<Competitor.SaveObject>();
        foreach (Competitor compition in FindObjectsOfType<Competitor>())
        {
            competitors.Add(compition.Save());
        }

        SaveCompetitors savedCompetitor = new SaveCompetitors { CompetitorArray = competitors.ToArray() };

        string jsonGrid = JsonUtility.ToJson(savedCompetitor, true);
        File.WriteAllText(SAVE_FOLDER + "/savedCompetitors.text", jsonGrid);
    }
    private void ToSaveUI()
    {
        itemNames.Clear();
        amounts.Clear();
        multipliers.Clear();
        manufacturers.Clear();
        //set values
        foreach (UIItemOrder order in OrderManager.Instance.orders)
        {
            itemNames.Add(order.item.myName);
            amounts.Add(order.dailyOrders / order.orderMultiplier);
            multipliers.Add(order.markUpSlider.value);
            manufacturers.Add(order.supplier);
        }
        foreach(Wall wall in walls)
        {
            if (wall.type == Wall.WallType.entrance) { wallBools.Add(true); }
            else { wallBools.Add(false); }
            wallTypes.Add((int)wall.entranceType);
        }
        List<string> loanNames = new List<string>();
        List<float> loanValues = new List<float>();
        foreach (Loan loan in Controller.Instance.loans)
        {
            loanNames.Add(loan.myName);
            loanValues.Add(loan.amountRemaining);
        }

        //messy code, I know
        bool winCondition1 = false;
        int goalType1 = -1;
        float goalAmount1 = -1;
        string itemName1 = "";
        float progress1 = -1;
        float timeRemaining1 = -1;
        int rewardInt1 = -1;

        bool winCondition2 = false;
        int goalType2 = -1;
        float goalAmount2 = -1;
        string itemName2 = "";
        float progress2 = -1;
        float timeRemaining2 = -1;
        int rewardInt2 = -1;

        bool winCondition3 = false;
        int goalType3 = -1;
        float goalAmount3 = -1;
        string itemName3 = "";
        float progress3 = -1;
        float timeRemaining3 = -1;
        int rewardInt3 = -1;
        if (UIController.Instance.activeGoals.Count > 0) 
        { 
            Goal thisGoal = UIController.Instance.activeGoals[0]; 
            if (thisGoal.reward == Goal.Rewards.win) { winCondition1 = true; } 
            goalType1 = (int)thisGoal.goal; 
            goalAmount1 = thisGoal.amountNeeded; 
            if (thisGoal.targetItem != null) { itemName1 = thisGoal.targetItem.myName; }
            progress1 = thisGoal.progress; 
            timeRemaining1 = thisGoal.deadline; 
            rewardInt1 = (int)thisGoal.reward; }
        if (UIController.Instance.activeGoals.Count > 1) { Goal thisGoal = UIController.Instance.activeGoals[1]; if (thisGoal.reward == Goal.Rewards.win) { winCondition2 = true; } goalType2 = (int)thisGoal.goal; goalAmount2 = thisGoal.amountNeeded; if (thisGoal.targetItem != null) { itemName2 = thisGoal.targetItem.myName; } progress2 = thisGoal.progress; timeRemaining2 = thisGoal.deadline; rewardInt2 = (int)thisGoal.reward; }
        if (UIController.Instance.activeGoals.Count > 2) { Goal thisGoal = UIController.Instance.activeGoals[2]; if (thisGoal.reward == Goal.Rewards.win) { winCondition3 = true; } goalType3 = (int)thisGoal.goal; goalAmount3 = thisGoal.amountNeeded; if (thisGoal.targetItem != null) { itemName3 = thisGoal.targetItem.myName; } progress3 = thisGoal.progress; timeRemaining3 = thisGoal.deadline; rewardInt3 = (int)thisGoal.reward; }

        List<string> shiftNames = new List<string>();
        List<int> shiftStartTimes = new List<int>();
        List<int> shiftEndTimes = new List<int>();
        for (int i = 0; i < ScheduleController.Instance.sets.Count; i++)
        {
            shiftNames.Add(ScheduleController.Instance.sets[i].myName);
            shiftStartTimes.Add(ScheduleController.Instance.sets[i].startTime);
            shiftEndTimes.Add(ScheduleController.Instance.sets[i].endTime);
        }

        Advertising.Instance.SaveAdvertValues(out bool auto, out int item, out bool popButton, out bool demandButton, out bool needButton, out float itemsSlider, out float popsSlider, out float daileySlider);
        SaveUI variables = new SaveUI
        {
            autosave = this.willAutosave,

            hiringLevel = UIController.Instance.hiringLevel,
            hiringWage = UIController.Instance.hiringWage,
            hiringOccupation = UIController.Instance.hiringOccupation,

            itemNames = this.itemNames,
            amounts = this.amounts,
            multipliers = this.multipliers,
            manufacturers = this.manufacturers,

            wallBools = this.wallBools,
            wallTypes = this.wallTypes, 

            calanderValues = CalanderController.Instance.GetValues(),
            calanderNames = CalanderController.Instance.GetNames(),
            calanderWeather = CalanderController.Instance.GetWeather(),

            loanNames = loanNames,
            loanValues = loanValues,

            autoAdvert = auto,
            advertItemSelected = item,
            populationBought = popButton,
            itemGrowthBought = demandButton,
            needBought = needButton,
            itemSlider = itemsSlider,
            popSlider = popsSlider,
            needSlider = daileySlider,

            thermoStatSetting = Controller.Instance.tempSet,
            outsideTemp = Controller.Instance.outsideTemp,
            insideTemp = Controller.Instance.insideTemp,
            storeName = Controller.Instance.storeName,
            revertToHiredTasks = Controller.Instance.revertToHiredTasks,
            ifDoneStocking = Controller.Instance.ifDoneStocking,
            ifDoneCashiering = Controller.Instance.ifDoneCashiering,
            ifDoneCleaning = Controller.Instance.ifDoneCleaning,
            ifDoneBuilding = Controller.Instance.ifDoneBuilding,
            ifDoneManaging = Controller.Instance.ifDoneManaging,
            priorityTask = Controller.Instance.priorityTask,

            storeCredit = Controller.Instance.storeCredit,
            customerEntry = Controller.Instance.customerEntry,
            customerMemberships = Controller.Instance.customerMemberships,
            billpayments = Controller.Instance.billpayments,
            employeePaychecks = Controller.Instance.employeePaychecks,

            winCondition1 = winCondition1,
            goalType1 = goalType1,
            goalAmount1 = goalAmount1,
            itemName1 = itemName1,
            progress1 = progress1,
            timeRemaining1 = timeRemaining1,
            rewardInt1 = rewardInt1,


            winCondition2 = winCondition2,
            goalType2 = goalType2,
            goalAmount2 = goalAmount2,
            itemName2 = itemName2,
            progress2 = progress2,
            timeRemaining2 = timeRemaining2,
            rewardInt2 = rewardInt2,


            winCondition3 = winCondition3,
            goalType3 = goalType3,
            goalAmount3 = goalAmount3,
            itemName3 = itemName3,
            progress3 = progress3,
            timeRemaining3 = timeRemaining3,
            rewardInt3 = rewardInt3,

            shiftNames = shiftNames,
            shiftStartTimes = shiftStartTimes,
            shiftEndTimes = shiftEndTimes,
        };

        string jsonVariables = JsonUtility.ToJson(variables, true);

        File.WriteAllText(SAVE_FOLDER + "/savedUI.text", jsonVariables);

        //string jsonDict = JsonConvert.SerializeObject(Controller.Instance.orderLog, Formatting.Indented);
        //        File.WriteAllText(SAVE_FOLDER + "/savedUI.text", jsonVariables);
        //string jsonVariables = JsonUtility.ToJson(variables);
        //string jsonLoans = JsonConvert.SerializeObject(Controller.Instance.loans, Formatting.Indented);
        //File.WriteAllText(SAVE_FOLDER + "/savedLoans.text", jsonLoans);
    }

    //-------------------------------------------------------------------------------------------------------Loading
    private List<string> itemNames = new List<string>();
    private List<int> amounts = new List<int>();
    private List<float> multipliers = new List<float>();
    [SerializeField] private List<string> manufacturers = new List<string>();
    [SerializeField] private Building buildingReferance;
    [SerializeField] private Item itemReferance;
    [SerializeField] private Competitor competitorReferance;
    [SerializeField] private Customer2 customerReferance;
    [SerializeField] private Employee2 employeeReferance;
    [SerializeField] private CustomerSO customerSOReferance;
    [SerializeField] private EmployeeSO employeeSOReferance;
    public void LoadGameButton()
    {
        TransitionController.Instance.loadGame = true;
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }
    public void LoadGame()
    {
        loading.SetActive(true);
        if (Directory.Exists(SAVE_FOLDER))
        {
            TransitionController.Instance.loadGame = true;

            ToLoadVariables();
            ToLoadTiles();
            ToLoadCustomers();
            ToLoadEmployees();
            ToLoadUI();
            ToLoadCompetitors();
            Invoke("ToLoadBuildings", 1.5f);
            Invoke("ToLoadItems", 2f);
        }
        else { Debug.Log("No save found"); }

        UIController.Instance.LoadOpenStore();
        Controller.Instance.LoadGame();
        Invoke("OrderDelay", 2.5f);
    }
    private void OrderDelay() { Controller.Instance.LoadOrders(itemNames, amounts, multipliers, manufacturers); }

    private void ToLoadVariables()
    {
        if (File.Exists(SAVE_FOLDER + "/savedVariables.text"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedVariables.text");

            SaveVariables variables = JsonUtility.FromJson<SaveVariables>(saveString);

            UIController.Instance.gameOver = variables.game_Over;
            Controller.Instance.money = variables.money;
            TransitionController.Instance.cityPopulation = variables.population;
            TransitionController.Instance.cityGrowth = variables.populationGrowth;
            UIController.Instance.minutes = variables.minutes;
            UIController.Instance.hour = variables.hour;
            UIController.Instance.day = variables.day;
            UIController.Instance.month = variables.month;
            UIController.Instance.year = variables.year;
            UIController.Instance.dayOfTheYear = variables.dayOfTheYear;
            UIController.Instance.typeOfDay = variables.activeEvent;
            TransitionController.Instance.numberOfCompetitors = variables.numberOfCompetitors;
            UIController.Instance.playedMonths = variables.played_Months;

            Controller.Instance.globalCalloutChanceMultiplier = variables.global_Callout_Chance_Multiplier;
            Controller.Instance.globalLoyaltyMultiplier = variables.global_Loyalty_Multiplier;
            Controller.Instance.competitorBailouts = variables.competitor_Bailouts;
            Controller.Instance.globalEmployeeStressMultiplier = variables.global_Employee_Stress_Multiplier;
            Controller.Instance.globalItemDemandSwingMultiplier = variables.global_Item_Demand_Swing_Multiplier;
            Controller.Instance.globalCustomerWageMultiplier = variables.global_Customer_Wage_Multiplier;
            Controller.Instance.competitorFreeMoney = variables.competitor_Free_Money;

            Controller.Instance.setPitch = variables.pitchSet;//UI?
            Controller.Instance.setVolume = variables.volumeSet;
            Controller.Instance.LoadMusic(variables.chosenMusic);
            UIController.Instance.weekday = variables.weekDay;

            Controller.Instance.storeOpen = variables.storeOpen;//UI?
            Controller.Instance.storeClose = variables.storeClose;
            Controller.Instance.shutdownOpen = variables.shutdownOpen;
            Controller.Instance.shutdownClose = variables.shutdownClose;
            Controller.Instance.shipmentTime = variables.shipmentTime;
            Controller.Instance.shipmentArrived = variables.shipmentArrived;
            for (int i = 0; i < variables.weekdays.Count; i++) { Controller.Instance.storeOpenDays.Add(variables.weekdays[i], variables.storeOpenDays[i]); }
            UIController.Instance.playedDays = variables.playedDays;

            UIController.Instance.moneyGainedText.text = "+$" + variables.MoneyGainedText.ToString("f2");
            UIController.Instance.moneyLostText.text = "-$" + variables.MoneyLostText.ToString("f2");
            UIController.Instance.MoneyGained = variables.MoneyGained;
            UIController.Instance.MoneyLost = variables.MoneyLost;

            TransitionController.Instance.tax = variables.tax;
            TransitionController.Instance.inflation = variables.inflation;
            TransitionController.Instance.items = variables.items;
            TransitionController.Instance.jobAmount = variables.jobAmount;
            TransitionController.Instance.mapSelection = variables.mapSelection;
            TransitionController.Instance.leasePricePerSquareFoot = variables.leasePricePerSquareFoot;
            TransitionController.Instance.averageTemp = variables.averageTemp;
            TransitionController.Instance.highTemp = variables.highTemp;
            TransitionController.Instance.lowTemp = variables.lowTemp;
            TransitionController.Instance.coldestDayOfTheYear = variables.coldestDayOfTheYear;
            TransitionController.Instance.HotestDayOfTheYear = variables.HotestDayOfTheYear;
            TransitionController.Instance.tileZones = variables.tileZones;
            TransitionController.Instance.difficulty = variables.difficulty;
            TransitionController.Instance.levelDifficulty = variables.levelDifficulty;
            TransitionController.Instance.totalDifficulty = variables.totalDifficulty;
            TransitionController.Instance.BuyoutStartDate = variables.BuyoutStartDate;
            TransitionController.Instance.moneyWinAmount = variables.moneyWinAmount;
            TransitionController.Instance.tutorialLevel = variables.tutorialLevel;
            TransitionController.Instance.goal = variables.goal;
            TransitionController.Instance.goalAmount = variables.goalAmount;
            TransitionController.Instance.mapName = variables.mapName;
            TransitionController.Instance.wageLevelIncrease = variables.wageLevelIncrease;
            TransitionController.Instance.goalreward = variables.goalreward;
            TransitionController.Instance.goalDisc = variables.goalDisc;
            TransitionController.Instance.averageCustomerHourlyIncome = variables.averageCustomerHourlyIncome;

            MapController.Instance.boughtZones = variables.boughtZones;

            Controller.Instance.MerchandiceExpiredMoneyLost = variables.MerchandiceExpiredMoneyLost;
            Controller.Instance.itemsSoldTotal = variables.itemsSoldTotal;
            Controller.Instance.itemsSoldDaily = variables.itemsSoldDaily;
            Controller.Instance.itemsSoldPast = variables.itemsSoldPast;
            Controller.Instance.electricityCost = variables.electricityCost;
            Controller.Instance.leaseDue = variables.leaseDue;
            MapController.Instance.ownedTilesCount = variables.ownedTilesCount;

            thisVariables = variables;
            Invoke("SpecialItemsDelay", 5);

            for (int i = 0; i < variables.unlockedSpecialSuppliers.Count; i++) { Controller.Instance.unlockedSpecialManufactorers.Add(variables.unlockedSpecialSuppliers[i], variables.unlockedSpecialSupplierItems[i]); }

            Controller.Instance.MoneyValueChange(0, UtilsClass.GetMouseWorldPosition(), false, true);
            UIController.Instance.LoadedDay();
        }
        else { Debug.Log("No variables found"); }
    }
    private SaveVariables thisVariables;
    private void SpecialItemsDelay()
    {
        foreach (string itemName in thisVariables.unlockedSpecialItems)
        {
            foreach (ItemSO item in Controller.Instance.items)
            {
                if (item.myName == itemName)
                {
                    Controller.Instance.unlockedSpecialItems.Add(item); Controller.Instance.NewItemCheck(item);
                    break;
                }
            }

        }
    }
    private void ToLoadTiles()
    {
        if (File.Exists(SAVE_FOLDER + "/savedGrid.text"))
        {
            MyGrid<NewGrid> grid = MapController.Instance.grid;

            //SaveGrid saveGrid = SaveSystem.LoadMostRecentObject<SaveObject>();
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedGrid.text");
            SaveGrid saveGrid = JsonUtility.FromJson<SaveGrid>(saveString);

            foreach (NewGrid.SaveObject tilemapObjectSaveObject in saveGrid.tilemapObjectSaveObjectArray)
            {
                NewGrid tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.position.x, tilemapObjectSaveObject.position.y);
                tilemapObject.Load(tilemapObjectSaveObject);
            }
        }
        else { Debug.Log("No tiles found"); }
    }
    private void ToLoadBuildings()
    {
        if (File.Exists(SAVE_FOLDER + "/savedBuildings.text"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedBuildings.text");
            SaveBuildings savedBuild = JsonUtility.FromJson<SaveBuildings>(saveString);
            foreach (Building.SaveObject building in savedBuild.buildingsArray)
            {
                //Building build = building.building;
                //print(build + " " + building);
                //
                BuildingSO buildingSO = null;
                foreach (BuildingSO buildS in importingBuildings)
                {
                    if (buildS.buildingName == building.buildingName) { buildingSO = buildS;  break; }
                }
                if (buildingSO != null) { buildingReferance.Load(building, buildingSO); }
                
            }
        }
        else { Debug.Log("No tiles found"); }
    }
    private void ToLoadItems()
    {
        if (File.Exists(SAVE_FOLDER + "/savedItems.text"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedItems.text");
            SaveItems savedItems = JsonUtility.FromJson<SaveItems>(saveString);

            foreach (Item.SaveObject items in savedItems.itemsArray)
            {
                //Item item = items.item;
                itemReferance.Load(items);
            }
        }
        else { Debug.Log("No tiles found"); }
    }
    private void ToLoadCustomers()
    {
        if (File.Exists(SAVE_FOLDER + "/savedCustomers.text"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedCustomers.text");
            SaveCustomers savedCustomers = JsonUtility.FromJson<SaveCustomers>(saveString);

            foreach (Customer2.SaveObject customers in savedCustomers.CustomersArray)
            {
                //Customer2 customer = customers.customer;
                customerReferance.Load(customers, customerSOReferance);
            }
        }
        else { Debug.Log("No tiles found"); }
    }
    private void ToLoadEmployees()
    {
        if (File.Exists(SAVE_FOLDER + "/savedEmployees.text"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedEmployees.text");
            SaveEmployees savedEmployees = JsonUtility.FromJson<SaveEmployees>(saveString);

            foreach (Employee2.SaveObject employees in savedEmployees.EmployeesArray)
            {
                //Employee2 employee = employees.employee;
                employeeReferance.Load(employees, employeeSOReferance);
            }
        }
        else { Debug.Log("No tiles found"); }
    }
    public void ToLoadCompetitors()
    {
        if (File.Exists(SAVE_FOLDER + "/savedCompetitors.text"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "/savedCompetitors.text");
            SaveCompetitors savedCompetitors = JsonUtility.FromJson<SaveCompetitors>(saveString);
            foreach (Competitor.SaveObject competitors in savedCompetitors.CompetitorArray)
            {
                //Competitor competitor = competitors.competitor;
                competitorReferance.Load(competitors);
            }
        }
        else { Debug.Log("No tiles found"); }
    }
    List<string> loanNames = new List<string>();
    List<float> loanValues = new List<float>();
    private void ToLoadUI()
    {

        if (File.Exists(SAVE_FOLDER + "/savedUI.text"))
        {
            string json = File.ReadAllText(SAVE_FOLDER + "/savedUI.text");
            SaveUI variables = JsonUtility.FromJson<SaveUI>(json);

            UIController.Instance.ChangeHiringLevel(variables.hiringLevel);
            UIController.Instance.hiringWageSlider.value = variables.hiringWage * 4;
            UIController.Instance.hiringOccupationSelection.value = variables.hiringOccupation;
            UIController.Instance.newHiringOccupation = variables.hiringOccupation;

            itemNames = variables.itemNames;
            amounts = variables.amounts;
            multipliers = variables.multipliers;
            manufacturers = variables.manufacturers;

            wallBools = variables.wallBools;
            wallTypes = variables.wallTypes;

            loanNames = variables.loanNames;
            loanValues = variables.loanValues;

            Controller.Instance.tempSet = variables.thermoStatSetting;
            Controller.Instance.outsideTemp = variables.outsideTemp;
            Controller.Instance.insideTemp = variables.insideTemp;
            Controller.Instance.storeName = variables.storeName;
            Controller.Instance.revertToHiredTasks = variables.revertToHiredTasks;
            Controller.Instance.ifDoneStocking = variables.ifDoneStocking;
            Controller.Instance.ifDoneCashiering = variables.ifDoneCashiering;
            Controller.Instance.ifDoneCleaning = variables.ifDoneCleaning;
            Controller.Instance.ifDoneBuilding = variables.ifDoneBuilding;
            Controller.Instance.ifDoneManaging = variables.ifDoneManaging;
            Controller.Instance.priorityTask = variables.priorityTask;

            Controller.Instance.storeCredit = variables.storeCredit;
            Controller.Instance.customerEntry = variables.customerEntry;
            Controller.Instance.customerMemberships = variables.customerMemberships;
            Controller.Instance.billpayments = variables.billpayments;
            Controller.Instance.employeePaychecks = variables.employeePaychecks;
            UIController.Instance.UpdateAutoTaskUI();
            willAutosave = variables.autosave;
            if (variables.autosave) { autosaveButton.Enable(); }


            StartCoroutine(CalanderController.Instance.SetDates(variables.calanderValues, variables.calanderNames, variables.calanderWeather));

            Advertising.Instance.LoadAdverts(variables.autoAdvert, variables.advertItemSelected, variables.populationBought, variables.itemGrowthBought, variables.needBought, variables.itemSlider, variables.popSlider, variables.needSlider);

            if (variables.goalType1 != -1) { UIController.Instance.LoadQuest(variables.goalType1, variables.goalAmount1, variables.itemName1, variables.progress1, variables.timeRemaining1, variables.rewardInt1, variables.winCondition1); }
            if (variables.goalType2 != -1) { UIController.Instance.LoadQuest(variables.goalType2, variables.goalAmount2, variables.itemName2, variables.progress2, variables.timeRemaining2, variables.rewardInt2, variables.winCondition2); }
            if (variables.goalType3 != -1) { UIController.Instance.LoadQuest(variables.goalType3, variables.goalAmount3, variables.itemName3, variables.progress3, variables.timeRemaining3, variables.rewardInt3, variables.winCondition3); }

            for (int i = 0; i < variables.shiftNames.Count; i++)
            {
                bool found = false;
                foreach(ScheduleSet set in ScheduleController.Instance.sets)
                {
                    if (set.myName == variables.shiftNames[i])
                    {
                        found = true;
                        set.LoadedStartUp(variables.shiftNames[i], variables.shiftStartTimes[i], variables.shiftEndTimes[i]);
                    }
                }
                if (!found)
                {
                    ScheduleController.Instance.NewShift(variables.shiftNames[i], variables.shiftStartTimes[i], variables.shiftEndTimes[i]);
                }
            }
            //string jsonLoans = File.ReadAllText(SAVE_FOLDER + "/savedLoans.text");

            //Controller.Instance.loans = JsonConvert.DeserializeObject<Dictionary<float, float>>(jsonLoans);
            /*
                        string saveString = File.ReadAllText(SAVE_FOLDER + "/savedUI.text");

                        SaveUI variables = JsonUtility.FromJson<SaveUI>(saveString);

                        Controller.Instance.orderLog = variables.orderLog;
            */
        }
        else { Debug.Log("No variables found"); }

        Invoke("DoorDelay", 1f);

    }
    private void DoorDelay()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            if (wallBools[i]) { walls[i].BecomeEntrance(); walls[i].ChangeEntranceType(wallTypes[i]); }
        }
        for (int i = 0; i < loanNames.Count; i++) { UIController.Instance.LoadLoans(loanValues[i], loanNames[i]); }
    }
    //-------------------------------------------------------------------------------------------------------Classes
    private class SaveVariables
    {
        //controller
        public float money;
        public int population;
        public float populationGrowth;
        public int minutes;//15 min incriments
        public int hour;
        public int day = 1;
        public int month = 1;
        public int year = 2000;
        public int dayOfTheYear;
        public string activeEvent;
        public int numberOfCompetitors;
        //specialDayEffects
        public float global_Callout_Chance_Multiplier;
        public float global_Loyalty_Multiplier;
        public bool competitor_Bailouts;
        public float global_Employee_Stress_Multiplier;
        public float global_Item_Demand_Swing_Multiplier;
        public float global_Customer_Wage_Multiplier;
        public float competitor_Free_Money;
        public int played_Months;
        public bool game_Over;

        //music
        public int chosenMusic;
        public int pitchSet;
        public int volumeSet;

        //ui
        public string weekDay;
        public float MoneyGainedText;
        public float MoneyLostText;
        public float MoneyGained;
        public float MoneyLost;
        public int playedDays;

        //store settings
        public int storeOpen;
        public int storeClose;
        public int shutdownOpen;
        public int shutdownClose;
        public bool storeShutDown;
        public int shipmentTime;
        public bool shipmentArrived;
        public List<String> weekdays = new List<string>();
        public List<bool> storeOpenDays = new List<bool>();
        //electricity costs
        //needing store policies


        //Transition
        public float tax;
        public float inflation;
        public List<string> items = new List<string>();
        public int jobAmount;
        public int mapSelection;
        public float leasePricePerSquareFoot;
        public float averageTemp;
        public float highTemp;//disable heaters?
        public float lowTemp;//disable coolers?
        public int coldestDayOfTheYear;
        public int HotestDayOfTheYear;
        public List<int> tileZones;
        public int difficulty;
        public float levelDifficulty;
        public float totalDifficulty;
        public int BuyoutStartDate;
        public float moneyWinAmount = 800000;
        public int tutorialLevel;
        public Goal.Goals goal;
        public float goalAmount;
        public string mapName;
        public float wageLevelIncrease;
        public float averageCustomerHourlyIncome;
        public string goalreward;
        public string goalDisc;

        public List<bool> boughtZones = new List<bool>();

        //speicals
        public List<string> unlockedSpecialItems = new List<string>();
        public List<string> unlockedSpecialSuppliers = new List<string>();
        public List<string> unlockedSpecialSupplierItems = new List<string>();

        //controller
        public float MerchandiceExpiredMoneyLost;
        public int itemsSoldTotal;
        public int itemsSoldDaily;
        public int itemsSoldPast;
        public float electricityCost;
        public float leaseDue;
        public int ownedTilesCount;
    }
    private class SaveGrid
    {
        public NewGrid.SaveObject[] tilemapObjectSaveObjectArray;
    }
    private class SaveBuildings
    {
        public Building.SaveObject[] buildingsArray;
    }
    private class SaveItems
    {
        public Item.SaveObject[] itemsArray;
    }
    private class SaveCustomers
    {
        public Customer2.SaveObject[] CustomersArray;
    }
    private class SaveCustomerPreferances { }
    private class SaveEmployees
    {
        public Employee2.SaveObject[] EmployeesArray;
    }
    private class SaveCompetitors
    {
        public Competitor.SaveObject[] CompetitorArray;
    }
    private class SaveUI
    {
        public bool autosave;
        //hiring
        public int hiringLevel;
        public float hiringWage;
        public int hiringOccupation;

        //orderManager
        public List<string> itemNames = new List<string>();
        public List<int> amounts = new List<int>();
        public List<float> multipliers = new List<float>();
        public List<string> manufacturers = new List<string>();

        //walls + entrances
        public List<bool> wallBools = new List<bool>();
        public List<int> wallTypes = new List<int>();

        //calander
        public List<int> calanderValues = new List<int>();
        public List<string> calanderNames = new List<string>();
        public List<string> calanderWeather = new List<string>();

        //loans
        public List<string> loanNames = new List<string>();
        public List<float> loanValues = new List<float>();

        //advertising
        public bool autoAdvert;
        public int advertItemSelected;
        public bool populationBought;
        public bool itemGrowthBought;
        public bool needBought;
        public float itemSlider;
        public float popSlider;
        public float needSlider;

        //misc
        public int thermoStatSetting;
        public float outsideTemp;
        public float insideTemp;
        public string storeName;
        public bool revertToHiredTasks;
        public string ifDoneStocking = "Do nothing";
        public string ifDoneCashiering = "Do nothing";
        public string ifDoneCleaning = "Do nothing";
        public string ifDoneBuilding = "Do nothing";
        public string ifDoneManaging = "Do nothing";
        public List<string> priorityTask = new List<string>();

        public bool storeCredit;
        public float customerMemberships;
        public float customerEntry;
        public string billpayments;
        public string employeePaychecks;

        //goals - 1
        public bool winCondition1;
        public int goalType1;
        public float goalAmount1;
        public string itemName1;
        public float progress1;
        public float timeRemaining1;
        public int rewardInt1;

        //goals - 2
        public bool winCondition2;
        public int goalType2;
        public float goalAmount2;
        public string itemName2;
        public float progress2;
        public float timeRemaining2;
        public int rewardInt2;

        //goals - 3
        public bool winCondition3;
        public int goalType3;
        public float goalAmount3;
        public string itemName3;
        public float progress3;
        public float timeRemaining3;
        public int rewardInt3;

        public List<string> shiftNames = new List<string>();
        public List<int> shiftStartTimes = new List<int>();
        public List<int> shiftEndTimes = new List<int>();
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Configs
    private void StartConfigs()
    {
        if (!Directory.Exists(CONFIG_FOLDER)) { Directory.CreateDirectory(CONFIG_FOLDER); SetConfigs(); }
        //Invoke("GetConfigs", 1);
        GetConfigs();
    }
    //-------------------------------------------------------------------------------------------------------Set
    private void SetConfigs()
    {
        SetStartingConfigs();
        print(Controller.Instance.specialYears.Count);
        Invoke("SetEventConfigs", 0.5f);
        SetGamePlayConfigs();
    }
    private void SetStartingConfigs()
    {
        StartingConfigs variables = new StartingConfigs
        {
            starting_Money_Bonus = 0,
            starting_Population_Bonus = 0,
            starting_populationGrowth_Bonus = 0,
            starting_taxRate_Bonus = 0,
            starting_year_Bonus = 0,

        };

        string jsonVariables = JsonUtility.ToJson(variables, true);

        File.WriteAllText(CONFIG_FOLDER + "/starting_Configs.text", jsonVariables);
    }
    private void SetEventConfigs()
    {
        EventConfigs variables = new EventConfigs
        {
            specialYears = Controller.Instance.specialYears,
            specialYearlyDays = Controller.Instance.specialYearlyDays,
            proceduralDays = Controller.Instance.proceduralDays,
            randomDays = Controller.Instance.randomDays,
        };

        string jsonVariables = JsonUtility.ToJson(variables, true);

        File.WriteAllText(CONFIG_FOLDER + "/events_Configs.text", jsonVariables);
    }
    private void SetGamePlayConfigs()
    {
        GamePlayConfigs variables = new GamePlayConfigs
        {

        };

        string jsonVariables = JsonUtility.ToJson(variables, true);

        File.WriteAllText(CONFIG_FOLDER + "/gameplay_Configs.text", jsonVariables);
    }
    //-------------------------------------------------------------------------------------------------------Get
    private void GetConfigs()
    {
        if (!TransitionController.Instance.loadGame) { GetStartingConfigs(); }
        GetEventConfigs();
        GetGamePlayConfigs();
    }
    private void GetStartingConfigs()
    {
        if (File.Exists(CONFIG_FOLDER + "/starting_Configs.text"))
        {
            string saveString = File.ReadAllText(CONFIG_FOLDER + "/starting_Configs.text");

            StartingConfigs variables = JsonUtility.FromJson<StartingConfigs>(saveString);

            TransitionController.Instance.cityPopulation += variables.starting_Population_Bonus;
            TransitionController.Instance.cityGrowth += variables.starting_populationGrowth_Bonus;
            TransitionController.Instance.tax += variables.starting_taxRate_Bonus;
            UIController.Instance.year += variables.starting_year_Bonus;
            Controller.Instance.MoneyValueChange(variables.starting_Money_Bonus, UtilsClass.GetMouseWorldPosition(), false, true);
        }
        else { Debug.Log("No variables found"); }
    }
    private void GetEventConfigs()
    {
        if (File.Exists(CONFIG_FOLDER + "/events_Configs.text"))
        {
            string saveString = File.ReadAllText(CONFIG_FOLDER + "/events_Configs.text");

            EventConfigs variables = JsonUtility.FromJson<EventConfigs>(saveString);

            foreach (KeyValuePair<string, int> pair in variables.specialYears) { if (!Controller.Instance.specialYears.ContainsKey(pair.Key)) { Controller.Instance.specialYears.Add(pair.Key, pair.Value); } }
            foreach (KeyValuePair<string, int> pair in variables.specialYearlyDays) { if (!Controller.Instance.specialYearlyDays.ContainsKey(pair.Key)) { Controller.Instance.specialYearlyDays.Add(pair.Key, pair.Value); } }
            foreach (KeyValuePair<string, float> pair in variables.proceduralDays) { if (!Controller.Instance.proceduralDays.ContainsKey(pair.Key)) { Controller.Instance.proceduralDays.Add(pair.Key, pair.Value); } }
            foreach (KeyValuePair<string, float> pair in variables.randomDays) { if (!Controller.Instance.randomDays.ContainsKey(pair.Key)) { Controller.Instance.randomDays.Add(pair.Key, pair.Value); } }
        }
        else { Debug.Log("No variables found"); }
    }
    private void GetGamePlayConfigs()
    {
        if (File.Exists(CONFIG_FOLDER + "/gameplay_Configs.text"))
        {
            string saveString = File.ReadAllText(CONFIG_FOLDER + "/gameplay_Configs.text");

            GamePlayConfigs variables = JsonUtility.FromJson<GamePlayConfigs>(saveString);

            //
        }
        else { Debug.Log("No variables found"); }
    }
    //-------------------------------------------------------------------------------------------------------Classes
    private class StartingConfigs
    {
        public int starting_Money_Bonus;
        public int starting_Population_Bonus;
        public float starting_populationGrowth_Bonus;
        public float starting_taxRate_Bonus;
        public int starting_year_Bonus;
        public List<string> names;
    }
    private class EventConfigs
    {
        public Dictionary<string, int> specialYears = new Dictionary<string, int>();
        public Dictionary<string, int> specialYearlyDays = new Dictionary<string, int>();
        public Dictionary<string, float> proceduralDays = new Dictionary<string, float>();
        public Dictionary<string, float> randomDays = new Dictionary<string, float>();
    }
    private class GamePlayConfigs
    {

    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Mods
    [SerializeField] private Transform spawnParent;
    [SerializeField] private ScriptableObject itemSO;
    [SerializeField] private ScriptableObject eventSO;
    [SerializeField] private ScriptableObject buildingSO;

    [SerializeField] private List<ItemSO> exportingItems = new List<ItemSO>();
    [SerializeField] private List<EventSO> exportingEvents = new List<EventSO>();
    [SerializeField] private List<BuildingSO> exportingBuildings = new List<BuildingSO>();
    [SerializeField] private List<Sprite> exportingTiles = new List<Sprite>();

    [SerializeField] public List<ItemSO> importedItems = new List<ItemSO>();
    [SerializeField] private List<EventSO> importedEvents = new List<EventSO>();
    [SerializeField] private List<BuildingSO> importingBuildings = new List<BuildingSO>();
    [SerializeField] private List<Sprite> importedTiles = new List<Sprite>();
    [SerializeField] private Names nameCon; 
    [SerializeField] private RuleTile ruleTile;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private GameObject buildingVisualPrefab;

    private void StartMods()
    {
        SetAppearanceStrings();
        if (!Directory.Exists(ITEMS_FOLDER)) 
        { 
            Directory.CreateDirectory(MODS_FOLDER); 
            Directory.CreateDirectory(ITEMS_FOLDER);
            Directory.CreateDirectory(BUILDINGS_FOLDER);
            Directory.CreateDirectory(BUILDINGS_SPRITES_FOLDER);
            Directory.CreateDirectory(TILES_FOLDER);
            Directory.CreateDirectory(SPRITES_FOLDER);
            Directory.CreateDirectory(GAMEPLAY_FOLDER);
            Directory.CreateDirectory(EVENTS_FOLDER);
            Directory.CreateDirectory(LOANS_FOLDER);
            Directory.CreateDirectory(CHARACTER_VISUALS_FOLDER);

            ExportBaseMods(); 
        }

        GetAllModes();
    }
    //-------------------------------------------------------------------------------------------------------Set
    private void ExportBaseMods()
    {
        ExportController();
        ExportNames();
        ExportAppreances();
        ExportEvents();
        ExportItems();
        ExportBuildings();
        ExportFloors();
        ExportLoans();
    }
    private void ExportController()
    {
        ConVariables variables = new ConVariables
        {
            slowSpeed = TickSystem.Instance.slowSpeed,
            mediumSpeed = TickSystem.Instance.mediumSpeed,
            fastSpeed = TickSystem.Instance.fastSpeed,
            minimum_Hire_Wage = UIController.Instance.hiringWageSlider.minValue / 4f,
            maximum_Hire_Wage = UIController.Instance.hiringWageSlider.maxValue / 4f,
            minimum_Advertising_Multiplier = -5,
            maximum_Advertising_Multiplier = 10,
            minimum_Item_SellPrice_Multiplier = Controller.Instance.itemMarkupMin,
            maximum_Item_SellPrice_Multiplier = Controller.Instance.itemMarkupMax,

            maxLevel = Controller.Instance.maxLevel,
            base_employee_walk_speed = Controller.Instance.baseWalkSpeed,
            base_employee_work_speed = Controller.Instance.baseWorkSpeed,
            base_audio_effect_employee = Controller.Instance.baseAudio,
            base_employee_learning_speed = Controller.Instance.baseLearning,
            base_employee_callout_chance = Controller.Instance.baseCalloutChance,
            base_employee_stress_release_rate = Controller.Instance.baseStressRelease,
            base_employee_stress_accumulate_rate = Controller.Instance.baseStressAccumulate,
            base_employee_social_skills = Controller.Instance.baseSocial,
            base_employee_loaylty = Controller.Instance.baseLoaylty,

            base_customer_walk_speed = Controller.Instance.c_baseWalkSpeed,
            base_customer_work_speed = Controller.Instance.c_baseWorkSpeed,
            base_audio_effect_customer = Controller.Instance.c_baseAudio,
            base_customer_social_skills = Controller.Instance.c_baseSocial,
            base_customer_loyalty = Controller.Instance.c_baseLoaylty,
            base_customer_neediness = Controller.Instance.c_needy,
            base_customer_income = Controller.Instance.c_money,
            base_customer_greed = Controller.Instance.c_greed,
            base_customer_store_timer = Controller.Instance.c_inStoreTime,
            max_Customer_Count = Controller.Instance.max_Customer_Count,
        };
        string jsonString = JsonUtility.ToJson(variables, true);
        File.WriteAllText(GAMEPLAY_FOLDER + "/" + "Controller_Settings.text", jsonString);
    }
    private void ExportNames()
    {
        AllNames names = new AllNames
        {
            english_Male_Names = nameCon.english,
            spanish_Male_Names = nameCon.spanish,
            russian_Male_Names = nameCon.russian,
            german_Male_Names = nameCon.german,
            chinese_Male_Names = nameCon.chinese,

            french_Male_Names = nameCon.french,
            japanese_Male_Names = nameCon.japanese,
            korean_Male_Names = nameCon.korean,
            polish_Male_Names = nameCon.polish,
            brazil_Male_Names = nameCon.brazil,
            turkish_Male_Names = nameCon.turkish,
            urkranian_Male_Names = nameCon.urkranian,


            english_Female_Names = nameCon.english_Female,
            spanish_Female_Names = nameCon.spanish_Female,
            russian_Female_Names = nameCon.russian_Female,
            german_Female_Names = nameCon.german_Female,
            chinese_Female_Names = nameCon.chinese_Female,

            french_Female_Names = nameCon.french_Female,
            japanese_Female_Names = nameCon.japanese_Female,
            korean_Female_Names = nameCon.korean_Female,
            polish_Female_Names = nameCon.polish_Female,
            brazil_Female_Names = nameCon.brazil_Female,
            turkish_Female_Names = nameCon.turkish_Female,
            urkranian_Female_Names = nameCon.urkranian_Female,
        };
        string jsonString = JsonUtility.ToJson(names, true);
        File.WriteAllText(GAMEPLAY_FOLDER + "/" + "Names.text", jsonString);
    }
    private void ExportEvents()
    {
        foreach (EventSO events in exportingEvents)
        {
            SOEvent so = new SOEvent
            {
                myName = events.myName,
                dayType = events.dayType.ToString(),
                eventType = events.eventType.ToString(),

                yearActivation_if_year = events.yearActivation_if_year,
                dayActivation_if_holiday = events.dayActivation_if_holiday,
                chanceOfHappeing_if_random = events.chanceOfHappeing_if_random,
                equinox_if_weather = events.equinox_if_weather,

                positiveEffect = events.positiveEffect,
                worldStress = events.worldStress,
                itemsEffected = events.itemsEffected,
                itemsEffectAmount = events.itemsEffectAmount,
                temperatureEffect = events.temperatureEffect,

                powerOutage = events.powerOutage,
                noShipments = events.noShipments,
                cameraEffect = events.cameraEffect,

                alert_message = events.message,
                global_Callout_Chance_Multiplier = events.globalCalloutChanceMultiplier,
                global_Loyalty_Multiplier = events.globalLoyaltyMultiplier,
                competitor_Bailouts = events.competitorBailouts,
                global_Employee_Stress_Multiplier = events.globalEmployeeStressMultiplier,
                global_Item_Demand_Swing_Multiplier = events.globalItemDemandSwingMultiplier,
                global_Customer_Wage_Multiplier = events.globalCustomerWageMultiplier,
                global_Free_Money_For_Competitors = events.competitorFreeMoney,
            };

            string jsonVariables = JsonUtility.ToJson(so, true);

            File.WriteAllText(EVENTS_FOLDER + "/" + events.myName + ".text", jsonVariables);
        }
    }
    private void ExportItems()
    {
        foreach(ItemSO item in exportingItems)
        {
            SOItem so = new SOItem
            {
                myName = item.myName,

                //prefab = item.prefab,
                //visual = item.visual,
                spriteName = item.myName,

                value = item.value,
                cost = item.cost,
                //itemID = item.itemID,
                size = item.size,
                defaultNeedGrowth = item.defaultNeedGrowth,
                LifeSpan = item.lifeSpan,
                itemType = item.itemType,
                year_Start = item.year_Start,
                year_End = item.year_End,

                special = item.special,
                seasonal = item.seasonal,
                seasons = item.seasons,
                in_Demand_Season = item.inDemandSeason,
                itemOffset = item.itemOffset,
            };

            string jsonVariables = JsonUtility.ToJson(so, true);

            File.WriteAllText(ITEMS_FOLDER + "/"+item.myName+".text", jsonVariables);

            // Get the texture from the sprite
            Texture2D texture = item.sprite.texture;

            // Convert the texture to a PNG byte array
            byte[] pngData = texture.EncodeToPNG();

            // Write the PNG byte array to a file
            File.WriteAllBytes(SPRITES_FOLDER + "/" + so.spriteName + ".png", pngData);
        }
    }
    public void ExportLoans()
    {
        LoanVariables loanVars1 = new LoanVariables
        {
            loanName = "Daddy's Money",
            loanAmount = 1000,
            interest = 5,
        };
        LoanVariables loanVars2 = new LoanVariables
        {
            loanName = "Personal Loan",
            loanAmount = 5000,
            interest = 15,
        };
        LoanVariables loanVars3 = new LoanVariables
        {
            loanName = "Business Loan",
            loanAmount = 10000,
            interest = 10,
        };
        LoanVariables loanVars4 = new LoanVariables
        {
            loanName = "A Mortgage",
            loanAmount = 20000,
            interest = 20,
        };

        string jsonVariables1 = JsonUtility.ToJson(loanVars1, true);
        string jsonVariables2 = JsonUtility.ToJson(loanVars2, true);
        string jsonVariables3 = JsonUtility.ToJson(loanVars3, true);
        string jsonVariables4 = JsonUtility.ToJson(loanVars4, true);

        File.WriteAllText(LOANS_FOLDER + "/" + "Loan_1" + ".text", jsonVariables1);
        File.WriteAllText(LOANS_FOLDER + "/" + "Loan_2" + ".text", jsonVariables2);
        File.WriteAllText(LOANS_FOLDER + "/" + "Loan_3" + ".text", jsonVariables3);
        File.WriteAllText(LOANS_FOLDER + "/" + "Loan_4" + ".text", jsonVariables4);
    }
    private string CV_HAIRS;
    private string CV_HAIRS_MALE;
    private string CVHM_F;
    private string CVHM_R;
    private string CVHM_L;
    private string CVHM_B;
    private string CV_HAIRS_FEMALE;
    private string CVHF_F;
    private string CVHF_R;
    private string CVHF_L;
    private string CVHF_B;
    private string CV_HAIRS_BEARDS;
    private string CVHB_F;
    private string CVHB_R;
    private string CVHB_L;
    private string CV_OUTFITS;
    private string CV_OUTFITS_JOB;
    private string CVOJ_F;
    private string CVOJ_R;
    private string CVOJ_L;
    private string CVOJ_B;
    private string CV_OUTFITS_CIV;
    private string CVOC_F;
    private string CVOC_R;
    private string CVOC_L;
    private string CVOC_B;
    private void SetAppearanceStrings()
    {
        CV_HAIRS = CHARACTER_VISUALS_FOLDER + "Hair/";
        CV_HAIRS_MALE = CV_HAIRS + "Male/";
        CVHM_F = CV_HAIRS_MALE + "Front/";
        CVHM_R = CV_HAIRS_MALE + "/Right/";
        CVHM_L = CV_HAIRS_MALE + "/Left/";
        CVHM_B = CV_HAIRS_MALE + "/Back/";
        CV_HAIRS_FEMALE = CV_HAIRS + "/Female/";
        CVHF_F = CV_HAIRS_FEMALE + "/Front/";
        CVHF_R = CV_HAIRS_FEMALE + "/Right/";
        CVHF_L = CV_HAIRS_FEMALE + "/Left/";
        CVHF_B = CV_HAIRS_FEMALE + "/Back/";
        CV_HAIRS_BEARDS = CV_HAIRS_MALE + "/Beards/";
        CVHB_F = CV_HAIRS_BEARDS + "/Front/";
        CVHB_R = CV_HAIRS_BEARDS + "/Right/";
        CVHB_L = CV_HAIRS_BEARDS + "/Left/";
        CV_OUTFITS = CHARACTER_VISUALS_FOLDER + "/Outfits/";
        CV_OUTFITS_JOB = CV_OUTFITS + "/Jobs/";
        CVOJ_F = CV_OUTFITS_JOB + "/Front/";
        CVOJ_R = CV_OUTFITS_JOB + "/Right/";
        CVOJ_L = CV_OUTFITS_JOB + "/Left/";
        CVOJ_B = CV_OUTFITS_JOB + "/Back/";
        CV_OUTFITS_CIV = CV_OUTFITS + "/Civilians/";
        CVOC_F = CV_OUTFITS_CIV + "/Front/";
        CVOC_R = CV_OUTFITS_CIV + "/Right/";
        CVOC_L = CV_OUTFITS_CIV + "/Left/";
        CVOC_B = CV_OUTFITS_CIV + "/Back/";
    }
    private void ExportAppreances()
    {
        Directory.CreateDirectory(CV_HAIRS);
        Directory.CreateDirectory(CV_HAIRS_MALE);
        Directory.CreateDirectory(CVHM_F);
        Directory.CreateDirectory(CVHM_R);
        Directory.CreateDirectory(CVHM_L);
        Directory.CreateDirectory(CVHM_B);
        Directory.CreateDirectory(CV_HAIRS_FEMALE);
        Directory.CreateDirectory(CVHF_F);
        Directory.CreateDirectory(CVHF_R);
        Directory.CreateDirectory(CVHF_L);
        Directory.CreateDirectory(CVHF_B);
        Directory.CreateDirectory(CV_HAIRS_BEARDS);
        Directory.CreateDirectory(CVHB_F);
        Directory.CreateDirectory(CVHB_R);
        Directory.CreateDirectory(CVHB_L);
        Directory.CreateDirectory(CV_OUTFITS);
        Directory.CreateDirectory(CV_OUTFITS_JOB);
        Directory.CreateDirectory(CVOJ_F);
        Directory.CreateDirectory(CVOJ_R);
        Directory.CreateDirectory(CVOJ_L);
        Directory.CreateDirectory(CVOJ_B);
        Directory.CreateDirectory(CV_OUTFITS_CIV);
        Directory.CreateDirectory(CVOC_F);
        Directory.CreateDirectory(CVOC_R);
        Directory.CreateDirectory(CVOC_L);
        Directory.CreateDirectory(CVOC_B);

        for (int x = 0; x < CharacterVisualCon.Instance.f_hairs_M.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.f_hairs_M[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHM_F + "/" + x + ".png", pngData);
        }
        for (int x = 0; x < CharacterVisualCon.Instance.r_hairs_M.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.r_hairs_M[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHM_R + "/" + x + ".png", pngData);
        }
        for (int x = 0; x < CharacterVisualCon.Instance.l_hairs_M.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.l_hairs_M[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHM_L + "/" + x + ".png", pngData);
        }
        for (int x = 0; x < CharacterVisualCon.Instance.b_hairs_M.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.b_hairs_M[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHM_B + "/" + x + ".png", pngData);
        }

        for (int x = 0; x < CharacterVisualCon.Instance.f_hairs_F.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.f_hairs_F[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHF_F + "/" + x + ".png", pngData);
        }
        for (int x = 0; x < CharacterVisualCon.Instance.r_hairs_F.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.r_hairs_F[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHF_R + "/" + x + ".png", pngData);
        }
        for (int x = 0; x < CharacterVisualCon.Instance.l_hairs_F.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.l_hairs_F[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHF_L + "/" + x + ".png", pngData);
        }
        for (int x = 0; x < CharacterVisualCon.Instance.b_hairs_F.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.b_hairs_F[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(CVHF_B + "/" + x + ".png", pngData);
        }

        for (int x = 0; x < CharacterVisualCon.Instance.f_beards.Count; x++)
        {
            if (CharacterVisualCon.Instance.f_beards[x] != null)
            {
                Sprite sprite = CharacterVisualCon.Instance.f_beards[x];
                Texture2D texture = sprite.texture;
                byte[] pngData = texture.EncodeToPNG();
                File.WriteAllBytes(CVHB_F + "/" + x + ".png", pngData);
            }
        }
        for (int x = 0; x < CharacterVisualCon.Instance.r_beards.Count; x++)
        {
            if (CharacterVisualCon.Instance.r_beards[x] != null)
            {
                Sprite sprite = CharacterVisualCon.Instance.r_beards[x];
                Texture2D texture = sprite.texture;
                byte[] pngData = texture.EncodeToPNG();
                File.WriteAllBytes(CVHB_R + "/" + x + ".png", pngData);
            }
        }
        for (int x = 0; x < CharacterVisualCon.Instance.l_beards.Count; x++)
        {
            if (CharacterVisualCon.Instance.l_beards[x] != null)
            {
                Sprite sprite = CharacterVisualCon.Instance.l_beards[x];
                Texture2D texture = sprite.texture;
                byte[] pngData = texture.EncodeToPNG();
                File.WriteAllBytes(CVHB_L + "/" + x + ".png", pngData);
            }
        }

        for (int x = 0; x < CharacterVisualCon.Instance.f_outfits.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.f_outfits[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();

            if (x < CharacterVisualCon.Instance.numberOfJobs + 3)
            {
                File.WriteAllBytes(CVOJ_F + "/" + x + ".png", pngData);
            }
            else
            {
                File.WriteAllBytes(CVOC_F + "/" + x + ".png", pngData);
            }
        }
        for (int x = 0; x < CharacterVisualCon.Instance.r_outfits.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.r_outfits[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();

            if (x < CharacterVisualCon.Instance.numberOfJobs + 3)
            {
                File.WriteAllBytes(CVOJ_R + "/" + x + ".png", pngData);
            }
            else
            {
                File.WriteAllBytes(CVOC_R + "/" + x + ".png", pngData);
            }


        }
        for (int x = 0; x < CharacterVisualCon.Instance.l_outfits.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.l_outfits[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();

            if (x < CharacterVisualCon.Instance.numberOfJobs + 3)
            {
                File.WriteAllBytes(CVOJ_L + "/" + x + ".png", pngData);
            }
            else
            {
                File.WriteAllBytes(CVOC_L + "/" + x + ".png", pngData);
            }
        }
        for (int x = 0; x < CharacterVisualCon.Instance.b_outfits.Count; x++)
        {
            Sprite sprite = CharacterVisualCon.Instance.b_outfits[x];
            Texture2D texture = sprite.texture;
            byte[] pngData = texture.EncodeToPNG();

            if (x < CharacterVisualCon.Instance.numberOfJobs + 3)
            {
                File.WriteAllBytes(CVOJ_B + "/" + x + ".png", pngData);
            }
            else
            {
                File.WriteAllBytes(CVOC_B + "/" + x + ".png", pngData);
            }
        }
    }
    private void ExportFloors()
    {

        /*
                 List<int> tileFloorValues = new List<int> { 1, 50, 0, 0, 10 };
        MapController.Instance.floorTypes.Add("Tile", tileFloorValues);
        List<int> fineFloorValues = new List<int> { 2, 100, 0, 0, 25 };
        MapController.Instance.floorTypes.Add("Fine Tile", fineFloorValues);
         */
        foreach (Sprite tileImage in exportingTiles)
        {
            List<int> tileFloorValues = new List<int> { 1, 50, 0, 0, 10 };
            if (tileImage.name == "Fine Tile") { tileFloorValues = new List<int> { 2, 100, 0, 0, 25 }; }

            FloorVariables floorVar = new FloorVariables
            {
                tileName = tileImage.name,
                speed = tileFloorValues[0],
                beauty = tileFloorValues[1],
                cleanSpeed = tileFloorValues[2],
                cost = tileFloorValues[4],
            };

            string jsonVariables = JsonUtility.ToJson(floorVar, true);
            File.WriteAllText(TILES_FOLDER + "/" + tileImage.name + ".text", jsonVariables);

            Texture2D texture = tileImage.texture;
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(TILES_FOLDER + "/" + tileImage.name + ".png", pngData);
        }
    }
    private void ExportBuildings()
    {
        foreach (BuildingSO building in exportingBuildings)
        {
            BuildingValues so = new BuildingValues
            {
                myName = building.buildingName,
                width = building.width,
                height = building.height,
                cost = building.cost,
                myDiscription = building.myDiscription,
                type = building.type.ToString(),
                capacity = building.itemCapacity,
                electricityCost = building.electricityCost,

                OnCeiling = building.OnCeiling,

                lightValue = building.lightValue,
                lightFullRangeValue = building.lightFullRangeValue,
                lightTotalRange = building.lightTotalRange,
                refrigeration = building.isRefrigeration,
                freshen = building.freshenAmount,
                can_Storage = building.storables,
                lifeDecayRate = building.lifeDecayRate,
                buildTime = building.buildTime,
                speedReducer = building.speedReducer,
                heatProduction = building.heatProduction,
                automatic = building.isAutomatic,
                yearIntroduced = building.yearIntroduced,
                beauty = building.beauty,

                baseColorType = building.baseColorType,
                mainColorType = building.mainColorType,

                employeePosition = building.employeePosition,
                customerPosition = building.customerPosition,
                itemHoverYPos = building.itemHoverYPos,
                customerArrowFlip = building.arrow2Flip,

                container_Width = building.container_Width,
                container_Height = building.container_Height,
                container_Type = building.container_Type,
                container_Sort_Type = building.container_Sort_Type,
                container_gridSize = building.container_gridSize,
                container_numRows = building.container_numRows,
                container_numCols = building.container_numCols,
                container_Position = building.container_Position,
            };

            string jsonVariables = JsonUtility.ToJson(so, true);

            File.WriteAllText(BUILDINGS_FOLDER + "/" + building.buildingName + ".text", jsonVariables);

            
            // Get the texture from the sprite
            Texture2D texture1 = building.baseSprite.texture;
            Texture2D texture2 = building.mainSprite.texture;
            Texture2D texture3 = building.fullSprite.texture;

            // Convert the texture to a PNG byte array
            byte[] pngData1 = texture1.EncodeToPNG();
            byte[] pngData2 = texture2.EncodeToPNG();
            byte[] pngData3 = texture3.EncodeToPNG();

            // Write the PNG byte array to a file
            File.WriteAllBytes(BUILDINGS_SPRITES_FOLDER + "/" + building.buildingName + "_base" + ".png", pngData1);
            File.WriteAllBytes(BUILDINGS_SPRITES_FOLDER + "/" + building.buildingName + "_main" + ".png", pngData2);
            File.WriteAllBytes(BUILDINGS_SPRITES_FOLDER + "/" + building.buildingName + "_full" + ".png", pngData3);
        }
    }

    //-------------------------------------------------------------------------------------------------------Get
    private void GetAllModes()
    {
        GetConVariables();
        GetNames();
        GetAppearances();
        GetEvents();
        GetItems();
        GetBuildings();
        GetFloors();
        StartCoroutine(GetLoans());
    }
    private void GetConVariables()
    {
        string saveString = File.ReadAllText(GAMEPLAY_FOLDER + "/" + "Controller_Settings.text");
        ConVariables conVar = JsonUtility.FromJson<ConVariables>(saveString);

        TickSystem.Instance.slowSpeed = conVar.slowSpeed;
        TickSystem.Instance.mediumSpeed = conVar.mediumSpeed;
        TickSystem.Instance.fastSpeed = conVar.fastSpeed;
        UIController.Instance.GetModdedSettings(conVar.minimum_Hire_Wage, conVar.maximum_Hire_Wage, conVar.minimum_Advertising_Multiplier, conVar.maximum_Advertising_Multiplier);
        Controller.Instance.itemMarkupMin = conVar.minimum_Item_SellPrice_Multiplier;
        Controller.Instance.itemMarkupMax = conVar.maximum_Item_SellPrice_Multiplier;

        Controller.Instance.maxLevel = conVar.maxLevel;
        Controller.Instance.baseWalkSpeed = conVar.base_employee_walk_speed;
        Controller.Instance.baseWorkSpeed = conVar.base_employee_work_speed;
        Controller.Instance.baseAudio = conVar.base_audio_effect_employee;
        Controller.Instance.baseLearning = conVar.base_employee_learning_speed;
        Controller.Instance.baseCalloutChance = conVar.base_employee_callout_chance;
        Controller.Instance.baseStressRelease = conVar.base_employee_stress_release_rate;
        Controller.Instance.baseStressAccumulate = conVar.base_employee_stress_accumulate_rate;
        Controller.Instance.baseSocial = conVar.base_employee_social_skills;
        Controller.Instance.baseLoaylty = conVar.base_employee_loaylty;

        Controller.Instance.c_baseWalkSpeed = conVar.base_customer_walk_speed;
        Controller.Instance.c_baseWorkSpeed = conVar.base_customer_work_speed;
        Controller.Instance.c_baseAudio = conVar.base_audio_effect_customer;
        Controller.Instance.c_baseSocial = conVar.base_customer_social_skills;
        Controller.Instance.c_baseLoaylty = conVar.base_customer_loyalty;
        Controller.Instance.c_needy = conVar.base_customer_neediness;
        Controller.Instance.c_money = conVar.base_customer_income;
        Controller.Instance.c_greed = conVar.base_customer_greed;
        Controller.Instance.c_inStoreTime = conVar.base_customer_store_timer;

        Controller.Instance.max_Customer_Count = conVar.max_Customer_Count;
    }
    private void GetNames()
    {
        string saveString = File.ReadAllText(GAMEPLAY_FOLDER + "/" + "Names.text");
        AllNames names = JsonUtility.FromJson<AllNames>(saveString);

        nameCon.english = names.english_Male_Names;
        nameCon.spanish = names.spanish_Male_Names;
        nameCon.russian = names.russian_Male_Names;
        nameCon.german = names.german_Male_Names;
        nameCon.chinese = names.chinese_Male_Names;

        nameCon.french = names.french_Male_Names;
        nameCon.japanese = names.japanese_Male_Names;
        nameCon.korean = names.korean_Male_Names;
        nameCon.polish = names.polish_Male_Names;
        nameCon.brazil = names.brazil_Male_Names;
        nameCon.turkish = names.turkish_Male_Names;
        nameCon.urkranian = names.urkranian_Male_Names;


        nameCon.english_Female = names.english_Female_Names;
        nameCon.spanish_Female = names.spanish_Female_Names;
        nameCon.russian_Female = names.russian_Female_Names;
        nameCon.german_Female = names.german_Female_Names;
        nameCon.chinese_Female = names.chinese_Female_Names;

        nameCon.french_Female = names.french_Female_Names;
        nameCon.japanese_Female = names.japanese_Female_Names;
        nameCon.korean_Female = names.korean_Female_Names;
        nameCon.polish_Female = names.polish_Female_Names;
        nameCon.brazil_Female = names.brazil_Female_Names;
        nameCon.turkish_Female = names.turkish_Female_Names;
        nameCon.urkranian_Female = names.urkranian_Female_Names;

        nameCon.StartUp();
    }
    private void GetEvents()
    {
        Controller.Instance.allEvents.Clear();
        DirectoryInfo directoryInfo = new DirectoryInfo(EVENTS_FOLDER);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "text");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string saveString = File.ReadAllText(modFiles[i].FullName);
            //Debug.Log(modFiles.Length + saveString);
            SOEvent so = JsonUtility.FromJson<SOEvent>(saveString);

            EventSO newEvent = EventSO.CreateInstance<EventSO>();

            newEvent.myName = so.myName;
            newEvent.dayType = (Controller.DayType)Enum.Parse(typeof(Controller.DayType), so.dayType);
            newEvent.eventType = (EventSO.EventType)Enum.Parse(typeof(EventSO.EventType), so.eventType);

            newEvent.yearActivation_if_year = so.yearActivation_if_year;
            newEvent.dayActivation_if_holiday = so.dayActivation_if_holiday;
            newEvent.chanceOfHappeing_if_random = so.chanceOfHappeing_if_random;
            newEvent.equinox_if_weather = so.equinox_if_weather;

            newEvent.positiveEffect = so.positiveEffect;
            newEvent.worldStress = so.worldStress;
            newEvent.itemsEffected = so.itemsEffected;
            newEvent.itemsEffectAmount = so.itemsEffectAmount;
            newEvent.temperatureEffect = so.temperatureEffect;

            newEvent.powerOutage = so.powerOutage;
            newEvent.noShipments = so.noShipments;
            newEvent.cameraEffect = so.cameraEffect;

            newEvent.message = so.alert_message;
            newEvent.globalCalloutChanceMultiplier = so.global_Callout_Chance_Multiplier;
            newEvent.globalLoyaltyMultiplier = so.global_Loyalty_Multiplier;
            newEvent.competitorBailouts = so.competitor_Bailouts;
            newEvent.globalEmployeeStressMultiplier = so.global_Employee_Stress_Multiplier;
            newEvent.globalItemDemandSwingMultiplier = so.global_Item_Demand_Swing_Multiplier;
            newEvent.globalCustomerWageMultiplier = so.global_Customer_Wage_Multiplier;
            newEvent.competitorFreeMoney = so.global_Free_Money_For_Competitors;

            importedEvents.Add(newEvent);
            Controller.Instance.allEvents.Add(newEvent);
        }
    }
    private void GetItems()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(ITEMS_FOLDER);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "text");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string saveString = File.ReadAllText(modFiles[i].FullName);
            //Debug.Log(modFiles.Length + saveString);
            SOItem so = JsonUtility.FromJson<SOItem>(saveString);

            ItemSO newItem = ItemSO.CreateInstance<ItemSO>();

            newItem.myName = so.myName;
            newItem.baseValue = so.value;
            newItem.value = so.value;
            newItem.cost = so.cost;
            newItem.size = so.size;
            newItem.lifeSpan = so.LifeSpan;
            newItem.itemType = so.itemType;
            newItem.year_Start = so.year_Start;
            newItem.year_End = so.year_End;
            newItem.special = so.special;
            newItem.seasonal = so.seasonal;
            newItem.seasons = so.seasons;

            newItem.prefab = itemSOBase.prefab;
            newItem.visual = itemSOBase.visual;
            newItem.itemID = i;
            newItem.inDemandSeason = so.in_Demand_Season;
            newItem.itemOffset = so.itemOffset;
            if (File.Exists(SPRITES_FOLDER + "/" + so.spriteName + ".png"))
            {
                string imagePath = SPRITES_FOLDER + "/" + so.spriteName + ".png";
                byte[] imageData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                newItem.sprite = sprite;
            }
            else { print("File did not exist"); }
            newItem.defaultNeedGrowth = so.defaultNeedGrowth;

            importedItems.Add(newItem);
        }


        //Controller.Instance.items = importedItems;
    }
    private IEnumerator GetLoans()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(LOANS_FOLDER);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "text");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string saveString = File.ReadAllText(modFiles[i].FullName);
            LoanVariables laonVars = JsonUtility.FromJson<LoanVariables>(saveString);
            UIController.Instance.SpawnLoan(laonVars.loanAmount, laonVars.interest, laonVars.loanName);
            yield return new WaitForEndOfFrame();
        }

        StartDelay();
    }
    private void GetAppearances()
    {
        CharacterVisualCon.Instance.f_hairs_M.Clear();
        DirectoryInfo directoryInfo = new DirectoryInfo(CVHM_F);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.f_hairs_M.Add(sprite);
        }

        CharacterVisualCon.Instance.r_hairs_M.Clear();
        directoryInfo = new DirectoryInfo(CVHM_R);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.r_hairs_M.Add(sprite);
        }

        CharacterVisualCon.Instance.l_hairs_M.Clear();
        directoryInfo = new DirectoryInfo(CVHM_L);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.l_hairs_M.Add(sprite);
        }

        CharacterVisualCon.Instance.b_hairs_M.Clear();
        directoryInfo = new DirectoryInfo(CVHM_B);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.b_hairs_M.Add(sprite);
        }


        CharacterVisualCon.Instance.f_hairs_F.Clear();
        directoryInfo = new DirectoryInfo(CVHF_F);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.f_hairs_F.Add(sprite);
        }

        CharacterVisualCon.Instance.r_hairs_F.Clear();
        directoryInfo = new DirectoryInfo(CVHF_R);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.r_hairs_F.Add(sprite);
        }

        CharacterVisualCon.Instance.l_hairs_F.Clear();
        directoryInfo = new DirectoryInfo(CVHF_L);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.l_hairs_F.Add(sprite);
        }

        CharacterVisualCon.Instance.b_hairs_F.Clear();
        directoryInfo = new DirectoryInfo(CVHF_B);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.b_hairs_F.Add(sprite);
        }


        CharacterVisualCon.Instance.f_beards.Clear();
        CharacterVisualCon.Instance.f_beards.Add(null);
        directoryInfo = new DirectoryInfo(CVHB_F);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.f_beards.Add(sprite);
        }

        CharacterVisualCon.Instance.r_beards.Clear();
        CharacterVisualCon.Instance.r_beards.Add(null);
        directoryInfo = new DirectoryInfo(CVHB_R);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.r_beards.Add(sprite);
        }

        CharacterVisualCon.Instance.l_beards.Clear();
        CharacterVisualCon.Instance.l_beards.Add(null);
        directoryInfo = new DirectoryInfo(CVHB_L);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.l_beards.Add(sprite);
        }

        //---------------

        CharacterVisualCon.Instance.f_outfits.Clear();
        directoryInfo = new DirectoryInfo(CVOJ_F);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.f_outfits.Add(sprite);
        }

        CharacterVisualCon.Instance.r_outfits.Clear();
        directoryInfo = new DirectoryInfo(CVOJ_R);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.r_outfits.Add(sprite);
        }

        CharacterVisualCon.Instance.l_outfits.Clear();
        directoryInfo = new DirectoryInfo(CVOJ_L);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.l_outfits.Add(sprite);
        }

        CharacterVisualCon.Instance.b_outfits.Clear();
        directoryInfo = new DirectoryInfo(CVOJ_B);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.b_outfits.Add(sprite);
        }


        directoryInfo = new DirectoryInfo(CVOC_F);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.f_outfits.Add(sprite);
        }


        directoryInfo = new DirectoryInfo(CVOC_R);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.r_outfits.Add(sprite);
        }


        directoryInfo = new DirectoryInfo(CVOC_L);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.l_outfits.Add(sprite);
        }

 
        directoryInfo = new DirectoryInfo(CVOC_B);
        modFiles = directoryInfo.GetFiles("*." + "png");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string imagePath = modFiles[i].FullName;
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);//does this size work?
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            CharacterVisualCon.Instance.b_outfits.Add(sprite);
        }

    }
    private void GetFloors()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(TILES_FOLDER);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "text");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string saveString = File.ReadAllText(modFiles[i].FullName);
            FloorVariables so = JsonUtility.FromJson<FloorVariables>(saveString);
            List<int> newFloorValues = new List<int> { so.speed, so.beauty, so.cleanSpeed, 0, so.cost };
            MapController.Instance.floorTypes.Add(so.tileName, newFloorValues);

            string imagePath = TILES_FOLDER + "/" + so.tileName + ".png";
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            bool found = false;
            foreach (RuleTile tiler in MapController.Instance.tiles) { if (so.tileName == tiler.name) { found = true; } }
            if (!found)
            {
                //create tilerule
                RuleTile newTile = RuleTile.CreateInstance<RuleTile>();
                ruleTile.m_DefaultSprite = sprite;
                ruleTile.name = so.tileName;
                MapController.Instance.tiles.Add(newTile);
            }

            //create build button
            UIController.Instance.SpawnTileBuildOption(sprite, so.tileName);
        }
    }

    private void GetBuildings()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(BUILDINGS_FOLDER);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "text");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string saveString = File.ReadAllText(modFiles[i].FullName);
            BuildingValues so = JsonUtility.FromJson<BuildingValues>(saveString);

            BuildingSO newBuilding = BuildingSO.CreateInstance<BuildingSO>();

            newBuilding.buildingName = so.myName;
            newBuilding.width = so.width;
            newBuilding.height = so.height;
            newBuilding.cost = so.cost;
            newBuilding.myDiscription = so.myDiscription;
            newBuilding.type = (BuildingSO.Type)Enum.Parse(typeof(BuildingSO.Type), so.type);
            newBuilding.itemCapacity = so.capacity;
            newBuilding.electricityCost = so.electricityCost;

            newBuilding.OnCeiling = so.OnCeiling;

            newBuilding.lightValue = so.lightValue;
            newBuilding.lightFullRangeValue = so.lightFullRangeValue;
            newBuilding.lightTotalRange = so.lightTotalRange;
            newBuilding.isRefrigeration = so.refrigeration;
            newBuilding.freshenAmount = so.freshen;
            newBuilding.storables = so.can_Storage;
            newBuilding.lifeDecayRate = so.lifeDecayRate;
            newBuilding.buildTime = so.buildTime;
            newBuilding.speedReducer = so.speedReducer;
            newBuilding.heatProduction = so.heatProduction;
            newBuilding.isAutomatic = so.automatic;
            newBuilding.yearIntroduced = so.yearIntroduced;
            newBuilding.beauty = so.beauty;

            newBuilding.baseColorType = so.baseColorType;
            newBuilding.mainColorType = so.mainColorType;

            newBuilding.employeePosition = so.employeePosition;
            newBuilding.customerPosition = so.customerPosition;
            newBuilding.itemHoverYPos = so.itemHoverYPos;
            newBuilding.arrow2Flip = so.customerArrowFlip;

            newBuilding.container_Width = so.container_Width;
            newBuilding.container_Height = so.container_Height;
            newBuilding.container_Type = so.container_Type;
            newBuilding.container_Sort_Type = so.container_Sort_Type;
            newBuilding.container_gridSize = so.container_gridSize;
            newBuilding.container_numRows = so.container_numRows;
            newBuilding.container_numCols = so.container_numCols;
            newBuilding.container_Position = so.container_Position;


    string imagePath1 = BUILDINGS_SPRITES_FOLDER + "/" + so.myName + "_base" + ".png";
            byte[] imageData1 = File.ReadAllBytes(imagePath1);
            Texture2D texture1 = new Texture2D(2, 2);
            texture1.LoadImage(imageData1);
            Sprite sprite1 = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), Vector2.zero);

            string imagePath2 = BUILDINGS_SPRITES_FOLDER + "/" + so.myName + "_main" + ".png";
            byte[] imageData2 = File.ReadAllBytes(imagePath2);
            Texture2D texture2 = new Texture2D(2, 2);
            texture2.LoadImage(imageData2);
            Sprite sprite2 = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), Vector2.zero);

            string imagePath3 = BUILDINGS_SPRITES_FOLDER + "/" + so.myName + "_full" + ".png";
            byte[] imageData3 = File.ReadAllBytes(imagePath3);
            Texture2D texture3 = new Texture2D(2, 2);
            texture3.LoadImage(imageData3);
            Sprite sprite3 = Sprite.Create(texture3, new Rect(0, 0, texture3.width, texture3.height), Vector2.zero);

            newBuilding.baseSprite = sprite1;
            newBuilding.mainSprite = sprite2;
            newBuilding.fullSprite = sprite3;

            newBuilding.prefab = buildingPrefab.transform;

            importingBuildings.Add(newBuilding);

            //create build button
            UIController.Instance.SpawnBuildOption(newBuilding);
        }
    }


    //-------------------------------------------------------------------------------------------------------Classes
    private class SOItem
    {
        public string myName;

        //public Transform prefab;//defualt
        //public Transform visual;//defualt
        //public Sprite sprite;//get from files
        public string spriteName;

        public float value;
        public float cost;
        //public int itemID;//autogenerated?

        public float size;
        public float defaultNeedGrowth;
        public int LifeSpan;
        public string itemType;
        public int year_Start;
        public int year_End;

        public bool special;
        public bool seasonal;
        public List<string> seasons = new List<string>();
        public string in_Demand_Season;
        public Vector2 itemOffset;
    }
    private class SOEvent
    {
        public string myName;
        public string dayType;
        public string eventType;
        public int yearActivation_if_year;
        public int dayActivation_if_holiday;
        public int chanceOfHappeing_if_random;
        public int equinox_if_weather;
        public bool positiveEffect;
        public float worldStress;
        public string itemsEffected;
        public float itemsEffectAmount;
        public float temperatureEffect;
        public bool powerOutage;
        public bool noShipments;
        public int cameraEffect;
        public string alert_message;
        public float global_Callout_Chance_Multiplier;
        public float global_Loyalty_Multiplier;
        public bool competitor_Bailouts;
        public float global_Employee_Stress_Multiplier;
        public float global_Item_Demand_Swing_Multiplier;
        public float global_Customer_Wage_Multiplier;
        public float global_Free_Money_For_Competitors;
    }

    private class AllNames
    {
        public List<string> english_Male_Names = new List<string>();
        public List<string> spanish_Male_Names = new List<string>();
        public List<string> russian_Male_Names = new List<string>();
        public List<string> german_Male_Names = new List<string>();
        public List<string> chinese_Male_Names = new List<string>();

        public List<string> french_Male_Names = new List<string>();
        public List<string> japanese_Male_Names = new List<string>();
        public List<string> korean_Male_Names = new List<string>();
        public List<string> polish_Male_Names = new List<string>();
        public List<string> brazil_Male_Names = new List<string>();
        public List<string> turkish_Male_Names = new List<string>();
        public List<string> urkranian_Male_Names = new List<string>();


        public List<string> english_Female_Names = new List<string>();
        public List<string> spanish_Female_Names = new List<string>();
        public List<string> russian_Female_Names = new List<string>();
        public List<string> german_Female_Names = new List<string>();
        public List<string> chinese_Female_Names = new List<string>();

        public List<string> french_Female_Names = new List<string>();
        public List<string> japanese_Female_Names = new List<string>();
        public List<string> korean_Female_Names = new List<string>();
        public List<string> polish_Female_Names = new List<string>();
        public List<string> brazil_Female_Names = new List<string>();
        public List<string> turkish_Female_Names = new List<string>();
        public List<string> urkranian_Female_Names = new List<string>();
    }
    private class ConVariables
    {
        public float slowSpeed;
        public float mediumSpeed;
        public float fastSpeed;

        public float minimum_Hire_Wage;
        public float maximum_Hire_Wage;
        public int minimum_Advertising_Multiplier;
        public int maximum_Advertising_Multiplier;
        public float minimum_Item_SellPrice_Multiplier;
        public float maximum_Item_SellPrice_Multiplier;

        public int maxLevel;
        public float base_employee_walk_speed;
        public float base_employee_work_speed;
        public float base_audio_effect_employee;
        public float base_employee_learning_speed;
        public float base_employee_callout_chance;
        public float base_employee_stress_release_rate;
        public float base_employee_stress_accumulate_rate;
        public float base_employee_social_skills;
        public float base_employee_loaylty;

        public float base_customer_walk_speed;
        public float base_customer_work_speed;
        public float base_audio_effect_customer;
        public float base_customer_social_skills;
        public float base_customer_loyalty;
        public float base_customer_neediness;
        public float base_customer_income;
        public float base_customer_greed;
        public int base_customer_store_timer;
        public int max_Customer_Count;
    }
    private class LoanVariables
    {
        public string loanName;
        public int loanAmount;
        public float interest;
    }
    private class FloorVariables
    {
        public string tileName;
        public int speed;
        public int beauty;
        public int cleanSpeed;
        public int cost;
    }
    private class BuildingValues
    {
        public string myName;
        public int width;
        public int height;
        public float cost;
        public string myDiscription;
        public string type;
        public int capacity;
        public float electricityCost;

        public bool OnCeiling;

        public int lightValue;
        public int lightFullRangeValue;
        public int lightTotalRange;
        public bool refrigeration;
        public float freshen;
        public List<string> can_Storage;
        public float lifeDecayRate;
        public int buildTime;
        public int speedReducer;
        public float heatProduction;
        public bool automatic;
        public int yearIntroduced;
        public int beauty;

        public string baseColorType;
        public string mainColorType;

        public Vector3 employeePosition;
        public Vector3 customerPosition;
        public float itemHoverYPos;
        public bool customerArrowFlip;

        public float container_Width;
        public float container_Height;
        public StockZone.StorageType container_Type;
        public StockZone.container container_Sort_Type;
        public float container_gridSize;
        public int container_numRows;
        public int container_numCols;
        public Vector3 container_Position;
    }
}
