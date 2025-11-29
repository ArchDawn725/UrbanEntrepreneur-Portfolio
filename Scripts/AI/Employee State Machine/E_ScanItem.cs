using Steamworks;
using UnityEngine;

public class E_ScanItem : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (selectItem != null)
        {
            Controller.Instance.MoneyValueChange(selectItem.value, employee.transform.position, true, false);
            if (targRegistor != null) { targRegistor.transform.GetChild(5).GetComponent<AudioSource>().Play(); }
            Controller.Instance.itemsSold[selectItem.myName]++;
            Controller.Instance.itemsSoldTotal++;

            if (SteamClient.IsValid)
            {
                Steamworks.SteamUserStats.AddStat("Items_Sold", 1);
                if (selectItem.itemSO.special) { Steamworks.SteamUserStats.AddStat("Special_Items_Sold", 1); }
                switch (selectItem.itemType)
                {
                    case "Fruit": Steamworks.SteamUserStats.AddStat("Fruits_Sold", 1); break;
                    case "Vegetable": Steamworks.SteamUserStats.AddStat("Vegetables_Sold", 1); break;
                    case "Cords": Steamworks.SteamUserStats.AddStat("Electronics_Sold", 1); break;
                    case "Electrictronic": Steamworks.SteamUserStats.AddStat("Electronics_Sold", 1); break;
                    case "Clothes": Steamworks.SteamUserStats.AddStat("Clothes_Sold", 1); break;
                }
                Steamworks.SteamUserStats.StoreStats();
            }

            //Destroy(selectItem.gameObject);
            selectItem.DeleteMe();
        }


        targItemID = 0;
        selectItem = null;

        /*
        if (targRegistor.customerQueue.Count > 0 && targRegistor.transform.GetChild(1).childCount == 0)
        {
            targRegistor.customerQueue[0].Leave();
        }
        */

        employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
        animator.SetTrigger("Success");
    }
}
