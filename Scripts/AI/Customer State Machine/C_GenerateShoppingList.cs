using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class C_GenerateShoppingList : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        customer.shoppingList.Clear();
        customer.couldNotAffordList.Clear();


        List<ItemSO> possibleItems = new List<ItemSO>();
        foreach (ItemSO item in Controller.Instance.items)
        {
            foreach (Item findingItem in FindObjectsOfType<Item>()) { if (findingItem.itemSO == item) { possibleItems.Add(item); break; } }
        }

        int numberOfAllItems = 0;
        foreach (ItemSO item in Controller.Instance.items)
        {
            if (!item.special && !item.seasonal && item.year_Start >= UIController.Instance.year && item.year_End < UIController.Instance.year) { numberOfAllItems++; }
        }

        int numberOfAvailableItems = possibleItems.Count;
        customer.storePreferance[0] -= numberOfAllItems - numberOfAvailableItems;
        if (possibleItems.Count < Controller.Instance.items.Count / 5) { customer.storePreferance[0] -= 10; }
        if (possibleItems.Count < Controller.Instance.items.Count / 10) { customer.storePreferance[0] -= 10; }
        if (possibleItems.Count == 0) { customer.storePreferance[0] -= 25; }


        if (TransitionController.Instance.items.Contains("Everything"))
        {

            //if (possibleItems.Count < Controller.Instance.items.Count / 5 && Controller.Instance.items.Count < 10) { customer.storePreferance[0] -= 10; }
            int randomNumber = Random.Range(0, Controller.Instance.items.Count);
            if (!possibleItems.Contains(Controller.Instance.items[randomNumber])) { possibleItems.Add(Controller.Instance.items[randomNumber]); }

            foreach (KeyValuePair<string, List<float>> pair in customer.ItemPreferences)
            {
                if (pair.Value[0] > 50) { foreach (ItemSO item in possibleItems) { if (item.myName == pair.Key && customer.money >= item.baseValue) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 60) { foreach (ItemSO item in possibleItems) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.9f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 70) { foreach (ItemSO item in possibleItems) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.75f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 80) { foreach (ItemSO item in possibleItems) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.5f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 90) { foreach (ItemSO item in possibleItems) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.25f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 100)
                {
                    foreach (ItemSO item in possibleItems) { if (item.myName == pair.Key) { customer.shoppingList.Add(item); } }
                    foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { customer.shoppingList.Add(item); } } //search for item anyway
                }
            }
        }
        else
        {
            foreach (KeyValuePair<string, List<float>> pair in customer.ItemPreferences)
            {
                if (pair.Value[0] > 50) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key && customer.money >= item.baseValue) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 60) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.9f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 70) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.75f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 80) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.5f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 90) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key && customer.money >= item.baseValue * 0.25f) { customer.shoppingList.Add(item); } } }
                if (pair.Value[0] > 100) { foreach (ItemSO item in Controller.Instance.items) { if (item.myName == pair.Key) { customer.shoppingList.Add(item); } } }
            }
        }


        if (customer.shoppingList.Count > 0 && customer.money > 0)
        {
            customer.shoppingList.Shuffle();
            if (!customer.special) { customer.shoppingList.ReduceTo(10); }
            targItemID = customer.shoppingList[0].itemID;
            customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
        }
        else { animator.SetBool("AtSite", false); animator.SetTrigger("Failure"); }
    }
}
