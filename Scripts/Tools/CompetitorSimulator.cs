using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CompetitorSimulator : MonoBehaviour
{
    private int days = -1;
    [SerializeField] private float waitTime;
    [SerializeField] private List<float> itemCosts = new List<float>();
    [SerializeField] private List<float> itemValues = new List<float>();
    [SerializeField] private List<int> itemNeed = new List<int>();
    [SerializeField] private int customers;


    [SerializeField] private float money;
    [SerializeField] private float debt;
    [SerializeField] private float debtMax;

    [SerializeField] private float employeePay;
    [SerializeField] private int numOfEmployees;
    public List<int> itemQuantities = new List<int>();
    public List<int> itemPreferances = new List<int>();

    [SerializeField] private bool special;
    [SerializeField] private bool needMoreEmployees;
    private void Start()
    {
        StartCoroutine(Corutine());
    }
    private IEnumerator Corutine()
    {
        yield return new WaitForSeconds(waitTime);
        days++;
        Debug.Log("Day: " + days);

        if (money < 0) { debt += 10000; money += 10000; }

        OperatingCosts();
        OrderItems();
        SellItems();
        Expand();

        if (debt > debtMax) { Debug.Log("Bankrupt day: " + days); }
        else { StartCoroutine(Corutine()); }
    }
    private void OperatingCosts()
    {
        float before = money;
        if (special) { money += 10000; }
        //wages
        money -= (numOfEmployees * employeePay) * ((1) + 1);
        //rent
        float rent = (961 * 1.5f) * ((1) + 1);
        money -= rent;
        float operatingCosts = 0;
        money -= operatingCosts;
        //electricty
        int amountOfCurrentItems = 0;
        for (int i = 0; i < itemQuantities.Count; i++) { amountOfCurrentItems += itemQuantities[i]; }
        money -= (amountOfCurrentItems * 1) * ((1) + 1);
        //debt
        money -= (debt / 100) * ((1) + 1);
        debt -= (debt / 100);

        float total = before - money;
        Debug.Log("Cost total: " + total);
    }
    private void OrderItems()
    {

    }
    private void SellItems() 
    {
        float before = money;
        for (int i = 0; i < customers; i++)
        {
            for (int x = 0; x < itemNeed.Count; x++)
            {
                if (itemQuantities[x] > 0)
                {
                    money += itemValues[x] * itemNeed[x];
                    itemQuantities[x] -= itemNeed[x];
                }
            }
        }
        float total = money -  before;
        Debug.Log("Profit total: " + total);
    }
    private void Expand()
    {
        if (needMoreEmployees) { numOfEmployees++; needMoreEmployees = false; }
    }
}
