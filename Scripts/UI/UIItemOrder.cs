using ArchDawn.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class UIItemOrder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemSO item;

    private TMP_Text nameText;

    private TMP_Text dailyOrdersText;
    private TMP_Text costPerText;
    private TMP_Text dailyCostText;
    private TMP_Text sellPerText;
    private TMP_Text orderMultiplierText;

    private Button increaseButton;
    private Button decreaseButton;

    private Image image;

    [HideInInspector] public float cost;
    [HideInInspector] public float sell;
    [HideInInspector] public int dailyOrders;
    [HideInInspector] public int orderMultiplier = 10;//set
    [HideInInspector] public int itemQuality = 25;
    private int displayOrders;
    public float costDaily;

    [SerializeField] private TextMeshProUGUI disc;
    public string supplier;
    private float costMultiplier = 1;
    [SerializeField] private float bulkBuyDiscount = 0.75f;
    public Slider markUpSlider;
    private TextMeshProUGUI markUpText;

    public void StartUp()
    {
        Transform costs = transform.GetChild(1);
        Transform main = transform.GetChild(2);

        costPerText = costs.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        sellPerText = costs.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        dailyCostText = costs.GetChild(2).GetComponent<TMP_Text>();
        orderMultiplierText = main.GetChild(6).GetComponent<TMP_Text>();

        image = main.GetChild(0).GetComponent<Image>();
        nameText = main.GetChild(1).GetComponent<TMP_Text>();
        dailyOrdersText = main.GetChild(4).GetComponent<TMP_Text>();

        decreaseButton = main.GetChild(3).GetComponent<Button>();
        increaseButton = main.GetChild(5).GetComponent<Button>();

        nameText.text = item.myName;
        nameText.GetComponent<AutoLocalizer>().UpdateLocalizedText(item.myName);
        image.sprite = item.sprite;
        cost = (item.cost * Controller.Instance.inflationAmount) * costMultiplier; costPerText.text = "-" + cost.ToString("f2") + "$";
        dailyCostText.text = "-" + 0f.ToString("f2") + "$";
        dailyOrdersText.text = 0.ToString();
        orderMultiplierText.text = "x" + orderMultiplier.ToString();

        increaseButton.onClick.AddListener(() => AddOrder(1));
        decreaseButton.onClick.AddListener(() => AddOrder(-1));

        //Controller.Instance.OrderItems += NextDay;

        disc = transform.parent.parent.parent.parent.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        markUpSlider = main.GetChild(9).GetComponent<Slider>();
        markUpText = markUpSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        name = item.myName;
        transform.GetChild(3).name = "Cheap Bulk";
        AddOrder(0);

        markUpSlider.minValue = Controller.Instance.itemMarkupMin * 10;
        markUpSlider.maxValue = Controller.Instance.itemMarkupMax * 10;
    }

    public void AddOrder(int value)
    {
        if (Controller.Instance.ctrlButtonDown) { value *= 10; }
        else if (Controller.Instance.shiftButtonDown) { value *= 5; }
        dailyOrders += value * orderMultiplier;
        UIController.Instance.stockTickets += value * orderMultiplier;
        if (dailyOrders < 0) { dailyOrders = 0; UIController.Instance.stockTickets += -value * orderMultiplier; }
        float bulkDiscount = (dailyOrders * bulkBuyDiscount) - dailyOrders;
        cost = (item.cost * Controller.Instance.inflationAmount) * costMultiplier;
        costPerText.text = "-" + cost.ToString("f2") + "$";
        costDaily = (dailyOrders * cost) + bulkDiscount;
        //dailyOrdersText.text = dailyOrders.ToString();
        displayOrders = dailyOrders / orderMultiplier;
        dailyOrdersText.text = displayOrders.ToString();
        dailyCostText.text = "-" + costDaily.ToString("f2") + "$";
        costDaily *= (TransitionController.Instance.tax + 1);
        sell = item.value; sellPerText.text = "+" + sell.ToString("f2") + "$";
        Activate();
        OrderManager.Instance.UpdateValues();
        transform.GetChild(2).name = dailyOrders.ToString();
        transform.GetChild(0).name = (cost*10).ToString();
        transform.GetChild(1).name = (item.value*10).ToString();
    }
    /*
    public void NextDay(object sender, System.EventArgs e)
    {
        StartCoroutine(OrderItems());
    }
    *//*

    *//*
    private IEnumerator OrderItems()
    {
        if (Controller.Instance.money > costDaily || UIController.Instance.gameOver)
        {
            bool foundStockpile = false;
            foreach (Building stock in Controller.Instance.stockPiles) { if (stock.built) { foundStockpile = true; break; } }
            if (foundStockpile)
            {
                //spawn
                for (int i = 0; i < dailyOrders; i++)
                {
                    Debug.Log(i + " / " + dailyOrders);
                    Item placedObject = Item.Create(item);
                    placedObject.quality = itemQuality;
                    yield return new WaitForEndOfFrame();

                }
                Debug.Log("Called3");
                //cost
                Controller.Instance.MoneyValueChange(-costDaily, UtilsClass.GetMouseWorldPosition(), true);
                Controller.Instance.dailyMoneyLostProduct += costDaily;
            }
        }
        PriceMarkEffects();
        yield return new WaitForEndOfFrame();
        Invoke("CheckDelay", 0.1f);
    }
    */
    public void OnPointerEnter(PointerEventData eventData) { Activate(); }
    public void OnPointerExit(PointerEventData eventData) { DeActivate(); }
    private void Activate()
    {
        string lifespan = "";
        if (item.lifeSpan != -1) { lifespan = (item.lifeSpan / (4 * 24)).ToString("f1") + Localizer.Instance.GetLocalizedText(" Days"); }
        else { lifespan = Localizer.Instance.GetLocalizedText("Indefinitely"); }

        int amountInStock = 0;
        foreach (Item item in FindObjectsOfType<Item>()) 
        { 
            if (item.myName == this.item.myName && (item.stock.storageType.ToString() == "shelf" || item.stock.storageType.ToString() == "stockpile" || item.stock.storageType.ToString() == "employee")) 
            { amountInStock++; } 
        }

        List<string> traits = new List<string>();
        foreach(string season in item.seasons) { traits.Add(Localizer.Instance.GetLocalizedText(season)); } 
        string traitString = null;
        if (item.seasonal) { traitString = string.Join(", ", traits); }
        else { traitString = Localizer.Instance.GetLocalizedText("All"); }
        

        disc.text =
            Localizer.Instance.GetLocalizedText(item.myName) + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Cost multiplier: ") + costMultiplier.ToString() + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Order cost: $") + cost + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Sell value: $") + item.value + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Item Quality: ") + itemQuality + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Item Shelf Life: ") + lifespan + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Total amount in stock: ") + amountInStock.ToString() + System.Environment.NewLine +
            Localizer.Instance.GetLocalizedText("Seasons available: ") + traitString
            ;
    }
    private void DeActivate()
    {
        disc.text =
""
    ;
    }
    public void ChangeSupplier(string manufactorer)
    {
        switch (manufactorer)
        {
            case "Cheap Bulk": supplier = "Cheap Bulk"; orderMultiplier = 10; itemQuality = 25; costMultiplier = 1; break;
            case "Quality Supplies": supplier = "Quality Supplies"; orderMultiplier = 5; itemQuality = 50; costMultiplier = 1.5f; break;
            case "Golden Goods": supplier = "Golden Goods"; orderMultiplier = 50; itemQuality = 150; costMultiplier = 1.1f; break;
            case "Prestigious Goods": supplier = "Prestigious Goods"; orderMultiplier = 1; itemQuality = 100; costMultiplier = 2f; break;
            case "Black Market": supplier = "Black Market"; orderMultiplier = 25; itemQuality = 75; costMultiplier = 0.25f; break;
            case "Clearance": supplier = "Clearance"; orderMultiplier = 25; itemQuality = 0; costMultiplier = 0.75f; break;
            case "Luxury Seller": supplier = "Luxery Seller"; orderMultiplier = 10; itemQuality = 200; costMultiplier = 1.25f; break;
            case "Any and All": supplier = "Any and All"; orderMultiplier = 100; itemQuality = 100; costMultiplier = 0.5f; break;
        }

        transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "x" + orderMultiplier.ToString();
        transform.GetChild(2).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = supplier;
        transform.GetChild(2).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().GetComponent<AutoLocalizer>().UpdateLocalizedText(supplier);
        AddOrder(0);

        transform.GetChild(3).name = supplier;
    }
    public void ChangeSupplier()
    {
        switch(supplier)
        {
            case "Cheap Bulk":
                supplier = "Quality Supplies"; orderMultiplier = 5; itemQuality = 50; costMultiplier = 1.5f;
                if (Controller.Instance.unlockedSpecialManufactorers.ContainsKey("Golden Goods") && Controller.Instance.unlockedSpecialManufactorers.ContainsValue(item.myName)) { supplier = "Golden Goods"; orderMultiplier = 50; itemQuality = 150; costMultiplier = 1.1f; }
                break;

            case "Golden Goods": supplier = "Quality Supplies"; orderMultiplier = 5; itemQuality = 50; costMultiplier = 1.5f; break;

            case "Quality Supplies": 
                supplier = "Prestigious Goods"; orderMultiplier = 1; itemQuality = 100; costMultiplier = 2f;
                if (Controller.Instance.unlockedSpecialManufactorers.ContainsKey("Black Market") && Controller.Instance.unlockedSpecialManufactorers.ContainsValue(item.myName)) { supplier = "Black Market"; orderMultiplier = 25; itemQuality = 75; costMultiplier = 0.25f; }
                break;

            case "Black Market": supplier = "Prestigious Goods"; orderMultiplier = 1; itemQuality = 100; costMultiplier = 2f; break;

            case "Prestigious Goods": 
                supplier = "Clearance"; orderMultiplier = 25; itemQuality = 0; costMultiplier = 0.75f;
                if (Controller.Instance.unlockedSpecialManufactorers.ContainsKey("Luxury Seller") && Controller.Instance.unlockedSpecialManufactorers.ContainsValue(item.myName)) { supplier = "Luxery Seller"; orderMultiplier = 10; itemQuality = 200; costMultiplier = 1.25f; }
                break;

            case "Luxury Seller": supplier = "Clearance"; orderMultiplier = 25; itemQuality = 0; costMultiplier = 0.75f; break;

            case "Clearance": 
                supplier = "Cheap Bulk"; orderMultiplier = 10; itemQuality = 25; costMultiplier = 1;
                if (Controller.Instance.unlockedSpecialManufactorers.ContainsKey("Any and All") && Controller.Instance.unlockedSpecialManufactorers.ContainsValue(item.myName)) { supplier = "Any and All"; orderMultiplier = 100; itemQuality = 100; costMultiplier = 0.5f; }
                break;

            case "Any and All": supplier = "Cheap Bulk"; orderMultiplier = 10; itemQuality = 25; costMultiplier = 1; break;
        }

        transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "x" + orderMultiplier.ToString();
        transform.GetChild(2).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = supplier;
        transform.GetChild(2).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().GetComponent<AutoLocalizer>().UpdateLocalizedText(supplier);
        AddOrder(0);

        transform.GetChild(3).name = supplier;
    }
    public void ChangeMarkUpPrice()
    {
        item.value = item.baseValue * (markUpSlider.value / 10f);
        foreach(Item items in FindObjectsOfType<Item>()) { if (items.itemSO == item) { items.value = item.value; } }
        markUpText.text = (markUpSlider.value * 10).ToString() + "%";
        AddOrder(0);
    }
    public void PriceMarkEffects()
    {
        if (markUpSlider.value != 10)
        {
            if (markUpSlider.value > 10)
            {
                //bad
                float value = ((markUpSlider.value - 10) / 2) / 10;
                foreach (Customer2 customer in Controller.Instance.customers) { customer.ItemPreferences[item.myName][0] -= customer.ItemPreferences[item.myName][1] * value; customer.ItemPreferences[item.myName][1] -= (customer.ItemPreferences[item.myName][1] * value) / 20; }
            }
            else if (markUpSlider.value < 10)
            {
                //good
                float value = ((markUpSlider.value - 10) * -1) / 10;
                foreach (Customer2 customer in Controller.Instance.customers) { customer.ItemPreferences[item.myName][0] += customer.ItemPreferences[item.myName][1] * value; customer.ItemPreferences[item.myName][1] += (customer.ItemPreferences[item.myName][1] * value) / 10; }
            }
        }
        /*
        if (markUpSlider.value != 10)
        {
            if (markUpSlider.value > 10)
            {
                //bad
                float value = (markUpSlider.value - 10) / 2;
                foreach (Customer2 customer in Controller.Instance.customers) { customer.ItemPreferences[item.myName][0] -= value; customer.ItemPreferences[item.myName][1] -= (value * 0.01f); }
            }
            if (markUpSlider.value < 10)
            {
                //good
                float value = (markUpSlider.value - 10) * -1;
                foreach (Customer2 customer in Controller.Instance.customers) { customer.ItemPreferences[item.myName][0] += value; customer.ItemPreferences[item.myName][1] += (value * 0.1f); }
            }
        }
        */
    }

    public void CheckDelay()
    {
        Invoke("CheckDelay2", 1);
    }
    private void CheckDelay2()
    {
        if (!Controller.Instance.unlockedSpecialItems.Contains(item) && (item.special || item.seasonal)) { OrderManager.Instance.orders.Remove(this); Destroy(gameObject); }
        if (Controller.Instance.removedItems.Contains(item)) { OrderManager.Instance.orders.Remove(this); Destroy(gameObject); }
    }
}
