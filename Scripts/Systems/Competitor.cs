using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Competitor : MonoBehaviour
{
    //starting money
    //employee pay
    //item quantity
    //store cleanlyness + beauty + item quality
    //item preferance
    //main seller
    public enum CompetitorType
    {
        Average,
        Bulk,
        Quality,
        Discount
        //__goods store
    }
    public CompetitorType type;

    //bills
    //---electricty // per item
    //---wages //employees
    //---rent //entire square footage
    //---items //buying items

    public float money;
    public float debt;
    public float debtMax;
    [SerializeField] public float employeePay = 7.25f;
    public int numOfEmployees = 1;//employees handle up to 10 of each item
    public int itemQuality;
    [SerializeField] public int orderAmounts;
    public int storeCleanlyness;
    public int storeBeauty;

    public Dictionary<string, int> itemAmounts = new Dictionary<string, int>();//for saving
    public List<int> itemPreferances = new List<int>();
    public List<int> maxItemPreferances = new List<int>();
    public ItemSO favoriteItem;
    public ItemSO dislikedItem;
    public List<int> itemQuantities = new List<int>();
    public bool bankrupt;
    public string myName = "Blah";
    [SerializeField] private List<String> names = new List<string>();
    public MapPin mapPin;
    public float priceMulti;
    public bool special;

    public void BeforeDelay()
    {
        Invoke("NewStart", 1);
    }
    public void NewStart()
    {
        favoriteItem = Controller.Instance.items[Random.Range(0, Controller.Instance.items.Count)];
        dislikedItem = Controller.Instance.items[Random.Range(0, Controller.Instance.items.Count)];
        if (favoriteItem.special || dislikedItem.special) { NewStart(); return; }
        if (dislikedItem == favoriteItem) { NewStart(); return; }

        //money = Random.Range(136800, 20000);
        //money /= (TransitionController.Instance.totalDifficulty);
        //money = Random.Range(68400, 10000);
        //money /= (TransitionController.Instance.totalDifficulty / 2);
        //money = Random.Range(27360, 4000);
        //money /= (TransitionController.Instance.totalDifficulty / 5);
        //money = Random.Range(27360, 4000);
        //money /= (TransitionController.Instance.totalDifficulty / 5) - ((TransitionController.Instance.totalDifficulty / 5) - 0.36);
        //money /= (TransitionController.Instance.totalDifficulty / 6.84f);

        //money = Random.Range(136800, 380000);
        //money = Random.Range(7200, 20000);
        money = Random.Range(7200, 380000);
        money /= (TransitionController.Instance.totalDifficulty);
        money = Mathf.Clamp(money, 36000, 100000);
        priceMulti = Random.Range(0.75f, 1.25f);
        Array enumValues = Enum.GetValues(typeof(CompetitorType));
        type = (CompetitorType)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
        switch (type)
        {
            case CompetitorType.Average: itemQuality = Random.Range(0, 100); orderAmounts = Random.Range(1, 10); storeCleanlyness = Random.Range(0, 100); storeBeauty = Random.Range(0, 100); break;
            case CompetitorType.Bulk: itemQuality = Random.Range(0, 50); orderAmounts = Random.Range(5, 10); storeCleanlyness = Random.Range(0, 50); storeBeauty = Random.Range(0, 50); priceMulti -= Random.Range(0, 0.25f); break;
            case CompetitorType.Quality: itemQuality = Random.Range(50, 100); orderAmounts = Random.Range(1, 5); storeCleanlyness = Random.Range(50, 100); storeBeauty = Random.Range(50, 100); priceMulti += Random.Range(0, 0.25f); break;
            case CompetitorType.Discount: itemQuality = Random.Range(0, 25); orderAmounts = Random.Range(1, 25); storeCleanlyness = Random.Range(0, 25); storeBeauty = Random.Range(0, 25); priceMulti -= Random.Range(0.25f, 0.5f); break;
        }

        foreach (ItemSO item in Controller.Instance.items)
        {
            if (!item.special)
            {
                int number = Random.Range(0, (int)(item.defaultNeedGrowth * (TransitionController.Instance.cityPopulation / 10)) + 1);
                if (special) { number *= 2; }
                if (item.itemID == favoriteItem.itemID) { number *= 3; }
                if (item.itemID == dislikedItem.itemID && !special) { number = 1; }
                itemPreferances.Add(number);
                maxItemPreferances.Add((int)(item.defaultNeedGrowth * (TransitionController.Instance.cityPopulation / 10)) + 1 * 10);
                itemAmounts.Add(item.myName, 0);
                itemQuantities.Add(0);
            }
            else
            {
                itemPreferances.Add(0);
                maxItemPreferances.Add(0);
                itemAmounts.Add(item.myName, 0);
                itemQuantities.Add(0);
            }
        }

        myName = names[Random.Range(0, names.Count)];

        if (special)
        {
            money *= 10;
            priceMulti /= 1.5f;
            itemQuality *= 2;
            orderAmounts *= 2;
            storeCleanlyness *= 2;
            storeBeauty *= 2;
        }

        UIController.Instance.OnDayValueChanged += OrderItems;
        debtMax = 100000 / TransitionController.Instance.totalDifficulty;
        if (special) { debtMax *= 100; }
    }
    public void LoadedStartUp()
    {
        UIController.Instance.OnDayValueChanged += OrderItems;
        debtMax = 100000 / TransitionController.Instance.totalDifficulty;
        if (special) { debtMax *= 100; }
    }

    private void OrderItems(object sender, System.EventArgs e)
    {
        OperatingCosts();
        float before = money;


        if (!bankrupt)
        {
            //items
            bool needMoreEmployees = false;
            for (int i = 0; i < Controller.Instance.items.Count; i++)
            {
                if (Controller.Instance.items[i].year_Start <= UIController.Instance.year && !Controller.Instance.items[i].special)
                {
                    if (Controller.Instance.items[i].seasonal)
                    {
                        if (UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[0] || UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[3])
                        {
                            if (!Controller.Instance.items[i].seasons.Contains("Winter")) { continue; }
                        }
                        else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[0] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[1])
                        {
                            if (!Controller.Instance.items[i].seasons.Contains("Spring")) { continue; }
                        }
                        else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[1] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[2])
                        {
                            if (!Controller.Instance.items[i].seasons.Contains("Summer")) { continue; }
                        }
                        else if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[2] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[3])
                        {
                            if (!Controller.Instance.items[i].seasons.Contains("Fall")) { continue; }
                        }
                    }
                    /*
                    switch (Controller.Instance.items[i].seasonal)
                    {
                        case -1: break;

                        //winter
                        case 0:
                            if (UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[0] || UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[3]) { break; }
                            else { continue; }

                        //Spring
                        case 1:
                            if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[0] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[1]) { break; }
                            else { continue; }

                        //summer
                        case 2:
                            if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[1] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[2]) { break; }
                            else { continue; }

                        //fall
                        case 3:
                            if (UIController.Instance.dayOfTheYear >= Controller.Instance.SeasonsStart[2] && UIController.Instance.dayOfTheYear < Controller.Instance.SeasonsStart[3]) { break; }
                            else { continue; }

                    }
                    */
                    //if (itemQuantities.Count <= i) { break; }
                    if (itemQuantities[i] <= 0) { itemPreferances[i]++; }
                    maxItemPreferances[i] = ((int)(Controller.Instance.items[i].defaultNeedGrowth * (TransitionController.Instance.cityPopulation / 10)) + 1) * 10;

                    if (itemQuantities[i] >= maxItemPreferances[i]) { itemPreferances[i]--; }
                    if (itemPreferances[i] < maxItemPreferances[i] * 5 && i == favoriteItem.itemID) { itemPreferances[i]++; }
                    if (itemPreferances[i] > 1 && i == dislikedItem.itemID && !special) { itemPreferances[i]--; }
                    if (itemPreferances[i] < 0) { itemPreferances[i] = 0; maxItemPreferances[i] = 0; }

                    int orders = Random.Range(0, itemPreferances[i] * orderAmounts);
                    if (orders > numOfEmployees * 10) { needMoreEmployees = true; orders = numOfEmployees * 10; }
                    itemQuantities[i] += orders;
                    itemAmounts[Controller.Instance.items[i].myName] += orders;
                    money -= Controller.Instance.items[i].cost * orders * ((TransitionController.Instance.tax) + 1);
                }
            }

            //hire employees
            if (needMoreEmployees) { numOfEmployees++; }

            //take loan?
            if (money < 0)
            {
                numOfEmployees--;
                if (debt < debtMax)
                {
                    //taxes?
                    money += debtMax / 2;
                    debt += debtMax / 2;
                    UIController.Instance.CreateLog(4, myName + " " + type.ToString() + Localizer.Instance.GetLocalizedText(" goods store") + Localizer.Instance.GetLocalizedText(" has taken out a loan to avoid bankruptcy!"), "Manager", 0);
                }
                else
                {
                    UIController.Instance.CreateLog(4, myName + " " + type.ToString() + Localizer.Instance.GetLocalizedText(" goods store") + Localizer.Instance.GetLocalizedText(" has gone out of business"), "Manager", 0);
                    DeclareBankrupty();
                }
            }
        }
        else if (money > 0)
        {
            //bankrupt = false;
        }
        else
        {
            money += 961 * TransitionController.Instance.leasePricePerSquareFoot;
            money += (debt / 100);
        }

        float total = money - before;
    }

    public void BuyItem(int itemID)
    {
        itemAmounts[Controller.Instance.items[itemID].myName] -= 1;
        itemQuantities[itemID] -= 1;

        float value = Controller.Instance.items[itemID].value * priceMulti;
        float newValue = (value * ((TransitionController.Instance.tax) + 1));
        newValue -= value;
        value = value - newValue;
        money += value;
        //relay some values to the customer?
    }

    public void DeclareBankrupty()
    {
        if (!Controller.Instance.competitorBailouts)
        {
            numOfEmployees = 0;
            bankrupt = true;
            foreach (Customer2 customer in Controller.Instance.customers)
            {
                customer.storePreferance[Controller.Instance.competitors.IndexOf(this) + 1] = 0;
            }
            mapPin.DeActivate();
            mapPin.gameObject.SetActive(false);
        }
        else
        {
            money += 1000;
        }
    }
    private void OperatingCosts()
    {
        float before = money;

        if (special) { money += 10000; }
        money += Controller.Instance.competitorFreeMoney;
        //wages
        money -= (numOfEmployees * employeePay) * ((TransitionController.Instance.tax) + 1);
        //rent
        float squareFootage = UIController.Instance.playedDays * 3;
        if (squareFootage > 961) { squareFootage = 961; } if (squareFootage < 100) { squareFootage = 100; }
        float rent = (squareFootage * TransitionController.Instance.leasePricePerSquareFoot) * ((TransitionController.Instance.tax) + 1);
        money -= rent;
        float operatingCosts = 0;
        money -= operatingCosts;
        //electricty
        int amountOfCurrentItems = 0;
        for (int i = 0; i < itemQuantities.Count; i++) { amountOfCurrentItems += itemQuantities[i]; }
        money -= (amountOfCurrentItems * TransitionController.Instance.electrictyCosts) * ((TransitionController.Instance.tax) + 1);
        //debt
        money -= (debt / 100) * ((TransitionController.Instance.tax) + 1);
        debt -= (debt / 100);

        float total = before - money;
    }

    //---------------------------------------------------------------------------------------------------------------------------

    [System.Serializable]
    public class SaveObject
    {
        public string storeName;
        public CompetitorType type;
        public float money;
        public float debt;

        public float employeePay;
        public int amountOfEmployees;

        public List<string> itemID = new List<string>();
        public List<int> itemAmount = new List<int>();
        public List<int> itemPrefs = new List<int>();
        public List<int> maxItemPrefs = new List<int>();
        public string favItem;
        public string dislikedItem;

        public bool isBankrupt;
        public bool isSpecial;

        public int itemQuality;
        public int storeCleanlyness;
        public int storeBeauty;
        public int orderAmounts;
        public float priceMulti;
        public List<int> itemQuantities = new List<int>();

        public Vector3 mapPinPos;
    }
    public SaveObject Save()
    {
        List<string> itemID = new List<string>();
        List<int> itemAmount = new List<int>();
        foreach (KeyValuePair<string, int> pair in this.itemAmounts)
        {
            itemID.Add(pair.Key);
            itemAmount.Add(pair.Value);
        }
        return new SaveObject
        {
            storeName = this.myName,
            type = this.type,
            money = this.money,
            debt = this.debt,

            employeePay = this.employeePay,
            amountOfEmployees = this.numOfEmployees,

            itemID = itemID,
            itemAmount = itemAmount,
            itemPrefs = this.itemPreferances,
            maxItemPrefs = this.maxItemPreferances,
            favItem = this.favoriteItem.myName,
            dislikedItem = this.dislikedItem.myName,

            isBankrupt = this.bankrupt,
            isSpecial = this.special,

            itemQuality = this.itemQuality,
            storeCleanlyness = this.storeCleanlyness,
            storeBeauty = this.storeBeauty,
            orderAmounts = this.orderAmounts,
            priceMulti = this.priceMulti,
            itemQuantities = this.itemQuantities,

            mapPinPos = mapPin.transform.GetComponent<RectTransform>().anchoredPosition,
        };
    }

    public void Load(SaveObject saveObject)
    {
        Competitor newCompetitor = Controller.Instance.CreateCompetitor();
        MapPin newMapPin = UIController.Instance.CreateMapPin(newCompetitor);

        newMapPin.competitor = newCompetitor;
        newCompetitor.mapPin = newMapPin;

        newCompetitor.myName = saveObject.storeName;
        newCompetitor.type = saveObject.type;
        newCompetitor.money = saveObject.money;
        newCompetitor.debt = saveObject.debt;

        newCompetitor.employeePay = saveObject.employeePay;
        newCompetitor.numOfEmployees = saveObject.amountOfEmployees;

        newCompetitor.itemAmounts.Clear();
        for (int i = 0; i < saveObject.itemID.Count; i++)
        {
            newCompetitor.itemAmounts.Add(saveObject.itemID[i], saveObject.itemAmount[i]);
        }
        newCompetitor.itemPreferances = saveObject.itemPrefs;
        newCompetitor.maxItemPreferances = saveObject.maxItemPrefs;
        newCompetitor.favItem = saveObject.favItem;
        newCompetitor.disItem = saveObject.dislikedItem;
        newnewComp = newCompetitor;

        newCompetitor.bankrupt = saveObject.isBankrupt;
        newCompetitor.special = saveObject.isSpecial;

        newCompetitor.itemQuality = saveObject.itemQuality;
        newCompetitor.storeCleanlyness = saveObject.storeCleanlyness;
        newCompetitor.storeBeauty = saveObject.storeBeauty;
        newCompetitor.orderAmounts = saveObject.orderAmounts;
        newCompetitor.priceMulti = saveObject.priceMulti;
        newCompetitor.itemQuantities = saveObject.itemQuantities;

        newMapPin.transform.GetComponent<RectTransform>().anchoredPosition = saveObject.mapPinPos;

        newMapPin.LoadedStartUp();
        newCompetitor.LoadedStartUp();
        newCompetitor.PreLoadDelay();
    }
    string favItem; string disItem; Competitor newnewComp;
    public void PreLoadDelay()
    {
        Invoke("LoadDelay", 1);
    }
    private void LoadDelay()
    {
        foreach (ItemSO item in Controller.Instance.items)
        {
            if (item.myName == favItem) { favoriteItem = item; }
            else if (item.myName == disItem) { dislikedItem = item; }
        }
    }
}
