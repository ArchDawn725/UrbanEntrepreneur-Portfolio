using ArchDawn.Utilities;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }

    public event EventHandler OnMoneyValueChanged;
    public event EventHandler FinishedLoading;
    public event EventHandler OrderItems;

    public List<Transform> startingPoints = new List<Transform>();
    public List<Wall> entrances = new List<Wall>();
    public float money;
    public Employee2 selectedEmployee;
    public Customer2 selectedCustomer;
    public Building selectedBuilding;
    public Wall selectedWall;

    public bool storeOpened;
    public int storeOpen;
    public int storeClose;
    public int shutdownOpen;
    public int shutdownClose;
    public bool storeShutDown;
    public int shipmentTime;
    public bool shipmentArrived;

    public List<Employee2> employees = new List<Employee2>();
    public List<Customer2> customers = new List<Customer2>();
    public List<Officer> officers = new List<Officer>();

    [SerializeField] public List<Building> buildings = new List<Building>();
    [SerializeField] public List<Building> shelves = new List<Building>();
    [SerializeField] public List<Building> registers = new List<Building>();
    [SerializeField] public List<Building> stockPiles = new List<Building>();

    public CustomerSO customerSO;

    [SerializeField] public List<Building> selectedBuildings = new List<Building>();
    public List<ItemSO> items = new List<ItemSO>();
    public List<ItemSO> unlockedSpecialItems = new List<ItemSO>();
    public List<ItemSO> removedItems = new List<ItemSO>();
    public Dictionary<string, int> itemsSold = new Dictionary<string, int>();
    public List<EventSO> allEvents = new List<EventSO>();
    public int itemsSoldTotal;
    public int itemsSoldDaily;
    public int itemsSoldPast;

    public float dailyMoneyLostWages;
    public float dailyMoneyLostProduct;
    private bool dayChangeTicker;

    public float electricityCost;

    public Dictionary<string, int> specialYears = new Dictionary<string, int>();
    public Dictionary<string, int> specialYearlyDays = new Dictionary<string, int>();
    public Dictionary<string, float> proceduralDays = new Dictionary<string, float>();
    public Dictionary<string, float> randomDays = new Dictionary<string, float>();
    public List<string> thisMonthWeather = new List<string>();

    public List<Loan> loans = new List<Loan>();

    [Space(10)]
    [Header("Debugs")]
    public EmployeeSO employeeSO;

    public int musicWorkBonus;//x
    public int musicSpeedBonus;//y
    public int musicStressBonus;//z
    private Vector3 musicBonuses;
    private Vector3 musicSpeedBonuses;
    private Vector3 musicVolumeBonuses;
    public int previousMusicChoice;
    public int setPitch;
    public int setVolume;
    [SerializeField] private List<AudioSource> music = new List<AudioSource>();
    [SerializeField] private GameObject competition;
    public List<Competitor> competitors = new List<Competitor>();
    public float worldEmployeeStressIncrease;
    public float MerchandiceExpiredMoneyLost;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider musicSpeedSlider;
    public bool customerStart;
    public int activeCustomers;

    public enum DayType
    {
        normal,
        holiday,
        weather,
        item_Effect,
    }
    public DayType dayType;
    public List<Wall> anyoneEntrances = new List<Wall>();
    public List<Wall> customerEntrances = new List<Wall>();
    public List<Wall> employeeEntrances = new List<Wall>();
    public float outsideTemp;
    public float insideTemp;
    public float insulation;
    public float heating;
    public int tempSet;

    public string billpayments;
    public string employeePaychecks;
    public float customerMemberships;
    public float customerEntry;
    public float leaseDue = 0;
    public string previousday;
    public float inflationAmount = 1;
    public List<float> requestedWages = new List<float>();
    public Dictionary<string, bool> storeOpenDays = new Dictionary<string, bool>();
    public string storeName = "My Store";
    [SerializeField] private GameObject officer;
    public Dictionary<string, string> unlockedSpecialManufactorers = new Dictionary<string, string>();
    public bool storeCredit;
    public float MoneyMadeByDebt;

    public bool shiftButtonDown;
    public bool ctrlButtonDown;
    Localizer localizer;
    private EventSO todaysEvent;

    [HideInInspector] public float itemMarkupMin = 0;
    [HideInInspector] public float itemMarkupMax = 3f;

    public bool revertToHiredTasks;
    public string ifDoneStocking = "Do nothing";
    public string ifDoneCashiering = "Do nothing";
    public string ifDoneCleaning = "Do nothing";
    public string ifDoneBuilding = "Do nothing";
    public string ifDoneManaging = "Do nothing";
    public List<string> priorityTask = new List<string>();

    public float musicVolume = 1;
    public float fxVolume = 1;
    public float talkVolume = 1;
    public float uiVolume = 1;

    private void Awake() { Instance = this; }
    private void Start()
    {
        localizer = Localizer.Instance;
        UIController.Instance.OnTimeValueChanged += TimeChange;
        UIController.Instance.OnDayValueChanged += DayChange;
        TickSystem.Instance.On10Tick += GetCustomerPreferance;

        TransitionController.Instance.GetStartingValues(out int m, out int x, out int y, out float cellSize);
        TransitionController.Instance.StartMapGoal();
        MapController.Instance.CreateGrid(x, y, cellSize);

        if (m > 0) { money = m * TransitionController.Instance.difficulty; }
        else { money = m; }
        
        insulation = 130 * TransitionController.Instance.totalDifficulty;

        GetSavedAudioSettings();
        //Invoke("StartUp", 10f);
    }

    public void StartUp()
    {
        if (!TransitionController.Instance.loadGame) { NoLoadGame(); }
        else { SaveController.Instance.LoadGame(); }
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
    }
    private void NoLoadGame()
    {
        OnMoneyValueChanged?.Invoke(this, EventArgs.Empty);
        for (int i = 0; i < TransitionController.Instance.numberOfCompetitors; i++) 
        { 
            GameObject comp = Instantiate(competition);
            competitors.Add(comp.GetComponent<Competitor>());
            UIController.Instance.SpawnMapCompetitor(comp.GetComponent<Competitor>());
            comp.GetComponent<Competitor>().BeforeDelay();
        }
        for (int i = 0; i < TransitionController.Instance.numberOfSpecialCompetitors; i++)
        {
            GameObject comp = Instantiate(competition);
            comp.GetComponent<Competitor>().special = true;
            competitors.Add(comp.GetComponent<Competitor>());
            UIController.Instance.SpawnMapCompetitor(comp.GetComponent<Competitor>());
            comp.GetComponent<Competitor>().BeforeDelay();
        }
        StartCoroutine(CityPopulationSpawn());
        ChangeMusic(Random.Range(0, 5));
        UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Customers will arrive in 24 hours!"), "Manager", 0);
        Finished_Loading();
    }
    public void LoadGame()
    {
        //SaveController.Instance.ToLoadCompetitors();
        UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Welcome back! Press play to continue!"), "Manager", 0);
        Finished_Loading();
        customerStart = true;
    }
    public Competitor CreateCompetitor()
    {
        GameObject comp = Instantiate(competition);
        competitors.Add(comp.GetComponent<Competitor>());
        return comp.GetComponent<Competitor>();
    }

    private void Finished_Loading()
    {
        FinishedLoading?.Invoke(this, EventArgs.Empty);
        GetAvailableItems(SaveController.Instance.importedItems);
        StartCoroutine(GenerateAvailableOrders());
        finishedLoading = true;
    }
    private bool finishedLoading;

    private void Update()
    {
        if (SteamClient.IsValid)
        {
            Steamworks.SteamClient.RunCallbacks();
        }

        if (Input.GetMouseButtonDown(0) && finishedLoading)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            DeselectHovers();

            Vector3 pos = UtilsClass.GetMouseWorldPosition();

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit == true)
            {
                if (hit.collider.gameObject.TryGetComponent(out Employee2 newSelectedEmployee) && !MapController.Instance.floorActive)
                {
                    if (selectedEmployee != null)
                    {
                        if (selectedEmployee.task == Employee2.Task.manager)
                        {
                            if (selectedEmployee.objective != Employee2.Objective.absent)
                            {
                                selectedEmployee.SwitchTask(null, "", newSelectedEmployee);
                                selectedEmployee.Deselected();
                                selectedEmployee = null;
                            }
                            //return;
                        }
                        else
                        {
                            selectedEmployee.Deselected();
                        }
                    }

                    selectedEmployee = newSelectedEmployee;

                    selectedEmployee.Selected();
                    UIController.Instance.NewSelectPopUp(selectedEmployee.gameObject);


                    //manager to employee relation?
                }

                if (hit.collider.gameObject.TryGetComponent(out Building newSelectedBuilding))
                {
                    if (MapController.Instance.placingBuilding == null && !MapController.Instance.floorActive)
                    {
                        if (selectedBuilding != null) { selectedBuilding.Deselected(); }
                        selectedBuilding = newSelectedBuilding;

                        if (selectedEmployee != null)
                        {
                            if (selectedEmployee.objective != Employee2.Objective.absent && !selectedBuilding.automatic)
                            {
                                selectedEmployee.SwitchTask(selectedBuilding, "", null);
                                selectedEmployee.Deselected();
                                selectedEmployee = null;
                                selectedBuilding.Deselected();
                                selectedBuilding = null;
                            }
                        }

                        if (selectedBuilding != null)
                        {
                            UIController.Instance.NewSelectPopUp(selectedBuilding.gameObject);
                            selectedBuilding.Selected();
                        }
                    }
                }

                if (MapController.Instance.placingBuilding == null && !MapController.Instance.floorActive)
                {
                    if (hit.collider.gameObject.TryGetComponent(out Customer2 newSelectedCustomer))
                    {
                        if (selectedCustomer != null) { selectedCustomer.Deselected(); }
                        selectedCustomer = newSelectedCustomer;

                        UIController.Instance.NewSelectPopUp(selectedCustomer.gameObject);
                        selectedCustomer.Selected();
                        //employee to customer?
                    }
                }


                if (hit.collider.gameObject.TryGetComponent(out Wall newSelectWall))
                {
                    if (MapController.Instance.placingBuilding == null)
                    {
                        if (selectedWall != null) { selectedWall.Deselected(); }
                        selectedWall = newSelectWall;

                        UIController.Instance.NewSelectPopUp(selectedWall.gameObject);
                        selectedWall.Selected();
                    }
                }
            }
            else
            {
                if (selectedEmployee != null) { selectedEmployee.GoHere(pos); }
                 else if (MapController.Instance.placingBuilding == null && !MapController.Instance.floorActive) { DeselectAll(); }
            }


        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (MapController.Instance.placingBuilding == null && !MapController.Instance.floorActive)
            {
                Vector3 pos = UtilsClass.GetMouseWorldPosition();
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                if (hit == true)
                {
                    if (hit.collider.gameObject.TryGetComponent(out Building anotherBuilding))
                    {
                        if (selectedBuildings.Count > 0)
                        {
                            if (!selectedBuildings.Contains(anotherBuilding) && anotherBuilding.type == selectedBuildings[0].type)
                            {
                                selectedBuildings.Add(anotherBuilding);
                                anotherBuilding.Selected();
                                UIController.Instance.NewMulipleSelectPopUp(anotherBuilding.gameObject);
                            }
                        }
                        else
                        {
                            selectedBuildings.Add(anotherBuilding);
                            anotherBuilding.Selected();
                            if (UIController.Instance.selectedBuilding != anotherBuilding)
                            {
                                UIController.Instance.NewSelectPopUp(anotherBuilding.gameObject);
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) { shiftButtonDown = true; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { shiftButtonDown = false; }
        if (Input.GetKeyDown(KeyCode.LeftControl)) { ctrlButtonDown = true; }
        if (Input.GetKeyUp(KeyCode.LeftControl)) { ctrlButtonDown = false; }
    }

    private void DeselectHovers()
    {
        foreach(Building building in selectedBuildings)
        {
            if (building != null) { building.Deselected(); }  
        }
        selectedBuildings.Clear();
    }

    public void DeselectAll()
    {
        UIController.Instance.NewSelectPopUp(null);
        if (selectedEmployee != null) { selectedEmployee.Deselected(); selectedEmployee = null; }
        if (selectedBuilding != null) { selectedBuilding.Deselected(); selectedBuilding = null; }
        if (selectedCustomer != null) { selectedCustomer.Deselected(); selectedCustomer = null; }
        foreach(Building build in selectedBuildings) { if (build != null) { build.Deselected(); } }
    }

    public void DeselectAll2()
    {
        if (selectedEmployee != null) { selectedEmployee.Deselected(); }
        if (selectedBuilding != null) { selectedBuilding.Deselected(); }
        if (selectedCustomer != null) { selectedCustomer.Deselected(); }
        foreach (Building build in selectedBuildings) { build.Deselected(); }
    }

    private void ClearSelected()
    {
        selectedEmployee = null;
        selectedBuilding = null;
        selectedCustomer = null;
    }

    public void MoneyValueChange(float value, Vector3 referance, bool natural, bool taxFree)
    {
        if (value < 0)
        {
            float newValue = value;
            if (!taxFree) { newValue = value * ((TransitionController.Instance.tax) + 1); }
            value = newValue;

            if (referance != null)
            {
                UtilsClass.CreateWorldTextPopup(value.ToString("f2") + "$", referance, Color.red);
            }
        }
        else if (value > 0)
        {
            float newValue = value;
            if (!taxFree) { newValue = (value * ((TransitionController.Instance.tax) + 1)); }
            newValue -= value;
            value = value - newValue;

            if (referance != null)
            {
                UtilsClass.CreateWorldTextPopup("+" + value.ToString("f2") + "$", referance, Color.green);
            }
        }

        if (natural)
        {
            if (value > 0) { UIController.Instance.MoneyGained += value; }
            else { UIController.Instance.MoneyLost -= value; }
        }

        money += value;
        OnMoneyValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool CompareStats()
    {
        selectedBuilding.GetStats(out int iReq, out int cReq, out int mReq);
        selectedEmployee.OutSkills(out int iSkill, out int cSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        if (iSkill >= iReq && cSkill >= cReq) return true;
        else return false;
    }

    private void TimeChange(object sender, System.EventArgs e)
    {

        int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
        if (time == shipmentTime && !shipmentArrived) { OrderItems?.Invoke(this, EventArgs.Empty); shipmentArrived = true; Controller.Instance.PriorityTaskCall("stocker"); }
        else if (time == shipmentTime && shipmentArrived) { UIController.Instance.CreateLog(1, "Shipment has already arrived today", "Manufacturer", 0); }//"Shipment has already arrived today", Color.white); }
        if (dayChangeTicker == true) 
        {
            dayChangeTicker = false;
            float mon = (float)dailyMoneyLostWages;
            UIController.Instance.CreateLog(0, Localizer.Instance.GetLocalizedText("Money paid to wages: ") + mon.ToString("f2"), "Accountant", 1);//"Money paid to wages: " + mon.ToString("f2"), Color.white);
            float  mon2 = (float)dailyMoneyLostProduct;
            UIController.Instance.CreateLog(0, Localizer.Instance.GetLocalizedText("Money paid for new product: ") + mon2.ToString("f2"), "Accountant", 1);//"Money paid for new product: " + mon.ToString("f2"), Color.white);
            dailyMoneyLostWages = 0; dailyMoneyLostProduct = 0;
            SaveController.Instance.AutoSave();
        }

        outsideTemp = GetTodaysTemp();
        outsideTemp += GetTimeTemp() - 25;
        if (UIController.Instance.typeOfDay == "Heat wave!!") { outsideTemp =  (outsideTemp * 1.15f) + (5 * TransitionController.Instance.totalDifficulty); }
        if (UIController.Instance.typeOfDay == "SnowStorm!!") { outsideTemp = (outsideTemp * 0.5f) - (10 * TransitionController.Instance.totalDifficulty); }
        insideTemp += ((outsideTemp - insideTemp) / insulation) + heating; //0.1 per heating element
    }
    public void FinishedOrdering() { dayChangeTicker = true; }

    private void DayChange(object sender, System.EventArgs e)
    {
        shipmentArrived = false;
        SpawnChance();

        DailyCostsLog();

        //dayChangeTicker = true;
        //UIController.Instance.UpdateLoanDisplay();
        UIController.Instance.GetGainsAndLosses();
        customerStart = true;
        inflationAmount = ((TransitionController.Instance.inflation * (UIController.Instance.playedMonths + 1))/ 100) + 1;
        NewSeasonCheck(false);
    }
    public IEnumerator SetSpecialDays()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, float> preRandomDays = new Dictionary<string, float>();

        foreach (EventSO events in allEvents)
        {
            if (events.eventType == EventSO.EventType.specialYear)
            {
                specialYears.Add(events.myName, events.yearActivation_if_year);
            }
            else if (events.eventType == EventSO.EventType.holiday)
            {
                specialYearlyDays.Add(events.myName, events.dayActivation_if_holiday);
            }
            else if (events.eventType == EventSO.EventType.weatherEvent)
            {
                if (TransitionController.Instance.tutorialLevel > 2)
                {
                    proceduralDays.Add(events.myName, events.equinox_if_weather);
                }
            }
            else if (events.eventType == EventSO.EventType.randomEvent)
            {
                if (events.positiveEffect)
                {
                    preRandomDays.Add(events.myName, events.chanceOfHappeing_if_random);
                }
                else if (TransitionController.Instance.tutorialLevel > 1)
                {
                    preRandomDays.Add(events.myName, events.chanceOfHappeing_if_random);
                }
            }
            yield return new WaitForEndOfFrame();
        }


        float sum = 0;
        foreach (KeyValuePair<string, float> pair in preRandomDays)
        {
            sum += pair.Value;
        }

        float prevoiusNumber = 0;
        var enumerator = preRandomDays.GetEnumerator();
        for (int i = 0; i < preRandomDays.Count; i++)
        {
            enumerator.MoveNext();
            var pair = enumerator.Current;
            float percentageChance = (float)pair.Value / sum * 100;
            randomDays.Add(pair.Key, (percentageChance + prevoiusNumber));
            prevoiusNumber += percentageChance;
            yield return new WaitForEndOfFrame();
        }

        if (!TransitionController.Instance.loadGame) { StartCoroutine(GetSpecialDays(UIController.Instance.dayOfTheYear)); }
    }
    public IEnumerator GetSpecialDays(int dayOfYear)
    {
        string specialDay = "Regular";

        //year
        foreach (KeyValuePair<string, int> pair in specialYears)
        {
            if (pair.Value == UIController.Instance.year) { }
            yield return new WaitForEndOfFrame();
        }

        //holidays
        foreach (KeyValuePair<string, int> pair in specialYearlyDays)
        {
            if (pair.Value == dayOfYear) { specialDay = pair.Key; }
            yield return new WaitForEndOfFrame();
        }

        //weather
        if (UIController.Instance.day == 1)
        {
            StartCoroutine(GenerateThisMonthWeather(dayOfYear));
        }
        if (specialDay == "Regular")
        {
            specialDay = thisMonthWeather[UIController.Instance.day];
            if (specialDay != "Regular") { ToolTip.Instance.ActivateTutorial(62); }
        }
        yield return new WaitForEndOfFrame();
        //random
        if (specialDay == "Regular") 
        {
            if (randomDays.Count > 0)
            {
                //chance that an event will happen at all
                float chanceOfSpecialDay = 10;
                float chance = Random.Range(0, 100);

                if (chanceOfSpecialDay > chance)
                {
                    chance = Random.Range(0, 100);
                    float closestNumber = 1000;
                    foreach (KeyValuePair<string, float> pair in randomDays)
                    {
                        if (pair.Value > chance && pair.Value - chance < closestNumber) { closestNumber = pair.Value - chance; specialDay = pair.Key; }
                    }
                    ToolTip.Instance.ActivateTutorial(31);
                }
            }
        }
        yield return new WaitForEndOfFrame();
        if (specialDay == "Regular")
        {
            if (previousday == "Thunderstorm!") { specialDay = "Power Outage!"; }
        }

        UIController.Instance.typeOfDay = specialDay;
        StartCoroutine(SpecialDayEffect());
    }
    private IEnumerator SpecialDayEffect()
    {
        yield return new WaitForSeconds(1);
        string specialDay = UIController.Instance.typeOfDay;

        dayType = DayType.normal;

        Camera.main.transform.GetChild(0).gameObject.SetActive(false);
        Camera.main.transform.GetChild(1).gameObject.SetActive(false);

        if (specialDay != "Regular")
        {
            todaysEvent = null;
            foreach(EventSO events in allEvents)
            {
                if (events.myName == specialDay) { todaysEvent = events; break; }
                yield return new WaitForEndOfFrame();
            }

            if (todaysEvent != null)
            {
                worldEmployeeStressIncrease = todaysEvent.worldStress;
                dayType = todaysEvent.dayType;

                switch(todaysEvent.itemsEffected)
                {
                    case "": break;
                    case "Everything": foreach (Customer2 customer in customers) { foreach (KeyValuePair<string, List<float>> pair in customer.ItemPreferences) { pair.Value[0] += todaysEvent.itemsEffectAmount * Controller.Instance.globalItemDemandSwingMultiplier; pair.Value[1] += (todaysEvent.itemsEffectAmount / 100) * Controller.Instance.globalItemDemandSwingMultiplier; yield return new WaitForEndOfFrame(); } } break;
                    case "Random":
                        List<ItemSO> possibleItems = new List<ItemSO>();
                        foreach (ItemSO items in items)
                        {
                            //years
                            if (items.year_Start <= UIController.Instance.year && items.year_End > UIController.Instance.year)
                            {
                                //special item  //seasons fruit
                                if (!items.special || !items.seasonal) { possibleItems.Add(items); }
                                else if (unlockedSpecialItems.Contains(items)) { possibleItems.Add(items); }
                            }
                        }
                        ItemSO anItem = possibleItems[Random.Range(0, possibleItems.Count)];
                        foreach (Customer2 customer in customers) { if (customer.ItemPreferences.ContainsKey(anItem.myName)) { customer.ItemPreferences[anItem.myName][0] += todaysEvent.itemsEffectAmount * Controller.Instance.globalItemDemandSwingMultiplier; customer.ItemPreferences[anItem.myName][1] += (todaysEvent.itemsEffectAmount / 100) * Controller.Instance.globalItemDemandSwingMultiplier; } yield return new WaitForEndOfFrame(); }
                        if (todaysEvent.dayType == DayType.item_Effect) { if (todaysEvent.myName == "Item Boycott!") { specialDay = Localizer.Instance.GetLocalizedText(anItem.myName) + Localizer.Instance.GetLocalizedText(" Boycott!"); } else if (todaysEvent.myName == "Item in Demand!") { specialDay = Localizer.Instance.GetLocalizedText(anItem.myName) + Localizer.Instance.GetLocalizedText(" in Demand!"); } }
                        break;
                    case "Random_Type":
                        ItemSO anItem2 = items[Random.Range(0, items.Count)];
                        foreach (Customer2 customer in customers) 
                        {
                            foreach (ItemSO item in items)
                            {
                                if (item.itemType == anItem2.itemType)
                                {
                                    customer.ItemPreferences[item.myName][0] += todaysEvent.itemsEffectAmount * Controller.Instance.globalItemDemandSwingMultiplier;
                                    customer.ItemPreferences[item.myName][1] += (todaysEvent.itemsEffectAmount / 100) * Controller.Instance.globalItemDemandSwingMultiplier;
                                }
                            }
                            yield return new WaitForEndOfFrame(); 
                        }
                        if (todaysEvent.dayType == DayType.item_Effect) { if (todaysEvent.myName == "Items Boycott!") { specialDay = Localizer.Instance.GetLocalizedText(anItem2.itemType) + Localizer.Instance.GetLocalizedText(" Boycott!"); } else if (todaysEvent.myName == "Items in Demand!") { specialDay = Localizer.Instance.GetLocalizedText(anItem2.itemType) + Localizer.Instance.GetLocalizedText(" in Demand!"); } }
                        break;
                    case "Food":
                        foreach (Customer2 customer in customers)
                        {
                            foreach(ItemSO item in items)
                            {
                                if (item.itemType == "Fruit" || item.itemType == "Vegetable")
                                {
                                    customer.ItemPreferences[item.myName][0] += todaysEvent.itemsEffectAmount * Controller.Instance.globalItemDemandSwingMultiplier; 
                                    customer.ItemPreferences[item.myName][1] += (todaysEvent.itemsEffectAmount / 100) * Controller.Instance.globalItemDemandSwingMultiplier;
                                }
                            }
                            yield return new WaitForEndOfFrame();
                        }
                            break;
                    case "non-food":
                        foreach (Customer2 customer in customers)
                        {
                            foreach (ItemSO item in items)
                            {
                                if (item.itemType != "Fruit" && item.itemType != "Vegetable")
                                {
                                    customer.ItemPreferences[item.myName][0] += todaysEvent.itemsEffectAmount * Controller.Instance.globalItemDemandSwingMultiplier;
                                    customer.ItemPreferences[item.myName][1] += (todaysEvent.itemsEffectAmount / 100) * Controller.Instance.globalItemDemandSwingMultiplier;
                                }
                            }
                            yield return new WaitForEndOfFrame();
                        }
                        break;
                }

                if (todaysEvent.powerOutage) { foreach (Building building in buildings) { building.TurnOff(); yield return new WaitForEndOfFrame(); } }
                if (todaysEvent.noShipments) { shipmentArrived = true; }
                if (todaysEvent.cameraEffect != -1) { Camera.main.transform.GetChild(todaysEvent.cameraEffect).gameObject.SetActive(true); }
            }
            else { specialDay = "Regular"; }
        }

        if (specialDay == "Regular")
        {
            dayType = DayType.normal;
            worldEmployeeStressIncrease = 0;
        }


        UIController.Instance.typeOfDay = specialDay;
                
        if (specialDay != "Regular") 
        {
            UIController.Instance.CreateLog(2, specialDay, "Weather channel", 2);//specialDay, Color.blue); 
            switch(dayType)
            {
                default: UIController.Instance.ChangeSpecialDayDisplay(specialDay, Color.red); break;
                case DayType.weather: UIController.Instance.ChangeSpecialDayDisplay(specialDay, Color.blue); break;
                case DayType.holiday: UIController.Instance.ChangeSpecialDayDisplay(specialDay, Color.green); break;
            }
        }
        else { UIController.Instance.ChangeSpecialDayDisplay(localizer.GetLocalizedText(specialDay) + " " + localizer.GetLocalizedText(UIController.Instance.weekday), Color.black); }
        if (previousday == "Power Outage!") { foreach (Building building in buildings) { building.TurnOn(); } }
        previousday = specialDay;
    }
    private IEnumerator GenerateThisMonthWeather(int dayOfYear)
    {
        dayOfYear -= 1;
        string specialDay = "Regular";
        for (int i = 1; i < 32; i++)
        {
            float chanceOfSpecialDay = 0;
            float chance = Random.Range(0, 100);
            float maximumChance = 35;
            int day = dayOfYear + i;

            foreach (KeyValuePair<string, float> pair in proceduralDays)
            {
                if (day == pair.Value) { chanceOfSpecialDay = maximumChance; }
                else if (day > pair.Value) { chanceOfSpecialDay = maximumChance - ((day - pair.Value) / 3); }
                else if (day < pair.Value) { chanceOfSpecialDay = maximumChance - ((pair.Value - day) / 3); }
                chanceOfSpecialDay = Mathf.Clamp(chanceOfSpecialDay, 0, maximumChance);

                if (chanceOfSpecialDay > chance) { specialDay = pair.Key; break; }
                else { specialDay = "Regular"; }

                //near begining of year
                if (pair.Value < maximumChance)
                {
                    float newValue = 365;
                    float newMaxiumChance = maximumChance - pair.Value;

                    if (day == newValue) { chanceOfSpecialDay = newMaxiumChance; }
                    else if (day > newValue) { chanceOfSpecialDay = newMaxiumChance - ((day - newValue) / 3); }
                    else if (day < newValue) { chanceOfSpecialDay = newMaxiumChance - ((newValue - day) / 3); }
                    chanceOfSpecialDay = Mathf.Clamp(chanceOfSpecialDay, 0, newMaxiumChance);

                    if (chanceOfSpecialDay > chance) { specialDay = pair.Key; break; }
                    else { specialDay = "Regular"; }
                }

                //near end of year
                if (pair.Value + maximumChance > 365)
                {
                    float newValue = 1;
                    float newMaxiumChance = (maximumChance + pair.Value) - 365;

                    if (day == newValue) { chanceOfSpecialDay = newMaxiumChance; }
                    else if (day > newValue) { chanceOfSpecialDay = newMaxiumChance - ((day - newValue) / 3); }
                    else if (day < newValue) { chanceOfSpecialDay = newMaxiumChance - ((newValue - day) / 3); }
                    chanceOfSpecialDay = Mathf.Clamp(chanceOfSpecialDay, 0, newMaxiumChance);

                    if (chanceOfSpecialDay > chance) { specialDay = pair.Key; break; }
                    else { specialDay = "Regular";}
                }
                yield return new WaitForEndOfFrame();
            }

            thisMonthWeather[i] = specialDay;
            yield return new WaitForEndOfFrame();
        }
    }
    private float spawnNumber;
    private void SpawnChance()
    {
        if (customers.Count < max_Customer_Count)
        {
            spawnNumber += TransitionController.Instance.cityGrowth;
            int spawns = (int)spawnNumber;
            spawnNumber -= spawns;

            if (spawns > 0)
            {
                for (int i = 0; i < spawns; i++)
                {
                    SpawnCustomer(false);
                }
            }
        }
    }
    public void SpawnCustomer(bool fired)
    {
        float number = Random.Range(15, 115);
        string birthName = "";
        bool isFemale = false;
        if (Random.Range(0, 2) == 0) { isFemale = false; }
        else { isFemale = true; }
        birthName = Names.Instance.GetName(isFemale);
        Customer2 placedObject = Customer2.Create(customerSO, number, birthName, false, isFemale);
        if (fired) { placedObject.storePreferance[0] = 0; }
        placedObject.applyBanned = true;
    }
    private IEnumerator CityPopulationSpawn()
    {
        yield return new WaitForSeconds(5);

        for (int i = 0; i < TransitionController.Instance.cityPopulation; i++)
        {
            float number = Random.Range(15, 115);
            string birthName = "";
            bool isFemale = false;
            if (Random.Range(0, 2) == 0) { isFemale = false; }
            else { isFemale = true; }
            birthName = Names.Instance.GetName(isFemale);
            Customer2 placedObject = Customer2.Create(customerSO, number, birthName, false, isFemale);
            yield return new WaitForEndOfFrame();
        }

        Advertising.Instance.UpdateAdvert();
    }
    public IEnumerator GenerateAvailableOrders()
    {
        //if store seels type of item or if special item
        foreach (ItemSO item in items) 
        { 
            if (item.year_Start <= UIController.Instance.year && !item.special && !item.seasonal) 
            {
                UIController.Instance.CreateOrder(item);
            }
            UIController.Instance.CreateStorageOption(item);
            yield return new WaitForEndOfFrame();
        }
        UIController.Instance.CreateStorageOption(null);
        NewSeasonCheck(true);
    }
    public void ChangeMusic(int selection)
    {
        musicWorkBonus -= (int)musicBonuses.x;
        musicSpeedBonus -= (int)musicBonuses.y;
        musicStressBonus -= (int)musicBonuses.z;
        musicBonuses = new Vector3 (0,0,0);
        music[previousMusicChoice].Stop();

        float setVolume = music[previousMusicChoice].volume;

        previousMusicChoice = selection;
        music[previousMusicChoice].Play();
        music[previousMusicChoice].volume = setVolume * musicVolume;

        switch (selection)
        {
            case 0: musicBonuses = new Vector3(2, -1, 0); break;//shiney Mall
            case 1: musicBonuses = new Vector3(0, 0, -1); break;//Elevator
            case 2: musicBonuses = new Vector3(1, 0, 0); break;//boss elevator
            case 3: musicBonuses = new Vector3(-1, 2, 0); break;//happy shopping
            case 4: musicBonuses = new Vector3(0, -1, -2); break;//shopping
        }

        musicWorkBonus += (int)musicBonuses.x;
        musicSpeedBonus += (int)musicBonuses.y;
        musicStressBonus += (int)musicBonuses.z;
        UIController.Instance.UpdateStoreMusicDisplay();
    }
    public void ChangeMusicSpeed(Slider slider)
    {
        if (slider != null)
            {
                musicWorkBonus -= (int)musicSpeedBonuses.x;
                musicSpeedBonus -= (int)musicSpeedBonuses.y;
                musicStressBonus -= (int)musicSpeedBonuses.z;

                //music[previousMusicChoice].pitch = 0.8f + (slider.value / 10);
                foreach (AudioSource audio in music) { audio.pitch = 0.8f + (slider.value / 10); }

                switch (slider.value)
                {
                    case -2: musicSpeedBonuses = new Vector3(-2, -1, -2); break;
                    case -1: musicSpeedBonuses = new Vector3(-1, 0, -1); break;
                    case 0: musicSpeedBonuses = new Vector3(0, 0, 0); break;
                    case 1: musicSpeedBonuses = new Vector3(1, 0, 1); break;
                    case 2: musicSpeedBonuses = new Vector3(2, 1, 2); break;
                }

                musicWorkBonus += (int)musicSpeedBonuses.x;
                musicSpeedBonus += (int)musicSpeedBonuses.y;
                musicStressBonus += (int)musicSpeedBonuses.z;

                setPitch = (int)slider.value;
            }
            else
            {
                int number = setPitch;

                musicWorkBonus -= (int)musicSpeedBonuses.x;
                musicSpeedBonus -= (int)musicSpeedBonuses.y;
                musicStressBonus -= (int)musicSpeedBonuses.z;

                //music[previousMusicChoice].pitch = 0.8f + (number / 10);
                foreach (AudioSource audio in music) { audio.pitch = 0.8f + (number / 10); }

                switch (number)
                {
                    case -2: musicSpeedBonuses = new Vector3(-2, -1, -2); break;
                    case -1: musicSpeedBonuses = new Vector3(-1, 0, -1); break;
                    case 0: musicSpeedBonuses = new Vector3(0, 0, 0); break;
                    case 1: musicSpeedBonuses = new Vector3(1, 0, 1); break;
                    case 2: musicSpeedBonuses = new Vector3(2, 1, 2); break;
                }

                musicWorkBonus += (int)musicSpeedBonuses.x;
                musicSpeedBonus += (int)musicSpeedBonuses.y;
                musicStressBonus += (int)musicSpeedBonuses.z;

                setPitch = number;
            }
            UIController.Instance.UpdateStoreMusicDisplay();
        
    }
    public void ChangeMusicVolume(Slider slider)
    {

            if (slider != null)
            {
                musicWorkBonus -= (int)musicVolumeBonuses.x;
                musicSpeedBonus -= (int)musicVolumeBonuses.y;
                musicStressBonus -= (int)musicVolumeBonuses.z;

                music[previousMusicChoice].volume = (0.2f + (slider.value / 20)) * musicVolume;

                switch (slider.value)
                {
                    case -2: musicVolumeBonuses = new Vector3(1, -2, -2); break;
                    case -1: musicVolumeBonuses = new Vector3(0, -1, -1); break;
                    case 0: musicVolumeBonuses = new Vector3(0, 0, 0); break;
                    case 1: musicVolumeBonuses = new Vector3(0, 1, 1); break;
                    case 2: musicVolumeBonuses = new Vector3(-1, 2, 2); break;
                }

                musicWorkBonus += (int)musicVolumeBonuses.x;
                musicSpeedBonus += (int)musicVolumeBonuses.y;
                musicStressBonus += (int)musicVolumeBonuses.z;

                setVolume = (int)slider.value;
            }
            else
            {
                int number = setVolume;

                musicWorkBonus -= (int)musicVolumeBonuses.x;
                musicSpeedBonus -= (int)musicVolumeBonuses.y;
                musicStressBonus -= (int)musicVolumeBonuses.z;

                music[previousMusicChoice].volume = (0.2f + (number / 20)) * musicVolume;

                switch (number)
                {
                    case -2: musicVolumeBonuses = new Vector3(1, -2, -2); break;
                    case -1: musicVolumeBonuses = new Vector3(0, -1, -1); break;
                    case 0: musicVolumeBonuses = new Vector3(0, 0, 0); break;
                    case 1: musicVolumeBonuses = new Vector3(0, 1, 1); break;
                    case 2: musicVolumeBonuses = new Vector3(-1, 2, 2); break;
                }

                musicWorkBonus += (int)musicVolumeBonuses.x;
                musicSpeedBonus += (int)musicVolumeBonuses.y;
                musicStressBonus += (int)musicVolumeBonuses.z;

                setVolume = number;
            }
            UIController.Instance.UpdateStoreMusicDisplay();
        

    }
    private void GetCustomerPreferance(object sender, System.EventArgs e)
    {
        List<int> StorePreferances = new List<int>();
        foreach (Customer2 customer in customers)
        {
            float preferance = 0;
            int preferanceStore = 0;

            for (int i = 0; i < customer.storePreferance.Count; i++)
            {
                if (customer.storePreferance[i] > preferance)
                {
                    preferance = customer.storePreferance[i];
                    preferanceStore = i;
                }
            }

            StorePreferances.Add(preferanceStore);
        }

        UIController.Instance.UpdateCustomerPreferance(StorePreferances);
    }
    public void LoadMusic(int selection)
    {
        ChangeMusic(selection);
        previousMusicChoice = selection;
        musicSpeedSlider.value = setPitch;
        musicVolumeSlider.value = setVolume * musicVolume;
        ChangeMusicVolume(null);
        ChangeMusicSpeed(null);
    }

    public float globalCalloutChanceMultiplier = 1;
    public float globalLoyaltyMultiplier = 1;
    public bool competitorBailouts;
    public float globalEmployeeStressMultiplier = 1;
    public float globalItemDemandSwingMultiplier = 1;
    public float globalCustomerWageMultiplier = 1;
    public float competitorFreeMoney = 0;

    public void NewYear()
    {
        foreach (ItemSO item in items) 
        {
            if (item.year_Start == (UIController.Instance.year + 1) && !item.seasonal && !item.special)
            {
                NewItemCheck(item);
            }
        }

        EventSO thisEvent = null;

        foreach(EventSO events in allEvents) { if (events.eventType == EventSO.EventType.specialYear && events.yearActivation_if_year == UIController.Instance.year) { thisEvent = events; } }
        if (thisEvent != null)
        {
            globalCalloutChanceMultiplier = thisEvent.globalCalloutChanceMultiplier;
            globalCalloutChanceMultiplier = thisEvent.globalCalloutChanceMultiplier;
            globalLoyaltyMultiplier = thisEvent.globalLoyaltyMultiplier;
            competitorBailouts = thisEvent.competitorBailouts;
            globalEmployeeStressMultiplier = thisEvent.globalEmployeeStressMultiplier;
            globalItemDemandSwingMultiplier = thisEvent.globalItemDemandSwingMultiplier;
            globalCustomerWageMultiplier = thisEvent.globalCustomerWageMultiplier;
            competitorFreeMoney = thisEvent.competitorFreeMoney;

            UIController.Instance.CreateLog(1, Localizer.Instance.GetLocalizedText(thisEvent.message), "Weather channel", 0);
            UIController.Instance.dateText.color = Color.red;
        }
        else
        {
            //transition controller?
            globalCalloutChanceMultiplier = 1;
            globalLoyaltyMultiplier = 1;
            competitorBailouts = false;
            globalEmployeeStressMultiplier = 1;
            globalItemDemandSwingMultiplier = 1;
            globalCustomerWageMultiplier = 1;
            competitorFreeMoney = 0;
            UIController.Instance.dateText.color = Color.black;
        }

        //competitiors
        NewYearCustomer();
    }
    private void NewYearCustomer()
    {
        //customers now want new items
        foreach (ItemSO item in items)
        {
            if (item.year_Start == UIController.Instance.year)
            {
                foreach (Customer2 customer in customers)
                {
                    customer.ItemPreferences[item.myName][0] = Random.Range(0, 100);
                    customer.ItemPreferences[item.myName][1] = (item.defaultNeedGrowth + Random.Range(-item.defaultNeedGrowth, item.defaultNeedGrowth * 1.5f)) * 5;
                }
            }
            //a year after
            if (item.year_Start == UIController.Instance.year - 1)
            {
                foreach (Customer2 customer in customers)
                {
                    customer.ItemPreferences[item.myName][1] /= 5;
                }
            }
            /*
            //a year before
            if (item.year_End == UIController.Instance.year - 1)
            {
                foreach (Customer2 customer in customers)
                {
                    customer.ItemPreferences[item.myName][1] /= 5;
                }
            }
            */
            if (item.year_End <= UIController.Instance.year)
            {
                foreach (Customer2 customer in customers)
                {
                    customer.ItemPreferences[item.myName][1] = 0;
                }
                if (!removedItems.Contains(item)) { removedItems.Add(item); }

            }
        }
    }
    private IEnumerator NewSeasonCustomer()
    {
        //winter
        if (UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[0] || UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[3])
        //if (UIController.Instance.dayOfTheYear == SeasonsStart[3])
        {
            foreach (ItemSO item in items)
            {
                if (item.seasonal)
                {
                    if (item.seasons.Contains("Fall") && !item.seasons.Contains("Winter"))
                    {
                        //if item was previos season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] /= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    if (item.seasons.Contains("Winter") && !item.seasons.Contains("Fall"))
                    {
                        //if item is current season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] *= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
                if (item.inDemandSeason != "All")
                {
                    float modifier = 1;
                    switch(item.inDemandSeason)
                    {
                        case "Spring": modifier = -10; break;
                        case "Summer": modifier = -10; break;
                        case "Fall": modifier = 10; break;
                        case "Winter": modifier = 10; break;
                    }

                    foreach (Customer2 customer in customers)
                    {
                        if (modifier > 0) { customer.ItemPreferences[item.myName][1] *= modifier; }
                        else { customer.ItemPreferences[item.myName][1] /= -modifier; }
                        
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
        //spring
        if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[0] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[1])
            //if (UIController.Instance.dayOfTheYear == SeasonsStart[0])
        {
            foreach (ItemSO item in items)
            {
                if (item.seasonal)
                {
                    if (item.seasons.Contains("Winter") && !item.seasons.Contains("Spring"))
                    {
                        //if item was previos season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] /= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    if (item.seasons.Contains("Spring") && !item.seasons.Contains("Winter"))
                    {
                        //if item is current season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] *= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
                if (item.inDemandSeason != "All")
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = 10; break;
                        case "Summer": modifier = -10; break;
                        case "Fall": modifier = -10; break;
                        case "Winter": modifier = 10; break;
                    }

                    foreach (Customer2 customer in customers)
                    {
                        if (modifier > 0) { customer.ItemPreferences[item.myName][1] *= modifier; }
                        else { customer.ItemPreferences[item.myName][1] /= -modifier; }

                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
        //summer
        if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[1] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[2])
        //if (UIController.Instance.dayOfTheYear == SeasonsStart[1])
        {
            foreach (ItemSO item in items)
            {
                if (item.seasonal)
                {
                    if (item.seasons.Contains("Spring") && !item.seasons.Contains("Summer"))
                    {
                        //if item was previos season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] /= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    if (item.seasons.Contains("Summer") && !item.seasons.Contains("Spring"))
                    {
                        //if item is current season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] *= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
                if (item.inDemandSeason != "All")
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = 10; break;
                        case "Summer": modifier = 10; break;
                        case "Fall": modifier = -10; break;
                        case "Winter": modifier = -10; break;
                    }

                    foreach (Customer2 customer in customers)
                    {
                        if (modifier > 0) { customer.ItemPreferences[item.myName][1] *= modifier; }
                        else { customer.ItemPreferences[item.myName][1] /= -modifier; }

                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
        //fall
        if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[2] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[3])
        //if (UIController.Instance.dayOfTheYear == SeasonsStart[2])
        {
            foreach (ItemSO item in items)
            {
                if (item.seasonal)
                {
                    if (item.seasons.Contains("Summer") && !item.seasons.Contains("Fall"))
                    {
                        //if item was previos season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] /= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    if (item.seasons.Contains("Fall") && !item.seasons.Contains("Summer"))
                    {
                        //if item is current season
                        foreach (Customer2 customer in customers)
                        {
                            customer.ItemPreferences[item.myName][1] *= 10;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
                if (item.inDemandSeason != "All")
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = -10; break;
                        case "Summer": modifier = 10; break;
                        case "Fall": modifier = 10; break;
                        case "Winter": modifier = -10; break;
                    }

                    foreach (Customer2 customer in customers)
                    {
                        if (modifier > 0) { customer.ItemPreferences[item.myName][1] *= modifier; }
                        else { customer.ItemPreferences[item.myName][1] /= -modifier; }

                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
    private float GetTodaysTemp()
    {
        int x = UIController.Instance.dayOfTheYear;
        float avg = TransitionController.Instance.averageTemp;
        float min = TransitionController.Instance.lowTemp;
        float max = TransitionController.Instance.highTemp;

        if (x <= 0f) return min;
        if (x <= 86f) return ((avg - min) / 86f) * x + min;
        if (x <= 172f) return ((max - avg) / (172f - 86f)) * (x - 86f) + avg;
        if (x <= 355f) return ((min - avg) / (355f - 258f)) * (x - 258f) + avg;
        return min;
    }
    private float GetTimeTemp()
    {
        //25 degree temp differenace
        int x = UIController.Instance.hour;

        float avg = 12.5f;
        float min = 0;
        float max = 25;

        if (x <= 0f) return min;
        if (x <= 6) return ((avg - min) / 6) * x + min;
        if (x <= 12) return ((max - avg) / (12 - 6)) * (x - 6) + avg;
        if (x <= 24) return ((min - avg) / (24 - 18)) * (x - 18) + avg;
        return min;
    }
    private void DailyCostsLog()
    {
        leaseDue += (TransitionController.Instance.leasePricePerSquareFoot * MapController.Instance.ownedTilesCount) / 30;

        //add interest to loans
        foreach(Loan loan in loans) { loan.DayChange(); }

        if (MerchandiceExpiredMoneyLost > 0)
        {
            UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Expired merchandise sunk cost: ") + MerchandiceExpiredMoneyLost.ToString("f2"), "Accountant", 1);//"Expired merchandise sunk cost: " + MerchandiceExpiredMoneyLost.ToString("f2"), Color.white);
            MerchandiceExpiredMoneyLost = 0;
        }

        itemsSoldDaily = itemsSoldTotal - itemsSoldPast;
        itemsSoldPast = itemsSoldTotal;

        UIController.Instance.HiringFees();
        //if daily - do nothing different
        if (billpayments == "Weekly" && UIController.Instance.weekday != "Monday") { return; }
        if (billpayments == "Biweekly" && (UIController.Instance.day != 1 || UIController.Instance.day != 15)) { return; }
        if (billpayments == "Monthly" && UIController.Instance.day != 1) { return; }

        //electricty
        float mon = (float)electricityCost * TransitionController.Instance.electrictyCosts;
        if (mon > 0)
        {
            UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Electricity Bill: ") + mon.ToString("f2"), "Accountant", 1);//"Electricity Bill: " + mon.ToString("f2"), Color.white);
            MoneyValueChange(-mon, UtilsClass.GetMouseWorldPosition(), true, false);
        }
        electricityCost = 0;

        //rent
        //float leasePay = (TransitionController.Instance.leasePricePerSquareFoot * MapController.Instance.ownedTilesCount) / 30;
        if (leaseDue > 0)
        {
            UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Lease Payment: ") + leaseDue.ToString("f2"), "Accountant", 1);//"Lease Payment: " + leaseDue.ToString("f2"), Color.white);
            MoneyValueChange(-leaseDue, UtilsClass.GetMouseWorldPosition(), true, false);
        }
        leaseDue = 0;

        if (MoneyMadeByDebt > 0)
        {
            UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Money made by store credit: ") + MoneyMadeByDebt.ToString("f2"), "Accountant", 1);
        }
        MoneyMadeByDebt = 0;

        //debt
        if (loans.Count > 0)
        {
            float payAmount = loans.Count * 100;
            UIController.Instance.CreateLog(0, localizer.GetLocalizedText("Debt collection Bill: ") + payAmount.ToString("f2"), "Accountant", 1);
            MoneyValueChange(-payAmount, UtilsClass.GetMouseWorldPosition(), true, false);
            foreach (Loan loan in loans) { loan.MakePayment(100); }
        }
    }
    public void GetAvailableItems(List<ItemSO> allItems)
    {
        if (TransitionController.Instance.items[0] == "Everything")
        {
            items = allItems;
            foreach (ItemSO item in items) { itemsSold.Add(item.myName, 0); }
        }
        else
        {
            foreach (string catagory in TransitionController.Instance.items)
            {
                foreach (ItemSO item in allItems)
                {
                    if (item.myName == catagory || item.itemType == catagory) { items.Add(item); itemsSold.Add(item.myName, 0); }
                }
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }
    }
    public void MentalBreakOccuring()
    {
        TickSystem.Instance.On600Tick += Waiting;
    }
    public void Call911()
    {
        Debug.Log("Spawn");
        GameObject newOfficer = Instantiate(officer, startingPoints[Random.Range(0, startingPoints.Count)].transform.position, Quaternion.Euler(0, 0, 0));
        newOfficer.GetComponent<Officer>().Setup(0);

        newOfficer = Instantiate(officer, startingPoints[Random.Range(0, startingPoints.Count)].transform.position, Quaternion.Euler(0, 0, 0));
        newOfficer.GetComponent<Officer>().Setup(1);

        newOfficer = Instantiate(officer, startingPoints[Random.Range(0, startingPoints.Count)].transform.position, Quaternion.Euler(0, 0, 0));
        newOfficer.GetComponent<Officer>().Setup(2);
    }
    private void Waiting(object sender, TickSystem.OnTickEventArgs e)
    {
        Call911();
        TickSystem.Instance.On600Tick -= Waiting;
    }
    public void NewItemCheck(ItemSO newItem)
    {
        if (newItem != null)
        {
            if (!unlockedSpecialItems.Contains(newItem)) { unlockedSpecialItems.Add(newItem); }
            foreach (UIItemOrder order in OrderManager.Instance.orders) { if (order.item == newItem) { return; } }
            UIController.Instance.CreateOrder(newItem);
            return;
        }
    }
    public List<int> SeasonsStart = new List<int>() { 78, 171, 266, 355 };
    private void NewSeasonCheck(bool newGame)
    {
        bool today = false;
        foreach(int start in SeasonsStart) { if (UIController.Instance.dayOfTheYear == start) { today = true; break; } }
        if (today || newGame)
        {
            foreach (ItemSO item in items)
            {
                if (item.seasonal)
                {
                    if (UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[0] || UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[3])
                    {
                        if (!item.seasons.Contains("Winter")) { if (unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Remove(item); } }
                        else { if (!unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Add(item); NewItemCheck(item); } }
                        continue;
                    }
                    if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[0] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[1])
                    {
                        if (!item.seasons.Contains("Spring")) { if (unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Remove(item); } }
                        else { if (!unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Add(item); NewItemCheck(item); } }
                        continue;
                    }
                    if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[1] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[2])
                    {
                        if (!item.seasons.Contains("Summer")) { if (unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Remove(item); } }
                        else { if (!unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Add(item); NewItemCheck(item); } }
                        continue;
                    }
                    if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[2] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[3])
                    {
                        if (!item.seasons.Contains("Fall")) { if (unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Remove(item); } }
                        else { if (!unlockedSpecialItems.Contains(item)) { unlockedSpecialItems.Add(item); NewItemCheck(item); } }
                        continue;
                    }
                }
            }
            if (today) { UIController.Instance.CreateLog(3, localizer.GetLocalizedText("It is a new season! Orders and customer interests may have changed!"), localizer.GetLocalizedText("Manager"), 0); }
            StartCoroutine(NewSeasonCustomer());
        }
    }

    public void LoadOrders(List<string> names, List<int> amounts, List<float> prices, List<string> manufactorers)
    {
        foreach(UIItemOrder order in OrderManager.Instance.orders)
        {
            int number = -1;
            number = names.IndexOf(order.item.myName);
            if (number != -1)
            {
                order.markUpSlider.value = prices[number];
                order.ChangeSupplier(manufactorers[number]);
                order.AddOrder(amounts[number]);
            }
        }
    }

    [HideInInspector] public int maxLevel = 20;
    [HideInInspector] public float baseWalkSpeed = 0;
    [HideInInspector] public float baseWorkSpeed = 75;
    [HideInInspector] public float baseAudio = 1;
    [HideInInspector] public float baseLearning = 1;
    [HideInInspector] public float baseCalloutChance = 3;
    [HideInInspector] public float baseStressRelease = 1;
    [HideInInspector] public float baseStressAccumulate = 1;
    [HideInInspector] public float baseSocial = 0;
    [HideInInspector] public float baseLoaylty = 25;

    [HideInInspector] public float c_baseWalkSpeed = 0;
    [HideInInspector] public float c_baseWorkSpeed = 3;
    [HideInInspector] public float c_baseAudio = 1;
    [HideInInspector] public float c_baseSocial = 5;
    [HideInInspector] public float c_baseLoaylty = 1;
    [HideInInspector] public float c_needy = 1;
    [HideInInspector] public float c_money = 10.5f;
    [HideInInspector] public float c_greed = 1;
    [HideInInspector] public int c_inStoreTime = 30;

    [HideInInspector] public int max_Customer_Count = 150;

    public void PriorityTaskCall(string task)
    {
        if (priorityTask.Contains(task))
        {
            int choosenOne = 0;
            int highestLevel = -1;
            string taskName = null;
            for (int i = 0; i < employees.Count; i++)
            {
                switch(task)
                {
                    case "stocker":
                        if (employees[i].inventorySkill > highestLevel && employees[i].atWork)
                        {
                            highestLevel = employees[i].inventorySkill;
                            choosenOne = i;
                        }
                        taskName = "stocker";
                            break;
                    case "cashier":
                        if (employees[i].customerServiceSkill > highestLevel && employees[i].atWork)
                        {
                            highestLevel = employees[i].customerServiceSkill;
                            choosenOne = i;
                        }
                        taskName = "cashier";
                        break;
                    case "janitor":
                        if (employees[i].janitorialSkill > highestLevel && employees[i].atWork)
                        {
                            highestLevel = employees[i].janitorialSkill;
                            choosenOne = i;
                        }
                        taskName = "janitor";
                        break;
                    case "engineer":
                        if (employees[i].engineerSkill > highestLevel && employees[i].atWork)
                        {
                            highestLevel = employees[i].engineerSkill;
                            choosenOne = i;
                        }
                        taskName = "engineer";
                        break;
                    case "manager":
                        if (employees[i].managementSkill > highestLevel && employees[i].atWork)
                        {
                            highestLevel = employees[i].managementSkill;
                            choosenOne = i;
                        }
                        taskName = "manager";
                        break;
                }
            }

            if (employees[choosenOne].task.ToString() != taskName) { employees[choosenOne].SwitchTask(null, taskName, null); Debug.Log("Forced Switch Task to: " + taskName); }
        }
    }
    [SerializeField] private List<AudioAdjuster> uiAudios = new List<AudioAdjuster>();
    public List<AudioAdjuster> fxAudios = new List<AudioAdjuster>();
    public void AdjustMusicVolumeSet(Slider set) { musicVolume = set.value / 10f; ChangeMusicVolume(null); PlayerPrefs.SetFloat("Music_Volume", musicVolume); }
    public void AdjustFXVolumeSet(Slider set) { fxVolume = set.value / 10f; foreach (AudioAdjuster audio in fxAudios) { audio.AdjustVolume(); } PlayerPrefs.SetFloat("FX_Volume", fxVolume); }
    public void AdjustTalkVolumeSet(Slider set) { talkVolume = set.value / 10f; PlayerPrefs.SetFloat("Talk_Volume", talkVolume); }
    public void AdjustUIVolumeSet(Slider set) { uiVolume = set.value / 10f; foreach (AudioAdjuster audio in uiAudios) { audio.AdjustVolume(); } PlayerPrefs.SetFloat("UI_Volume", uiVolume); }
    [SerializeField] private Slider[] volumeSliders;
    private void GetSavedAudioSettings()
    {
        if (PlayerPrefs.HasKey("Music_Volume"))
        {
            musicVolume = PlayerPrefs.GetFloat("Music_Volume");
            volumeSliders[0].value = musicVolume * 10;
        }
        if (PlayerPrefs.HasKey("FX_Volume"))
        {
            fxVolume = PlayerPrefs.GetFloat("FX_Volume");
            volumeSliders[1].value = fxVolume * 10;
        }
        if (PlayerPrefs.HasKey("Talk_Volume"))
        {
            talkVolume = PlayerPrefs.GetFloat("Talk_Volume");
            volumeSliders[2].value = talkVolume * 10;
        }
        if (PlayerPrefs.HasKey("UI_Volume"))
        {
            uiVolume = PlayerPrefs.GetFloat("UI_Volume");
            volumeSliders[3].value = uiVolume * 10;
        }
    }
}
