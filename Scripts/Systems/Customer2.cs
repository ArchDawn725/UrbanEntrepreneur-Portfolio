using System;
using System.Collections.Generic;
using UnityEngine;
using static MapController;
using Random = UnityEngine.Random;

public class Customer2 : MonoBehaviour
{
    public string myName;
    public string birthName;
    public float age;
    public float storeOpinion;
    public bool isFemale;

    private const float MAX_SPEED = 50;
    private const float MIN_SPEED = 6; 
    private float baseSpeed;
    public float speedCalc = 30;
    public float GetSpeed(MapController.NewGrid newGrid) { if (GetGrid() != null) { return Math.Clamp((baseSpeed + GetAgeMultiplier() + GetCleanMultiplier() + (Controller.Instance.musicSpeedBonus * auditorySensitivity) + GetLightMultiplier() + newGrid.GetSpeed()), MIN_SPEED, MAX_SPEED) * TickSystem.Instance.timeMultiplier; } else return baseSpeed; }
    public float GetAgeMultiplier() { return ((float)(age * -1f) / 5f) + 10; } //-10 , +10
    //level
    //stress?
    public float GetCleanMultiplier() { if (GetGrid() != null) { return ((GetGrid().GetCleanNormalized() * -1) * 15) + 10; } else { return 0; } } //-5, +10
    //music?
    //audio sensitive?
    public float GetLightMultiplier() { if (GetGrid() != null) { return (GetGrid().GetLightNormalized() * 15) - 5; } else { return 0; } } //-5 , +10
    //building speed reducer
    //manager
    //trained

    /*
    //muisic // -6, +6
    public float GetSpeed() { return (BASE_SPEED + GetLightMultiplier() + GetCleanMultiplier() + (Controller.Instance.musicSpeedBonus * auditorySensitivity) + GetAgeMultiplier()) * TickSystem.Instance.timeMultiplier; }
    public float GetLightMultiplier() { return 3 * GetGrid().GetLightNormalized() - 2; } //-2 , +1

    public float GetCleanMultiplier() { return 3 *  GetGrid().GetCleanNormalized() - 2; } //-2, +1
    */

    public int capacity;

