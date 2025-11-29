using ArchDawn.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    private Transform list;
    private Transform valuesParent;
    public List<UIItemOrder> orders = new List<UIItemOrder>();

    private TMP_Text totalCosts;
    private TMP_Text totalGains;
    private TMP_Text totals;//after tax

    private float expiditeCost;
    private Button expiditeButton;
    private void Awake() { Instance = this; }
    private void Start()
    {
        list = transform.GetChild(3).GetChild(0).GetChild(0);
        valuesParent = transform.GetChild(0).GetChild(0);

        totalCosts = valuesParent.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        totalGains = valuesParent.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        totals = valuesParent.GetChild(0).GetChild(2).GetComponent<TMP_Text>();

        expiditeButton = transform.GetChild(5).GetComponent<Button>();
        Controller.Instance.OnMoneyValueChanged += InteractableCheck;
        Controller.Instance.OrderItems += NextDay;
    }
    public void UpdateValues()
    {
        if (list.childCount != orders.Count)
        {
            orders.Clear();
            for (int i = 0; i < list.childCount; i++)
            {
                orders.Add(list.GetChild(i).GetComponent<UIItemOrder>());
            }
        }

        float totalgain = 0;
        float totalcost = 0;

        foreach (UIItemOrder order in orders)
        {
            totalgain += (order.sell * order.dailyOrders);
            totalcost += (order.cost * order.dailyOrders);
        }

        float total = totalcost * (TransitionController.Instance.tax + 1);
        expiditeCost = total * 4;//+1 due to every order being called on own script
        InteractableCheck(null, System.EventArgs.Empty);

        totalCosts.text = "-" + totalcost.ToString("f2");
        totalGains.text = "+" + totalgain.ToString("f2");
        totals.text = "$" + total.ToString("f2");
    }
    public void ExpiditeOrder()
    {
        StartCoroutine(OrderItems());
        Controller.Instance.MoneyValueChange(-expiditeCost, UtilsClass.GetMouseWorldPosition(), true, false);
    }
    public void NextDay(object sender, System.EventArgs e)
    {
        StartCoroutine(OrderItems());
    }

    private void InteractableCheck(object sender, System.EventArgs e)
    {
        if (expiditeButton != null)
        {
            if (Controller.Instance.money >= expiditeCost || UIController.Instance.gameOver) { expiditeButton.interactable = true; }
            else { expiditeButton.interactable = false; }
        }
    }

    private IEnumerator OrderItems()
    {
        if (list.childCount != orders.Count)
        {
            orders.Clear();
            for (int i = 0; i < list.childCount; i++)
            {
                orders.Add(list.GetChild(i).GetComponent<UIItemOrder>());
            }
        }
        yield return new WaitForEndOfFrame();
        foreach (UIItemOrder order in orders)
        {
            if (Controller.Instance.money > order.costDaily || UIController.Instance.gameOver)
            {
                bool foundStockpile = false;
                foreach (Building stock in Controller.Instance.stockPiles) { if (stock.built) { foundStockpile = true; break; } }
                if (foundStockpile)
                {
                    //spawn
                    for (int i = 0; i < order.dailyOrders; i++)
                    {
                        Item placedObject = Item.Create(order.item);
                        placedObject.quality = order.itemQuality;
                        yield return new WaitForEndOfFrame();

                    }
                    //cost
                    Controller.Instance.MoneyValueChange(-order.costDaily, UtilsClass.GetMouseWorldPosition(), true, false);
                    Controller.Instance.dailyMoneyLostProduct += order.costDaily;
                }
            }
            order.PriceMarkEffects();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        foreach (UIItemOrder order in orders) { order.CheckDelay(); }
        yield return new WaitForEndOfFrame();
        Controller.Instance.FinishedOrdering();
    }
}
