using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerReporter : MonoBehaviour
{
    private Controller con;
    private Transform days;
    private Transform hours;
    private Transform items;

    private List<Slider> daySliders = new List<Slider>();
    private List<Slider> hourSliders = new List<Slider>();

    private void Start()
    {
        days = transform.GetChild(0);
        hours = transform.GetChild(1);
        items = transform.GetChild(2).GetChild(0);

        for (int i = 0; i < days.childCount; i++) { daySliders.Add(days.GetChild(i).GetChild(0).GetComponent<Slider>()); }
        for (int i = 0; i < hours.childCount; i++) { hourSliders.Add(hours.GetChild(i).GetChild(0).GetComponent<Slider>()); }

        con = Controller.Instance;

        TickSystem.Instance.On10Tick += On10Tick;
        On10Tick(null, null);
    }
    private void On10Tick(object sender, TickSystem.OnTickEventArgs e)
    {
        foreach (Slider slide in daySliders) { slide.value = 0; slide.maxValue = con.customers.Count; }
        foreach(Customer2 customer in con.customers)
        {
            if (customer.ShoppingDays["Sunday"] == true) { daySliders[0].value++; }
            if (customer.ShoppingDays["Monday"] == true) { daySliders[1].value++; }
            if (customer.ShoppingDays["Tuesday"] == true) { daySliders[2].value++; }
            if (customer.ShoppingDays["Wednesday"] == true) { daySliders[3].value++; }
            if (customer.ShoppingDays["Thursday"] == true) { daySliders[4].value++; }
            if (customer.ShoppingDays["Friday"] == true) { daySliders[5].value++; }
            if (customer.ShoppingDays["Saturday"] == true) { daySliders[6].value++; }
        }

        foreach (Slider slide in hourSliders) { slide.value = 0; slide.maxValue = 2147483647; }
        foreach (Customer2 customer in con.customers) { hourSliders[customer.shoppingTime].value++; }
        int highestNumber = 0;
        foreach (Slider slide in hourSliders) { if (slide.value > highestNumber) { highestNumber = (int)slide.value; } }
        foreach (Slider slide in hourSliders) { slide.maxValue = highestNumber; }

        for (int i = 0; i < items.childCount; i++) { items.GetChild(i).GetChild(0).GetComponent<Slider>().value = 0; items.GetChild(i).GetChild(0).GetComponent<Slider>().maxValue = 2147483647; }
        for (int i = 0; i < items.childCount; i++)
        {
            foreach (Customer2 customer in con.customers)
            {
                if (customer.ItemPreferences.ContainsKey(items.GetChild(i).name))
                {
                    items.GetChild(i).GetChild(0).GetComponent<Slider>().value += (int)customer.ItemPreferences[items.GetChild(i).name][0];
                }
            }
        }
        highestNumber = 0;
        for (int i = 0; i < items.childCount; i++) { if (items.GetChild(i).GetChild(0).GetComponent<Slider>().value > highestNumber) { highestNumber = (int)items.GetChild(i).GetChild(0).GetComponent<Slider>().value; } }
        for (int i = 0; i < items.childCount; i++) { items.GetChild(i).GetChild(0).GetComponent<Slider>().maxValue = highestNumber; }
    }
}