    //Hidden
    //Transforms
    private BoxCollider2D myCollider;
    private BoxCollider2D myCollider2;
    private Transform visuals;
    private Transform container;
    private Emotion emotion;
    public void OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont) { box = myCollider; vis = visuals; cont = container; }
    public Animator animator;
    private ChatMessage bubble;

    //pathfinding
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    public Vector3 targetPosition;
    public void OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos) { currentPath = currentPathIndex; pathList = pathVectorList; targetPos = targetPosition; }
    public void SetPathfinding(in int currentPath, in List<Vector3> pathList, in Vector3 targetPos) { currentPathIndex = currentPath; pathVectorList = pathList; targetPosition = targetPos; }

    //AI
    public Transform target;
    private int newTask;

    public void OutAI(out Transform targ, out int newTsk) { targ = target; newTsk = newTask; }
    public void SetAI(in Transform targ, in int newTsk) { target = targ; newTask = newTsk; }

    public Item selectedItem;
    public int targetItemID;

    public Building targetBuilding;

    public Building targetRegistor;
    public Building targetShelf;
    public Building targetStockPile;

    public Building newTargetBuilding;

    public void GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding) { selectItem = selectedItem; targItemID = targetItemID; targBuilding = targetBuilding; targRegistor = targetRegistor; targShelf = targetShelf; targStockPile = targetStockPile; newTargBuilding = newTargetBuilding; }
    public void SetTargets(in Item selectItem, in int targItemID, in Building targBuilding, in Building targRegistor, in Building targShelf, in Building targStockPile, in Building newTargBuilding) { selectedItem = selectItem; targetItemID = targItemID; targetBuilding = targBuilding; targetRegistor = targRegistor; targetShelf = targShelf; targetStockPile = targStockPile; newTargetBuilding = newTargBuilding; }
    [Space(10)]
    [Header("Debugs")]
    [HideInInspector] public bool messageCalled;
    [SerializeField] private bool selected;
    public bool activated;
    public bool shopping;

    public int timeTicks;
    public int activeTicks;
    public Dictionary<string, List<float>> ItemPreferences = new Dictionary<string, List<float>>();
    public Dictionary<string, bool> ShoppingDays = new Dictionary<string, bool>();
    public int shoppingTime = -1;
    [SerializeField] private int shoppingMinute = -1;
    public List<float> storePreferance = new List<float>();
    public List<ItemSO> shoppingList = new List<ItemSO>();
    public List<ItemSO> couldNotAffordList = new List<ItemSO>();
    public bool patroling;
    private Vector3 startPos;
    public Dictionary<string, bool> traits = new Dictionary<string, bool>();
    private int stayInStoreTimer;
    [HideInInspector] public float storeImpact;
    private float scheduleStrictness;
    private float auditorySensitivity;
    private float callOutChance;
    private float needyModifier;
    private float tempPref;
    public int workSpeed;
    private GameObject moodBar;
    private BarController moodBarCon;
    private Dictionary<string, int> queuedComplaints = new Dictionary<string, int>();
    private Dictionary<string, int> allComplaints = new Dictionary<string, int>();
    private Dictionary<string, int> allComplements = new Dictionary<string, int>();
    public PersonVisualCon personVis;
    public float money;
    public float income;
    private float greed;
    private float socialSkills;
    public bool special;
    public float moneyOwedToStore;
    //public Dictionary<Building, Item> Memory = new Dictionary<Building, Item>();
    public List<Building> Memory = new List<Building>();
    public Wall entrance;
    public bool beforeLine;
    public bool applyBanned;
    public bool insideStore;

    private void OnMouseEnter() 
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

        float value = storeOpinion * 1f / 200;
        if (value > 1) { value = 1; }
        if (value < 0) { value = 0; }
        moodBarCon.Activate(value);
        moodBar.SetActive(true);
        moodBar.GetComponent<FadeController>().autoFade = false;
        moodBar.GetComponent<FadeController>().Activate();
    }
    private void OnMouseExit() 
    { 
        if (!selected) { transform.GetChild(0).GetChild(0).gameObject.SetActive(false); }
        moodBar.GetComponent<FadeController>().autoFade = true;
    }
    public void Selected() { selected = true; OnMouseEnter(); ToolTip.Instance.DismissTutorial(27); }
    public void Deselected() { selected = false; OnMouseExit(); }

    private void TimeSchedule(object sender, System.EventArgs e)
    {
        timeTicks++;
        if (timeTicks >= 96) { DaySchedule(); timeTicks = 0; } // 4 x 24
        if (activated)
        {
            activeTicks++;
            if (activeTicks >= stayInStoreTimer) { animator.SetBool("WaitingOutside", false); }
            if (activeTicks >= stayInStoreTimer * 2) { animator.SetBool("Searching", false); } // 4 x 6
            if (activeTicks >= stayInStoreTimer * 3) { animator.SetBool("Waiting", false); animator.SetBool("AtSite", false); } // 4 x 12
            if (activeTicks >= stayInStoreTimer * 5) { StuckFailSafe(); } // 4 x 12

            if (activeTicks % 5 == 0)
            {
                float value = storeOpinion * 1f / 200;
                if (value > 1) { value = 1; }
                if (value < 0) { value = 0; }
                moodBarCon.Activate(value);
                moodBar.SetActive(true);
                moodBar.GetComponent<FadeController>().Activate();
            }
        }

        if (shopping)
        {
            int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
            if (Controller.Instance.storeOpen != 0000)
            {
                if (time >= Controller.Instance.storeOpen) { }
                else { animator.SetBool("Searching", false); }
            }
            if (Controller.Instance.storeClose != 2400)
            {
                if (time < Controller.Instance.storeClose - 30) { }
                else { animator.SetBool("Searching", false); }
            }
        }

        if (!activated)
        {
            if (ShoppingDays[UIController.Instance.weekday] == true && ComeInTodayChance() == true)
            {

                if (UIController.Instance.hour == shoppingTime && UIController.Instance.minutes == shoppingMinute && Controller.Instance.customerStart)
                {
                    if (Random.Range(0, 100.01f) > callOutChance * Controller.Instance.globalCalloutChanceMultiplier)
                    {
                        if (Controller.Instance.dayType == Controller.DayType.weather)
                        {
                            if (traits["weather_fearful"]) { return; }
                            else { ChooseStore(); }
                        }
                        else if (Controller.Instance.dayType == Controller.DayType.holiday) 
                        {
                            if (traits["vacationer"]) { return; }
                            else { ChooseStore(); }
                        }
                        else { ChooseStore(); }
                    }
                    else if (traits["weather_lover"] && Controller.Instance.dayType == Controller.DayType.weather) { ChooseStore(); }
                    else if (traits["procrastinator"] && Controller.Instance.dayType == Controller.DayType.holiday) { ChooseStore(); }
                }
            }
        }

        if (!applied && !applyBanned) { if (Random.Range(0, 100) > 90) { Apply(); } }
    }
    public void StuckFailSafe() { StuckPositionFailSafe(); animator.Rebind(); RemoveAllClaims(); Debug.LogError("I got stuck!"); }
    public void StuckPositionFailSafe() { transform.position = Controller.Instance.entrances[0].entranceNode.transform.position; }
    private void DaySchedule()
    {
        if (!shopping) { animator.SetBool("AtSite", false); animator.SetBool("Waiting", false); animator.SetBool("Searching", false); }
        AdjustPreferences();
        if (UIController.Instance.day == 1)
        {
            Controller.Instance.MoneyValueChange(Controller.Instance.customerMemberships, transform.position, true, false);
            money -= Controller.Instance.customerMemberships;
            storePreferance[0] -= Controller.Instance.customerMemberships;
        }

        money += (income * 5) * Controller.Instance.globalCustomerWageMultiplier;
        if (Controller.Instance.dayType == Controller.DayType.holiday) { money += income * 5; }
        if (moneyOwedToStore > 0)
        {
            moneyOwedToStore *= 1.02f;
            money -= (moneyOwedToStore / 10) + 10;
            Controller.Instance.MoneyValueChange((moneyOwedToStore / 10) + 10, transform.position, true, false);
            Controller.Instance.MoneyMadeByDebt += moneyOwedToStore / 10;
            moneyOwedToStore -= (moneyOwedToStore / 10) + 10;
        }

        //CheckSpecialItems();
        if (Random.Range(0,2) > 0 && myApplication == null) { applied = false; }
        ClampItemPrefs();
    }
    private void ClampItemPrefs()
    {
        foreach (KeyValuePair<string, List<float>> pair in ItemPreferences)
        {
            pair.Value[0] = Mathf.Clamp(pair.Value[0], 0, 110);
            pair.Value[1] = Mathf.Clamp(pair.Value[1], 0, 110);
            //make sure item is sellable too
        }
    }
    private bool ComeInTodayChance()
    {
        List<float> dayChances = new List<float> { 0, 100, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 90, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 95, 95, 95 };
        float chance = dayChances[UIController.Instance.day];
        //other variables like weekend and holidays and sales
        if (UIController.Instance.weekday == "Thursday") { chance += 5; }
        if (UIController.Instance.weekday == "Friday") { chance += 10; }
        if (Controller.Instance.dayType == Controller.DayType.holiday) { chance += 15; }

        float result = Random.Range(0, 100);

        if (result <= chance) { return true; }
        else { return false; }
    }
    private void ChooseStore()
    {
        float previousNumber = 0;
        float largestNumber = 0;
        List<float> storeChance = new List<float>();
        for (int i = 0; i < storePreferance.Count; i++)
        {
            storeChance.Add(storePreferance[i] + largestNumber);
            previousNumber = storePreferance[i];
            largestNumber = storePreferance[i] + largestNumber;
        }

        float number = Random.Range(0, largestNumber);
        float closestNumber = 1000;
        int targetStore = 0;
        for (int i = 0; i < storeChance.Count; i++)
        {
            //if (number < storeChance[i] && storeChance[i] - number < closestNumber) 
            if (i != 0)
            {
                if (number < storeChance[i] && number > storeChance[i - 1])
                { closestNumber = storeChance[i] - number; targetStore = i; }
            }
            else { targetStore = i; }
            //if (time < number && number - time < closestNumber) { closestNumber = time - number; newNumber = time; }
        }


        if (targetStore == 0)
        {
            //storeOpinion = 100;
            queuedComplaints.Clear();
            allComplaints.Clear(); allComplements.Clear();
            storeOpinion = storePreferance[0] * 2;
            storeOpinion += 30; //test?
            //activated = true;
            Controller.Instance.activeCustomers++;
            animator.SetBool("AtSite", true);
        }
        else
        {
            personVis.UpdateEmotion((((int)((storePreferance[targetStore] * 2) / 40) * -1) + 4));
            animator.SetTrigger("AtOtherStore");
            ShopAtOtherStore(targetStore);
        }
    }
    public void ShopAtOtherStore(int targetStore)
    {
        storeOpinion = storePreferance[targetStore] * 2;
        //generate List
        shoppingList.Clear();
        foreach (KeyValuePair<string, List<float>> pair in ItemPreferences)
        {
            if (pair.Value[0] > 50) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { if (item != special) { shoppingList.Add(item); } } } } // pair.Value[0] -= 10
            if (pair.Value[0] > 60) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { if (item != special) { shoppingList.Add(item); } } } } // pair.Value[0] -= 10
            if (pair.Value[0] > 70) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { if (item != special) { shoppingList.Add(item); } } } } // pair.Value[0] -= 10
            if (pair.Value[0] > 80) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { if (item != special) { shoppingList.Add(item); } } } } // pair.Value[0] -= 10
            if (pair.Value[0] > 90) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { if (item != special) { shoppingList.Add(item); } } } } // pair.Value[0] -= 10
            if (pair.Value[0] > 100) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { if (item != special) { shoppingList.Add(item); } } } } // pair.Value[0] -= 10
        }

        int boughtItems = 0;

        //shopping
        for (int i = 0; i < shoppingList.Count; i++)
        {
            if (Controller.Instance.competitors[targetStore - 1].itemAmounts.ContainsKey(shoppingList[i].myName))
            {
                if (Controller.Instance.competitors[targetStore - 1].itemAmounts[shoppingList[i].myName] > 0)
                {
                    boughtItems++;
                    storePreferance[targetStore] += storeImpact;
                    ItemPreferences[shoppingList[i].myName][0] -= 10;
                    Controller.Instance.competitors[targetStore - 1].BuyItem(shoppingList[i].itemID);
                    money -= shoppingList[i].value * Controller.Instance.competitors[targetStore - 1].priceMulti;
                    //what somp sells it for
                    storeOpinion += ((float)Controller.Instance.competitors[targetStore - 1].itemQuality / 100f) - 0.5f;
                    storeOpinion -= Controller.Instance.competitors[targetStore - 1].priceMulti - 1;
                }
            }
        }

        //after shopping
        if (shoppingList.Count > 0)
        {
            storePreferance[targetStore] -= (shoppingList.Count - boughtItems) * storeImpact * 2;
        }

        if (shoppingList.Count == 0) { storePreferance[targetStore] += storeImpact; }
        else { storePreferance[targetStore] -= storeImpact; }

        storeOpinion += Controller.Instance.competitors[targetStore - 1].storeBeauty;
        storeOpinion += Controller.Instance.competitors[targetStore - 1].storeCleanlyness;
        storePreferance[targetStore] += ((storeOpinion / 10) - 10) * storeImpact;
        if (!Controller.Instance.competitors[targetStore - 1].special) { storePreferance[targetStore] -= TransitionController.Instance.totalDifficulty; }
        if (storePreferance[targetStore] > 100) { storePreferance[targetStore] = 100; }
    }

    public void Absent()
    {
        //SwitchObjective(0);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        myCollider.enabled = false;
        myCollider2.enabled = false;
        /*
        if (animator.GetBool("LeaveForever"))
        {
            UnSubScribe();
            Destroy(this.gameObject, 0.1f);//revert to customer
        }
        */
        RemoveAllClaims();
        //animator.SetBool("AtSite", false);
        animator.SetBool("Waiting", false); animator.SetBool("Searching", false);

        if (animator.GetBool("AtSite"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
        else { activeTicks = 0; shopping = false; activated = false; }

        moodBar.SetActive(false);
        emotion.Disable();
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

    private void Tick20Delay(object sender, TickSystem.OnTickEventArgs e)
    {
        if (animator.GetBool("Dead")) { transform.GetChild(0).GetChild(9).GetComponent<Animation>()["Death"].speed = TickSystem.Instance.timeMultiplier; }
        if (special) { transform.GetChild(0).GetChild(10).GetComponent<Animation>()["Mad"].speed = TickSystem.Instance.timeMultiplier; }
        messageCalled = false;
    }
    public void FinishCheckingOut()
    {
        RemoveClaim();
        storePreferance[0] += storeImpact;
        if (shoppingList.Count == 0) { storePreferance[0] += storeImpact; }
        if (targetRegistor != null) { if (targetRegistor.employee != null) { StartInteraction(targetRegistor.employee); } }
        
        Leave();
    }
    public void Leave()
    {
        if (shoppingList.Count != 0) { storePreferance[0] -= (storeImpact * shoppingList.Count) * 1.75f; }
        storePreferance[0] += ((storeOpinion / 10) - 10) * storeImpact;
        storePreferance[0] += TransitionController.Instance.totalDifficulty;
        if (storePreferance[0] < 0) { storePreferance[0] = 0; }
        RemoveAllClaims();
        animator.SetBool("AtSite", false); animator.SetBool("Waiting", false); animator.SetBool("Searching", false);
        shopping = false;
        activeTicks = 0;
        activated = false;
        PostReview();
    }
    private void PostReview()
    {
        if (UIController.Instance.todaysReviews < UIController.Instance.maxCustomerReviews && Random.Range(0, 100) > 90)
        {
            int colorNumber = 0;
            string message = "";
            int emotion = 0;

            switch((((int)(storeOpinion / 40) * -1) + 4))
            {
                //happy -> mad
                case 0: emotion = 5; colorNumber = 4; break;
                case 1: emotion = 6; colorNumber = 0; break;
                case 2: emotion = 7; colorNumber = 0; break;
                case 3: emotion = 8; colorNumber = 3; break;
                case 4: emotion = 9; colorNumber = 1; break;
                case 5: emotion = 10; colorNumber = 1; break;
            }

            int max = 0;
            string selectedPreMessage = "";
            bool reversed = false;
            if (emotion < 7)
            {
                if (allComplements.Count > 0)
                {
                    foreach (KeyValuePair<string, int> pair in allComplements)
                    {
                        if (pair.Value > max)
                        {
                            selectedPreMessage = pair.Key;
                            max = pair.Value;
                        }
                    }
                }
                else if (allComplaints.Count > 0)
                {
                    max = 999999;
                    foreach (KeyValuePair<string, int> pair in allComplaints)
                    {
                        if (pair.Value < max)
                        {
                            reversed = true;
                            selectedPreMessage = pair.Key;
                            max = pair.Value;
                        }
                    }
                }
                else { return; }

            }
            else if (emotion >= 7)
            {
                if (allComplaints.Count > 0)
                {
                    foreach (KeyValuePair<string, int> pair in allComplaints)
                    {
                        if (pair.Value > max)
                        {
                            selectedPreMessage = pair.Key;
                            max = pair.Value;
                        }
                    }
                }
                else if (allComplements.Count > 0)
                {
                    max = 999999;
                    foreach (KeyValuePair<string, int> pair in allComplements)
                    {
                        if (pair.Value < max)
                        {
                            reversed = true;
                            selectedPreMessage = pair.Key;
                            max = pair.Value;
                        }
                    }
                }
                else { return; }
            }

            if (!reversed)
            {
                switch(selectedPreMessage)
                {
                    case "The floor is so dirty!": message = "The store was disgusting!"; break;
                    case "The floor is so clean!": message = "The store was so clean!"; break;
                    case "This place is ugly!": message = "The store was ugly."; break;
                    case "This place is so beautiful!": message = "The store was so beautiful!"; break;
                    case "It's so crowded in here!": message = "It took all day to get in and out!"; break;
                    case "Wow! This place is so empty!": message = "I was in and out in a breeze!"; break;
                    case "It's so hot!": message = "I was sweating so much!"; break;
                    case "It's too cold!": message = "I was freezing to death!"; break;
                    case "It feels great in here!": message = "The temperture of the store was refreshing!"; break;
                    case "This thing is falling apart!": message = "The items I bought were useless and ugly"; break;
                    case "This thing is amazing!": message = "This store only sells the best things!"; break;
                    case "Everything is so cheap!": message = "Everything was so cheap!"; break;
                    case "Everything is so expensive!": message = "Everything was too expensive"; break;
                    case "Everyone is so rude!": message = "The staff was so mean!"; break;
                    case "Everyone is so nice!": message = "The staff were very kind!"; break;
                }
            }
            else
            {
                switch (selectedPreMessage)
                {
                    case "The floor is so dirty!": message = "I wish they swept more often."; break;
                    case "The floor is so clean!": message = "Atleast the store was cleanish."; break;
                    case "This place is ugly!": message = "Please add some tile floors or somthing."; break;
                    case "This place is so beautiful!": message = "I did not like the store but I liked the look of it."; break;
                    case "It's so crowded in here!": message = "Too many people like shopping here."; break;
                    case "Wow! This place is so empty!": message = "I was the only one in the store!"; break;
                    case "It's so hot!": message = "It was too hot."; break;
                    case "It's too cold!": message = "It was too cold."; break;
                    case "It feels great in here!": message = "At least the temperature was just right."; break;
                    case "This thing is falling apart!": message = "I liked the store but the things they sell are aweful."; break;
                    case "This thing is amazing!": message = "The store sucked but I liked what they sold me."; break;
                    case "Everything is so cheap!": message = "At least everything was cheap."; break;
                    case "Everything is so expensive!": message = "I wish everything did not cost so much."; break;
                    case "Everyone is so rude!": message = "The employees should be nicer to thier customers."; break;
                    case "Everyone is so nice!": message = "Atleast the staff were nice to talk to."; break;
                }
            }
            UIController.Instance.CreateLog(colorNumber, message, myName + Localizer.Instance.GetLocalizedText(" customer"), emotion);
        }

    }

    private void LeaveForever()
    {
        /*
        RemoveAllClaims();
        UIController.Instance.CreateLog("I'm tired of waiting! I'm going somewhere else! - " + myName);
        UIController.Instance.UpdateCustomerStatus(2);
        animator.SetBool("LeaveForever", true);
        animator.SetBool("AtSite", false);
        //if (!activated) { Absent(); }
        */
    }

    public void RemoveAllClaims()//temp?
    {
        foreach (Building shelf in Controller.Instance.shelves) { if (shelf.customerQueue.Contains(this)) { shelf.customerQueue.Remove(this); shelf.allQueue.Remove(this.gameObject); } }
        //foreach (Building shelf in Controller.Instance.stockPiles) { if (shelf.employeeQueue.Contains(this)) { shelf.employeeQueue.Remove(this); shelf.allQueue.Remove(this.gameObject); } }
        foreach (Building registor in Controller.Instance.registers) { if (registor.customerQueue.Contains(this)) { registor.customerQueue.Remove(this); registor.allQueue.Remove(this.gameObject); } }
    }

    private bool walkingSoundEnabled;
    private bool walkingAuidoAdjusting;
    public void ToggleWalkingSounds(bool enable)
    {
        if (!enable) { walkingSoundEnabled = false; }
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

                List<NewGrid> path = MapController.Instance.FindPath(myX, myY, x, y, true, false);
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
    private void OnTimeSpeedChange(object sender, TickSystem.OnTickEventArgs e)
    {
        speedCalc = GetSpeed(GetGrid());
        animator.speed = TickSystem.Instance.timeMultiplier;
    }
    private void On10Tick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (shopping) 
        {
            if (GetGrid() != null)
            {
                //high values = bad
                if (GetGrid().GetCleanNormalized() >= 1) { storeOpinion -= 0.2f; }
                if (GetGrid().GetCleanNormalized() >= .75 && GetGrid().GetCleanNormalized() < 1) { storeOpinion -= 0.1f; }
                if (GetGrid().GetCleanNormalized() >= .60 && GetGrid().GetCleanNormalized() < .75) { storeOpinion -= 0.05f; }
                if (GetGrid().GetCleanNormalized() >= .50 && GetGrid().GetCleanNormalized() < .60) { storeOpinion += 0.05f; }
                if (GetGrid().GetCleanNormalized() <= .50 && GetGrid().GetCleanNormalized() > .25) { storeOpinion += 0.1f; }
                if (GetGrid().GetCleanNormalized() <= .25 && GetGrid().GetCleanNormalized() > 0) { storeOpinion += 0.2f; }
                if (GetGrid().GetCleanNormalized() <= 0) { storeOpinion += 0.3f; }
                if (GetGrid().GetCleanNormalized() >= 0.75) { QueueComplaint("The floor is so dirty!", null); }
                if (GetGrid().GetCleanNormalized() <= 0.25f && TransitionController.Instance.tutorialLevel > 2) { QueueComplaint(null, "The floor is so clean!"); }

                //beauty
                if (GetGrid().GetBeautyNormalized() >= 1) { storeOpinion += 0.3f; }
                if (GetGrid().GetBeautyNormalized() >= .75 && GetGrid().GetBeautyNormalized() < 1) { storeOpinion += 0.2f; }
                if (GetGrid().GetBeautyNormalized() >= .60 && GetGrid().GetBeautyNormalized() < .75) { storeOpinion += 0.1f; }
                if (GetGrid().GetBeautyNormalized() >= .50 && GetGrid().GetBeautyNormalized() < .60) { storeOpinion += 0.05f; }
                if (GetGrid().GetBeautyNormalized() <= .50 && GetGrid().GetBeautyNormalized() > .25) {  storeOpinion -= 0.05f; }
                if (GetGrid().GetBeautyNormalized() <= .25 && GetGrid().GetBeautyNormalized() > 0) {  storeOpinion -= 0.1f; }
                if (GetGrid().GetBeautyNormalized() <= 0) {  storeOpinion -= 0.2f; }
                if (GetGrid().GetBeautyNormalized() <= 0.25f) { QueueComplaint("This place is ugly!", null); }
                if (GetGrid().GetBeautyNormalized() >= 0.75f) { QueueComplaint(null, "This place is so beautiful!"); }

                //time
                if (activeTicks >= stayInStoreTimer * 4) { storeOpinion -= 0.2f; }
                if (activeTicks >= stayInStoreTimer * 2 && activeTicks < stayInStoreTimer * 4) { storeOpinion -= 0.1f; }
                if (activeTicks >= stayInStoreTimer && activeTicks < stayInStoreTimer * 3) { storeOpinion -= 0.05f; }
                if (activeTicks == stayInStoreTimer * 2) { storeOpinion += 0.05f; }
                if (activeTicks < stayInStoreTimer && activeTicks > stayInStoreTimer) { storeOpinion += 0.1f; }
                if (activeTicks <= stayInStoreTimer && activeTicks > 0) { storeOpinion += 0.2f; }
                if (activeTicks == 0) { storeOpinion += 0.3f; }

                //amount of customers
                int activeCustomers = Controller.Instance.activeCustomers;
                int customerCapacity = TransitionController.Instance.customerCapacity;
                if (activeCustomers >= customerCapacity * 2) { storeOpinion -= 0.2f; }
                if (activeCustomers >= customerCapacity * 1.5 && activeCustomers < customerCapacity * 2) { storeOpinion -= 0.1f; }
                if (activeCustomers > customerCapacity && activeCustomers < customerCapacity * 1.5) { storeOpinion -= 0.05f; }
                if (activeCustomers == customerCapacity) { storeOpinion += 0.05f; }
                if (activeCustomers < customerCapacity && activeCustomers > customerCapacity / 2) { storeOpinion += 0.1f; }
                if (activeCustomers <= customerCapacity / 2 && activeCustomers > 1) { storeOpinion += 0.2f; }
                if (activeCustomers == 1) { storeOpinion += 0.3f; }
                if (activeCustomers >= customerCapacity * 1.5f) { QueueComplaint("It's so crowded in here!", null); }
                if (activeCustomers <= customerCapacity / 2) { QueueComplaint(null, "Wow! This place is so empty!"); }

                //itemQuality

                //storeTemp
                float temp = Controller.Instance.insideTemp;
                if (temp >= tempPref * 1.25) { storeOpinion -= 0.4f; }
                if (temp >= 76 && temp < tempPref * 1.25) { storeOpinion -= 0.2f; }
                if (temp >= tempPref && temp < 76) { storeOpinion += 0.1f; }
                if (temp == tempPref) { storeOpinion += 0.3f; }
                if (temp < tempPref && temp > 68) { storeOpinion += 0.1f; }
                if (temp <= 68 && temp > tempPref * 0.75f) { storeOpinion -= 0.2f; }
                if (temp <= tempPref * 0.75f) { storeOpinion -= 0.4f;  }
                if (temp >= tempPref * 1.25) { QueueComplaint("It's so hot!", null); }
                if (temp <= tempPref * 0.75f) { QueueComplaint("It's too cold!", null); }
                if (temp == tempPref) { QueueComplaint(null, "It feels great in here!"); }

                Mathf.Clamp(storeOpinion, 0, 200);


                GenerateDirt();
                emotion.Activate((int)(storeOpinion / 2));
                //emotion

            }
            else
            {
                gameObject.transform.position = Controller.Instance.entrances[0].entranceNode.transform.position;
            }

        }

        if (activated) { personVis.UpdateEmotion((((int)(storeOpinion / 40) * -1) + 4)); }
    }
    public void ItemQuality(Item item)
    {
        int quality = item.quality;

        if (quality >= 100) { storeOpinion += 0.5f; }
        if (quality >= 75 && quality < 100) { storeOpinion += 0.3f; }
        if (quality >= 50 && quality < 75) {  storeOpinion += 0.2f; }
        if (quality == 50) { storeOpinion += 0.1f; }
        if (quality < 50 && quality > 25) { storeOpinion -= 0.1f; }
        if (quality <= 25 && quality > 0) { storeOpinion -= 0.2f; }
        if (quality <= 0) { storeOpinion -= 0.3f; }
        if (quality <= 25) { QueueComplaint("This thing is falling apart!", null); }
        if (quality >= 75) { QueueComplaint(null, "This thing is amazing!"); }

        float priceIncrease = item.value / item.itemSO.baseValue;

        if (priceIncrease >= greed * 2) { storeOpinion -= 1f; ItemPreferences[item.myName][0] -= priceIncrease * 10; ItemPreferences[item.myName][1] -= priceIncrease; }
        if (priceIncrease >= greed * 1.75f && priceIncrease < greed * 2) { storeOpinion -= 0.5f; ItemPreferences[item.myName][0] -= priceIncrease * 4; ItemPreferences[item.myName][1] -= priceIncrease * 0.3f; }
        if (priceIncrease >= greed * 1.5f && quality < greed * 1.75f) { storeOpinion -= 0.25f; ItemPreferences[item.myName][0] -= priceIncrease; ItemPreferences[item.myName][1] -= priceIncrease * 0.1f; }
        if (priceIncrease > greed * 0.75f && quality < greed * 1.5f) { storeOpinion += 0.1f; }
        if (priceIncrease > greed * 0.5f && quality <= greed * 0.75f) { storeOpinion += 0.4f; ItemPreferences[item.myName][0] += priceIncrease; ItemPreferences[item.myName][1] += priceIncrease * 0.1f; }
        if (priceIncrease <= greed * 0.5f) { storeOpinion += 0.8f; ItemPreferences[item.myName][0] += priceIncrease * 2; ItemPreferences[item.myName][1] += priceIncrease * 0.2f; }
        if (priceIncrease <= greed * 0.75f) { QueueComplaint("Everything is so cheap!", null); }
        if (priceIncrease >= greed * 1.75f) { QueueComplaint(null, "Everything is so expensive!"); }

        if (money >= item.value) { money -= item.value; }
        else if (Controller.Instance.storeCredit) { Controller.Instance.MoneyValueChange(-(item.value + 1), transform.position, true, true); moneyOwedToStore += item.value; }
        else { money -= item.value; }
    }
    private void GenerateDirt()
    {
        if (TransitionController.Instance.tutorialLevel > 2) 
        {
            int playableGridStart = TransitionController.Instance.playablegridstart;
            int playableGridSize = TransitionController.Instance.playablegridsize;
            GetMyGridPosXY(out int x, out int y);

            if ((x >= playableGridStart && x <= playableGridSize + playableGridStart) && y >= playableGridStart && y <= playableGridSize + playableGridStart) { GetGrid().IncreaseCleanValue(0.25f); }
        }
    }
    public void TalkBubble(string message, int type, int color)
    {
        //UtilsClass.CreateWorldTextPopup("This isn't the job I was hired to do!", this.gameObject.transform.position, Color.red);
        bubble.NewMessage(message, type, color);
    }
    public void StartInteraction(Employee2 employee)
    {
        float number = Random.Range(0, 50f);
        number += socialSkills;
        employee.CustomerInteraction(this, number);
    }
    public void ContinueInteraction(float value)
    {
        int randomTalk = Random.Range(0, 3);
        if (value <= 50) {
            switch(randomTalk)
            {
                default: TalkBubble("Eat ****", 0, 3); break;
                case 1: TalkBubble("How rude!", 0, 3); break;
                case 2: TalkBubble("Piss off!", 0, 3); break;
            }
        }
        else if (value > 75 && value < 100) {
            switch (randomTalk)
            {
                default: TalkBubble("You too", 0, 0); break;
                case 1: TalkBubble("Okay", 0, 0); break;
                case 2: TalkBubble("Alright", 0, 0); break;
            }
        }
        else if (value >= 100) {
            switch (randomTalk)
            {
                default: TalkBubble("Thank you! You as well!!", 0, 1); break;
                case 1: TalkBubble("Sure thing!", 0, 1); break;
                case 2: TalkBubble("Thanks!", 0, 1); break;
            }
        }

        if (value < 25) { storeOpinion -= 10f; }
        if (value >= 25 && value < 50) { storeOpinion -= 2.5f; }
        if (value >= 50 && value < 75) { storeOpinion += 1f; }
        if (value >= 75 && value < 100) { storeOpinion += 2.5f; }
        if (value >= 100) { storeOpinion += 10f; }

        if (value < 33) { QueueComplaint("Everyone is so rude!", null); }
        if (value > 66) { QueueComplaint(null, "Everyone is so nice!"); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Random.Range(0, 100) >= 95 && collision.transform.GetComponent<Employee2>() != null) { StartInteraction(collision.transform.GetComponent<Employee2>()); }
    }
    public Vector3 StandNextToMe(Employee2 manager, Officer officer)
    {
        Vector3 thisTargetPos = new Vector3(0, 0, 0);
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
        else
        {
            if (manager != null) { return manager.transform.position; }
            else if (officer != null) { return officer.transform.position; }
            else { return Vector3.zero; }
        }
    }
    public void ForcedEntry()
    {
        stayInStoreTimer = 50;
        money = 1000000;
        storeOpinion = 0;
        foreach (KeyValuePair<string, List<float>> pair in ItemPreferences)
        {
            pair.Value[0] = 100;
        }
        activated = true;
        Controller.Instance.activeCustomers++;
        Controller.Instance.MoneyValueChange(Controller.Instance.customerEntry, transform.position, true, false);
        money -= Controller.Instance.customerEntry;
        storeOpinion -= (Controller.Instance.customerEntry * 10);
        transform.GetChild(0).GetChild(10).gameObject.SetActive(true);
        animator.SetBool("AtSite", true);
        animator.speed = TickSystem.Instance.timeMultiplier;
        personVis.UpdateEmotion((((int)(storeOpinion / 40) * -1) + 4));
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
    }
    /*
    private void CheckSpecialItems()
    {
        foreach (ItemSO item in Controller.Instance.items)
        {
            //if it has been removed
            if (!Controller.Instance.unlockedSpecialItems.Contains(item) && ItemPreferences.ContainsKey(item.myName) && item.special)
            {
                int amountInStock = 0;
                foreach (Item allItem in FindObjectsOfType<Item>()) { if (allItem.myName == item.myName) { amountInStock++; } }
                //if player has none
                if (amountInStock <= 0)
                {
                    ItemPreferences.Remove(item.myName);
                }
            }
        }

    }*/

    private int stockingSkill = -1;
    private int serviceSkill;
    private int cleaningSkill;
    private int buildingSkill;
    private int managerSkill;
    private bool applied;
    private bool applying;
    private int times;
    private StaffApplicant myApplication;

    public void OutSkills(out int stockingSkill, out int serviceSkill, out int cleaningSkill, out int buildingSkill, out int managerSkill) { stockingSkill = this.stockingSkill; serviceSkill = this.serviceSkill; cleaningSkill = this.cleaningSkill; buildingSkill = this.buildingSkill; managerSkill = this.managerSkill; }

    private void SetUpSkills()
    {
        switch(times)
        {
            case 0:
                if (Random.Range(0, 100) > 30 && stockingSkill < 20) { stockingSkill++; SetUpSkills(); return; }
                else { times++; SetUpSkills(); return; }
            case 1:
                if (Random.Range(0, 100) > 25 && serviceSkill < 20) { serviceSkill++; SetUpSkills(); return; }
                else { times++; SetUpSkills(); return; }
            case 2:
                if (Random.Range(0, 100) > 35 && cleaningSkill < 20) { cleaningSkill++; SetUpSkills(); return; }
                else { times++; SetUpSkills(); return; }
            case 3:
                if (Random.Range(0, 100) > 40 && buildingSkill < 20) { buildingSkill++; SetUpSkills(); return; }
                else { times++; SetUpSkills(); return; }
            case 4:
                if (Random.Range(0, 100) > 45 && managerSkill < 20) { managerSkill++; SetUpSkills(); return; }
                else { times++; break; }
        }
    }
    private void Apply()
    {
        if (UIController.Instance.isHiring && !applyBanned)
        {
            applied = true;
            applying = true;

            myApplication = UIController.Instance.NewApplicent(this);
        }
    }


    /// //Build   --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public static Customer2 Create(CustomerSO customerSO, float age, string birthName, bool loaded, bool female)
    {
        Vector3 startingPos = new Vector3(0,0,0);
        //if (Controller.Instance.entrances.Count > 0) { startingPos = Controller.Instance.entrances[0].position; }
        Transform unitTransform = Instantiate(customerSO.prefab, startingPos, Quaternion.Euler(0, 0, 0));

        Customer2 customer = unitTransform.GetComponent<Customer2>();
        customer.Setup(customerSO, age, birthName, loaded, female);

        return customer;
    }

    private CustomerSO customerSO;

    private void Setup(CustomerSO customerSO, float age, string birthName, bool loaded, bool female)
    {
        this.customerSO = customerSO;
        this.age = age;
        this.birthName = birthName;
        this.myName = birthName;
        gameObject.name = myName + " Customer";
        this.isFemale = female;
        //this.myName = customerSO.name;
        //Debug.Log(Controller.Instance.entrances.Count);
        //if (startPos == new Vector3(0,0,0)) { transform.position = Controller.Instance.entrances[0].position; }
        GetTransforms();
        OnMouseExit();
        DetermineShoppingTimes();
        SetUpPreferences();
        AssignTraits();
        if (!loaded) { transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().SetSprites(-1, 0, 0, 0, 0, 0, 0, isFemale, null); }
        if (animator.GetBool("AtSite")) { activated = true; shopping = true; animator.SetBool("Waiting", true); animator.SetBool("Searching", true); }
        Invoke("LoadDelay", 1);
        Subscribe();
    }
    private void LoadDelay() { if (stockingSkill == -1) { stockingSkill = 0; SetUpSkills(); } if (applying) { Apply(); } }

    private void GetTransforms()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        myCollider = colliders[0];
        myCollider2 = colliders[1];
        visuals = transform.GetChild(0);
        container = transform.GetChild(0).GetChild(3);

        moodBar = transform.GetChild(1).GetChild(0).gameObject;
        moodBarCon = moodBar.GetComponent<BarController>();

        emotion = transform.GetChild(1).GetChild(3).GetComponent<Emotion>();
        personVis = transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>();
        bubble = transform.GetChild(1).GetChild(4).GetComponent<ChatMessage>();
    }

    private void Subscribe()
    {
        UIController.Instance.OnTimeValueChanged += TimeSchedule;//needs come back at random chance
                                                                 //UIController.Instance.OnDayValueChanged += DaySchedule;//needs come back at random chance
        TickSystem.Instance.On10Tick += On10Tick;
        TickSystem.Instance.OnAdjusted50Tick += Tick20Delay;
        Controller.Instance.customers.Add(this);
        TickSystem.Instance.OnSpeedChange += OnTimeSpeedChange;
    }

    private void UnSubScribe()
    {
        UIController.Instance.OnTimeValueChanged -= TimeSchedule;//needs come back at random chance
                                                                 //UIController.Instance.OnDayValueChanged -= DaySchedule;//needs come back at random chance
        TickSystem.Instance.On10Tick -= On10Tick;
        TickSystem.Instance.OnAdjusted50Tick -= Tick20Delay;
        Controller.Instance.customers.Remove(this);
        TickSystem.Instance.OnSpeedChange -= OnTimeSpeedChange;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
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
        //appearance         public PersonVisualCon personVis;

        public Vector3 position;

        public string myName;
        public string birthName;
        public float age;
        public bool isFemale;

        public float baseSpeed;
        public int workSpeed;

        public int timeTicks;
        public int activeTicks;
        public int shoppingTime = -1;
        public int shoppingMinute = -1;

        public float money;
        public float income;
        public float moneyOwedToStore;
        public bool special;

        //triats
        public float greed;
        public float socialSkills;
        public float scheduleStrictness;
        public float auditorySensitivity;
        public float callOutChance;
        public float needyModifier;
        public float tempPref;
        public int stayInStoreTimer;
        public float storeImpact;

        public List<string> shoppingDaysName = new List<string>();
        public List<bool> shoppingDaysBool = new List<bool>();

        public List<string> itemPreferencesName = new List<string>();
        public List<float> itemPreferencesValue = new List<float>();
        public List<float> itemPreferencesGrowth = new List<float>();

        public List<float> storePreferance = new List<float>();

        public List<string> traitNames = new List<string>();
        public List<bool> traitValues = new List<bool>();

        public List<float> characterSizes = new List<float>();
        public List<int> appearance = new List<int>();

        public bool atSite;

        public int stockingSkill;
        public int serviceSkill;
        public int cleaningSkill;
        public int buildingSkill;
        public int managerSkill;
        public bool applied;
        public bool applying;
        public bool banned_From_Applying;
        public bool insideStore;
    }

    public SaveObject Save()
    {
        List<string> shoppingDayNames = new List<string>();
        List<bool> shoppingDaysBools = new List<bool>();
        foreach (KeyValuePair<string, bool> pair in ShoppingDays)
        {
            shoppingDayNames.Add(pair.Key);
            shoppingDaysBools.Add(pair.Value);
        }
        List<string> itemPreferencesNames = new List<string>();
        List<float> itemPreferences1s = new List<float>();
        List<float> itemPreferences2s = new List<float>();
        foreach (KeyValuePair<string, List<float>> pair in ItemPreferences)
        {
            itemPreferencesNames.Add(pair.Key);
            itemPreferences1s.Add(pair.Value[0]);
            itemPreferences2s.Add(pair.Value[1]);
        }
        List<string> traitNames = new List<string>();
        List<bool> traitValues = new List<bool>();
        foreach (KeyValuePair<string, bool> pair in traits)
        {
            traitNames.Add(pair.Key);
            traitValues.Add(pair.Value);
        }
        return new SaveObject
        {
            position = this.transform.position,
            myName = this.myName,
            birthName = this.birthName,
            age = this.age,
            isFemale = this.isFemale,

            baseSpeed = this.baseSpeed,
            workSpeed = this.workSpeed,

            timeTicks = this.timeTicks,
            activeTicks = this.activeTicks,
            shoppingTime = this.shoppingTime,
            shoppingMinute = this.shoppingMinute,

            money = this.money,
            income = this.income,
            moneyOwedToStore = this.moneyOwedToStore,
            special = this.special,

            greed = this.greed,
            socialSkills = this.socialSkills,
            scheduleStrictness = this.scheduleStrictness,
            auditorySensitivity = this.auditorySensitivity,
            callOutChance = this.callOutChance,
            needyModifier = this.needyModifier,
            tempPref = this.tempPref,
            stayInStoreTimer = this.stayInStoreTimer,
            storeImpact = this.storeImpact,

            shoppingDaysName = shoppingDayNames,
            shoppingDaysBool = shoppingDaysBools,

            itemPreferencesName = itemPreferencesNames,
            itemPreferencesValue = itemPreferences1s,
            itemPreferencesGrowth = itemPreferences2s,

            storePreferance = this.storePreferance,

            traitNames = traitNames,
            traitValues = traitValues,

            characterSizes = this.personVis.sizes,
            appearance = this.personVis.set,

            atSite = animator.GetBool("AtSite"),

            stockingSkill = this.stockingSkill,
            serviceSkill = this.serviceSkill,
            cleaningSkill = this.cleaningSkill,
            buildingSkill = this.buildingSkill,
            managerSkill = this.managerSkill,
            applied = this.applied,
            applying = this.applying,
            banned_From_Applying = this.applyBanned,
            insideStore = this.insideStore,
        };
    }

    public void Load(SaveObject saveObject, CustomerSO customerSO)
    {
        if (saveObject.special) { return; }
        Customer2 placedCustomer = Customer2.Create(customerSO, saveObject.age, saveObject.birthName, true, saveObject.isFemale);
        placedCustomer.SetApperance(saveObject.appearance, saveObject.characterSizes);

        placedCustomer.transform.position = saveObject.position; 
        placedCustomer.startPos = saveObject.position;

        placedCustomer.myName = saveObject.myName;
        placedCustomer.birthName = saveObject.birthName;
        placedCustomer.age = saveObject.age;
        placedCustomer.isFemale = saveObject.isFemale;

        placedCustomer.baseSpeed = saveObject.baseSpeed;
        placedCustomer.workSpeed = saveObject.workSpeed;

        placedCustomer.timeTicks = saveObject.timeTicks;
        placedCustomer.activeTicks = saveObject.activeTicks;
        placedCustomer.shoppingTime = saveObject.shoppingTime;
        placedCustomer.shoppingMinute = saveObject.shoppingMinute;

        placedCustomer.money = saveObject.money;
        placedCustomer.income = saveObject.income;
        placedCustomer.moneyOwedToStore = saveObject.moneyOwedToStore;

        placedCustomer.greed = saveObject.greed;
        placedCustomer.socialSkills = saveObject.socialSkills;
        placedCustomer.scheduleStrictness = saveObject.scheduleStrictness;
        placedCustomer.auditorySensitivity = saveObject.auditorySensitivity;
        placedCustomer.callOutChance = saveObject.callOutChance;
        placedCustomer.needyModifier = saveObject.needyModifier;
        placedCustomer.tempPref = saveObject.tempPref;
        placedCustomer.stayInStoreTimer = saveObject.stayInStoreTimer;
        placedCustomer.storeImpact = saveObject.storeImpact;

        placedCustomer.stockingSkill = saveObject.stockingSkill;
        placedCustomer.serviceSkill = saveObject.serviceSkill;
        placedCustomer.cleaningSkill = saveObject.cleaningSkill;
        placedCustomer.buildingSkill = saveObject.buildingSkill;
        placedCustomer.managerSkill = saveObject.managerSkill;
        placedCustomer.applied = saveObject.applied;
        placedCustomer.applying = saveObject.applying;
        placedCustomer.applyBanned = saveObject.banned_From_Applying;
        placedCustomer.insideStore = saveObject.insideStore;

        placedCustomer.ShoppingDays.Clear();
        for (int i = 0; i < saveObject.shoppingDaysBool.Count; i++)
        {
            placedCustomer.ShoppingDays.Add(saveObject.shoppingDaysName[i], saveObject.shoppingDaysBool[i]);
        }

        placedCustomer.ItemPreferences.Clear();
        for (int i = 0; i < saveObject.itemPreferencesName.Count; i++)
        {
            List<float> itemPreferencess = new List<float>();
            itemPreferencess.Add(saveObject.itemPreferencesValue[i]);
            itemPreferencess.Add(saveObject.itemPreferencesGrowth[i]);
            placedCustomer.ItemPreferences.Add(saveObject.itemPreferencesName[i], itemPreferencess);
        }

        placedCustomer.storePreferance.Clear(); placedCustomer.storePreferance = saveObject.storePreferance;

        placedCustomer.traits.Clear();
        for (int i = 0; i < saveObject.traitNames.Count; i++)
        {
            placedCustomer.traits.Add(saveObject.traitNames[i], saveObject.traitValues[i]);
        }

        //placedCustomer.Setup(customerSO, saveObject.age, saveObject.birthName);
        placedCustomer.animator.SetBool("AtSite", saveObject.atSite);
    }
    public void SetApperance(List<int> appearance, List<float> sizes) { transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().SetSprites(appearance[0], appearance[1], appearance[2], appearance[3], appearance[4], appearance[5], appearance[6], isFemale, sizes); }
    private void SetUpPreferences()
    {
        needyModifier = Controller.Instance.c_needy;
        needyModifier += Random.Range(-0.75f, 1.25f);//1 average
        foreach (ItemSO item in Controller.Instance.items)
        {

            //Need / Growth / StorePreferance
            //float need = Random.Range(0, 100);
            //test
            float need = Random.Range(0, 100) * needyModifier;
            float needGrowth = (item.defaultNeedGrowth + Random.Range(-item.defaultNeedGrowth, item.defaultNeedGrowth * 1.5f)) * needyModifier;
            needGrowth = Mathf.Clamp(needGrowth, -100, 100);
            //float storePreferance = Random.Range(0, Controller.Instance.numberOfStores);

            //seasonal
            if (item.seasonal)
            {
                if (UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[0] || UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[3])
                {
                    if (!item.seasons.Contains("Winter")) { needGrowth /= 10; need = 0; }
                }
                else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[0] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[1])
                {
                    if (!item.seasons.Contains("Spring")) { needGrowth /= 10; need = 0; }
                }
                else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[1] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[2])
                {
                    if (!item.seasons.Contains("Summer")) { needGrowth /= 10; need = 0; }
                }
                else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[2] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[3])
                {
                    if (!item.seasons.Contains("Fall")) { needGrowth /= 10; need = 0; }
                }

            }

            if (item.inDemandSeason != "All")
            {
                if (UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[0] || UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[3])
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = 1; break;
                        case "Summer": modifier = -10; break;
                        case "Fall": modifier = 1; break;
                        case "Winter": modifier = 10; break;
                    }

                    if (modifier > 0) { needGrowth *= modifier; }
                    else { needGrowth /= -modifier; }
                }
                else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[0] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[1])
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = 10; break;
                        case "Summer": modifier = 1; break;
                        case "Fall": modifier = -10; break;
                        case "Winter": modifier = 1; break;
                    }

                    if (modifier > 0) { needGrowth *= modifier; }
                    else { needGrowth /= -modifier; }
                }
                else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[1] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[2])
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = 1; break;
                        case "Summer": modifier = 10; break;
                        case "Fall": modifier = 1; break;
                        case "Winter": modifier = -10; break;
                    }

                    if (modifier > 0) { needGrowth *= modifier; }
                    else { needGrowth /= -modifier; }
                }
                else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[2] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[3])
                {
                    float modifier = 1;
                    switch (item.inDemandSeason)
                    {
                        case "Spring": modifier = -10; break;
                        case "Summer": modifier = 1; break;
                        case "Fall": modifier = 10; break;
                        case "Winter": modifier = 1; break;
                    }

                    if (modifier > 0) { needGrowth *= modifier; }
                    else { needGrowth /= -modifier; }
                }
            }

            if (item.special)
            {
                if (!Controller.Instance.unlockedSpecialItems.Contains(item)) { need = 0; needGrowth = 0; }
            }

            if (item.year_Start > UIController.Instance.year)
            {
                need = 0;
                needGrowth = 0;
            }
            if (item.year_Start == UIController.Instance.year)
            {
                need *= 2;
                needGrowth *= 5;
            }

            if (UIController.Instance.year > item.year_End)
            {
                need = 0;
                needGrowth = 0;
            }
            List<float> list = new List<float> { need, needGrowth };
            if (!ItemPreferences.ContainsKey(item.myName)) { ItemPreferences.Add(item.myName, list); }
        }
    }

    private void AdjustPreferences()
    {
        foreach (KeyValuePair<string, List<float>> pair in ItemPreferences)
        {
            //seasonal
            pair.Value[0] += pair.Value[1];
            if (pair.Value[0] > 110) { pair.Value[0] = 110; }
        }
    }


    private void DetermineShoppingTimes()
    {
        /*
        float sunday = 90;
        float monday = 80;
        float tuesday = 70;
        float wensday = 65;
        float thursday = 75;
        float friday = 85;
        float saturday = 95;
        */
        scheduleStrictness = Random.Range(2.5f, 7.5f);
        //List<float> dayChances = new List<float> { 95, 80, 75, 70, 85, 90, 100};
        List<float> dayChances = new List<float> { (100 - scheduleStrictness), (100 - scheduleStrictness * 4), (100 - scheduleStrictness * 5), (100 - scheduleStrictness * 6), (100 - scheduleStrictness * 3), (100 - scheduleStrictness * 2), 100 };
        //List<float> timeChances = new List<float> { 10, 10, 10, 10, 20, 20, 30, 30, 40, 40, 50, 50, 80, 80, 80, 80, 80, 60, 60, 40, 40, 30, 30, 20 };
        List<int> weightedTimeChances = new List<int> { 0, 1, 2, 3, 5, 7, 10, 13, 17, 21, 26, 31, 39, 48, 56, 64, 72, 78, 84, 88, 92, 95, 98, 100 };

        if (ShoppingDays.Count == 0)
        {
            for (int i = 0; i < dayChances.Count; i++)
            {
                bool boolen = false;
                float number = Random.Range(0, 100);
                if (number < dayChances[i]) { boolen = true; }
                else { boolen = false; }
                switch(i)
                {
                    case 0: ShoppingDays.Add("Sunday", boolen); break;
                    case 1: ShoppingDays.Add("Monday", boolen); break;
                    case 2: ShoppingDays.Add("Tuesday", boolen); break;
                    case 3: ShoppingDays.Add("Wednesday", boolen); break;
                    case 4: ShoppingDays.Add("Thursday", boolen); break;
                    case 5: ShoppingDays.Add("Friday", boolen); break;
                    case 6: ShoppingDays.Add("Saturday", boolen); break;
                }
                
            }
        }

        if (shoppingTime == -1)
        {
            float number = Random.Range(-1f, 100f);
            float closestNumber = 1000;
            int newNumber = 0;

            foreach (int time in weightedTimeChances)
            {
                if (time > number && time - number < closestNumber) { closestNumber = time - number; newNumber = time; }
                switch (newNumber)
                {
                    case 0: shoppingTime = 0; break;
                    case 1: shoppingTime = 1; break;
                    case 2: shoppingTime = 2; break;
                    case 3: shoppingTime = 3; break;
                    case 5: shoppingTime = 4; break;
                    case 7: shoppingTime = 5; break;
                    case 10: shoppingTime = 6; break;
                    case 13: shoppingTime = 7; break;
                    case 17: shoppingTime = 8; break;
                    case 21: shoppingTime = 9; break;
                    case 26: shoppingTime = 10; break;
                    case 31: shoppingTime = 11; break;
                    case 39: shoppingTime = 12; break;
                    case 48: shoppingTime = 13; break;
                    case 56: shoppingTime = 14; break;
                    case 64: shoppingTime = 15; break;
                    case 72: shoppingTime = 16; break;
                    case 78: shoppingTime = 17; break;
                    case 84: shoppingTime = 18; break;
                    case 88: shoppingTime = 19; break;
                    case 92: shoppingTime = 20; break;
                    case 95: shoppingTime = 21; break;
                    case 98: shoppingTime = 22; break;
                    case 100: shoppingTime = 23; break;
                }
            }

            int rando = Random.Range(0, 4);

            switch(rando)
            {
                case 0: shoppingMinute = 0; break;
                case 1: shoppingMinute = 15; break;
                case 2: shoppingMinute = 30; break;
                case 3: shoppingMinute = 45; break;
                case 4: shoppingMinute = 0; break;
            }
        }

        if (storePreferance.Count == 0)
        {
            for (int i = 0; i < Controller.Instance.competitors.Count + 1; i++)
            {
                float number = Random.Range(0, 100);
                if (i == 0) { number -= 30; }
                if (number < 0) { number = 0; }
                if (i != 0) { if (Controller.Instance.competitors[i - 1].bankrupt) { number = -100; } }
                storePreferance.Add(number);
            }
        }
    }

    private void AssignTraits()
    {
        if (traits.Count == 0)
        {
            //severe weather
            bool boolen = false;
            if (Random.Range(0, 100) > 75) { boolen = true; } else { boolen = false; }
            traits.Add("weather_fearful", boolen);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("weather_lover", boolen);

            //shopping times
            if (shoppingTime < 7) { boolen = true; } else { boolen = false; }
            traits.Add("early_bird", boolen);
            if (shoppingTime > 17) { boolen = true; } else { boolen = false; }
            traits.Add("night_owl", boolen);

            //holidays
            if (Random.Range(0, 100) > 60) { boolen = true; } else { boolen = false; }
            traits.Add("procrastinator", boolen);
            traits.Add("workaholic", boolen);
            if (Random.Range(0, 100) > 80 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("vacationer", boolen);

            //Time customer will stay in store for
            stayInStoreTimer = (int)Controller.Instance.c_inStoreTime;
            stayInStoreTimer += Random.Range(-12, 12);//24 average
            if (stayInStoreTimer < 24) { boolen = true; } else { boolen = false; }
            traits.Add("stressful", boolen);
            if (stayInStoreTimer > 36) { boolen = true; } else { boolen = false; }
            traits.Add("stressless", boolen);

            //How great everything impacts this customer
            storeImpact = Controller.Instance.c_baseLoaylty;
            storeImpact += Random.Range(-0.5f, 0.5f);//24 average
            if (storeImpact < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("loyal", boolen);
            if (storeImpact > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("Unfaithful", boolen);

            //How great everything impacts this customer
            float number = Random.Range(0.5f, 1.5f);//24 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_learner", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_learner", boolen);

            //how strict the customers schedule is
            if (scheduleStrictness < 3.75f) { boolen = true; } else { boolen = false; }
            traits.Add("free", boolen);
            if (scheduleStrictness > 6.25f) { boolen = true; } else { boolen = false; }
            traits.Add("busy", boolen);

            //how much stress is reduced during time off
            number = Random.Range(0.5f, 1.5f);//1 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("burnt_out", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("work_lover", boolen);

            //walking speed
            baseSpeed = Controller.Instance.c_baseWalkSpeed;
            baseSpeed += Random.Range(-10, 10);//40 average
            if (baseSpeed < -5) { boolen = true; } else { boolen = false; }
            traits.Add("slow_walker", boolen);
            if (baseSpeed > 5) { boolen = true; } else { boolen = false; }
            traits.Add("fast_walker", boolen);

            //age
            if (age < 21) { boolen = true; } else { boolen = false; }
            traits.Add("young", boolen);
            if (age > 65) { boolen = true; } else { boolen = false; }
            traits.Add("old", boolen);

            //music effect
            auditorySensitivity = Controller.Instance.c_baseAudio;
            auditorySensitivity += Random.Range(-0.5f, 0.5f);//1 average
            if (auditorySensitivity < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("deaf", boolen);
            if (auditorySensitivity > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("sensitive", boolen);

            //calling out chance //chance of not coming in
            callOutChance = Random.Range(0.01f, 10f);//5 average
            if (callOutChance < 2.5f) { boolen = true; } else { boolen = false; }
            traits.Add("reliable", boolen);
            if (callOutChance > 7.5f) { boolen = true; } else { boolen = false; }
            traits.Add("unreliable", boolen);

            //needy //item reduction and frequency
            if (needyModifier < 0.6f) { boolen = true; } else { boolen = false; }
            traits.Add("frugal", boolen);
            if (needyModifier > 1.5f) { boolen = true; } else { boolen = false; }
            traits.Add("needy", boolen);

            //temp preferance
            tempPref = Random.Range(68f, 76f);//72 average
            if (tempPref < 70f) { boolen = true; } else { boolen = false; }
            traits.Add("Cold_Lover", boolen);
            if (tempPref > 74f) { boolen = true; } else { boolen = false; }
            traits.Add("Heat_lover", boolen);

            //workspeed
            workSpeed = (int)Controller.Instance.c_baseWorkSpeed;
            workSpeed += Random.Range(-2, 2);//3 average
            if (workSpeed <= 2) { boolen = true; } else { boolen = false; }
            traits.Add("fast_worker", boolen);
            if (workSpeed > 4) { boolen = true; } else { boolen = false; }
            traits.Add("slow_worker", boolen);

            //money
            //income = Controller.Instance.money;
            income = TransitionController.Instance.averageCustomerHourlyIncome;
            //income = 10.5f;//income from region
            income += Random.Range(income * -0.25f, income);//3 average
            if (income <= TransitionController.Instance.averageCustomerHourlyIncome / 2) { boolen = true; } else { boolen = false; }
            traits.Add("Poor", boolen);
            if (income > TransitionController.Instance.averageCustomerHourlyIncome * 1.5f) { boolen = true; } else { boolen = false; }
            traits.Add("Rich", boolen);

            //greed
            greed = Controller.Instance.c_greed;
            greed += Random.Range(-0.9f, 1f);//3 average
            if (greed <= 0.5f) { boolen = true; } else { boolen = false; }
            traits.Add("Stingy", boolen);
            if (greed > 1.5f) { boolen = true; } else { boolen = false; }
            traits.Add("Spendful", boolen);

            //wage preferance
            if (Random.Range(0, 100) > 75) { boolen = true; } else { boolen = false; }
            traits.Add("Cheap", boolen);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("Expensive", boolen);

            //social Skills
            socialSkills = Controller.Instance.c_baseSocial;
            socialSkills += Random.Range(-5f, 5f);//5 average
            if (socialSkills < 3f) { boolen = true; } else { boolen = false; }
            traits.Add("Introvert", boolen);
            if (socialSkills > 7f) { boolen = true; } else { boolen = false; }
            traits.Add("Extrovert", boolen);

            if (Random.Range(0, 100) > 95) { traits.Add("Pyromaniac", true); } else { traits.Add("Pyromaniac", false); }
            if (Random.Range(0, 100) > 95) { traits.Add("Clumsy", true); } else { traits.Add("Clumsy", false); }
            if (Random.Range(0, 100) > 95) { traits.Add("Lunatic", true); } else { traits.Add("Lunatic", false); }
            if (Random.Range(0, 100) > 95) { traits.Add("Angry", true); } else { traits.Add("Angry", false); }
        }
    }

    private void QueueComplaint(string compliant, string  complement)
    {
        if (compliant != null)
        {
            if (queuedComplaints.ContainsKey(compliant))
            {
                queuedComplaints[compliant] += Random.Range(0,10);
                if (queuedComplaints[compliant] >= 200)
                {
                    //UtilsClass.CreateWorldTextPopup(compliant, transform.position, Color.red);
                    TalkBubble(compliant, 1, 2);
                    queuedComplaints.Remove(compliant);
                }
            }
            else { queuedComplaints.Add(compliant, 1); }

            if (allComplaints.ContainsKey(compliant)) { allComplaints[compliant] += Random.Range(0, 10); }
            else { allComplaints.Add(compliant, 1); }
        }

        if (complement != null)
        {
            if (queuedComplaints.ContainsKey(complement))
            {
                queuedComplaints[complement] += Random.Range(0, 10);
                if (queuedComplaints[complement] >= 400)
                {
                    //UtilsClass.CreateWorldTextPopup(complement, transform.position, Color.green);
                    TalkBubble(complement, 1, 1);
                    queuedComplaints.Remove(complement);
                }
            }
            else { queuedComplaints.Add(complement, 1); }

            if (allComplements.ContainsKey(complement)) { allComplements[complement] += Random.Range(0, 10); }
            else { allComplements.Add(complement, 1); }
        }
    }
    public void DestroyMe()
    {
        animator.SetBool("Waiting", false);
        animator.SetBool("AtSite", false);
        RemoveAllClaims();
        UnSubScribe();
        Destroy(this.gameObject, 1f);
    }

    public bool dueForDeletion;
}
