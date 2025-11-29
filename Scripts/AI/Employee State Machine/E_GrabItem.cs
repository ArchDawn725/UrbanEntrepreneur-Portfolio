using UnityEngine;
public class E_GrabItem : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null)
        {
            employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            employee.AddItem(selectItem);
            if (targBuilding != null) { targBuilding.RemovedItem(); }
        }

        if (customer != null)
        {
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            if (customer.money >= selectItem.value)
            {
                customer.AddItem(selectItem);
                customer.shoppingList.Remove(selectItem.itemSO);

                selectItem = null;
                customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                if (targBuilding != null) { targBuilding.RemovedItem(); }
            }
            else if (Controller.Instance.storeCredit)
            {
                customer.AddItem(selectItem);
                customer.shoppingList.Remove(selectItem.itemSO);

                selectItem = null;
                customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                if (targBuilding != null) { targBuilding.RemovedItem(); }
            }
            else
            {
                customer.couldNotAffordList.Add(selectItem.itemSO);
                customer.shoppingList.Remove(selectItem.itemSO);
                selectItem.claimed = false;
                selectItem = null;
                customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                //UtilsClass.CreateWorldTextPopup("I dont have enough money for this?", customer.transform.position, Color.red);
                customer.TalkBubble("I dont have enough money for this? :(", 1, 2);
                if (ToolTip.Instance.highestToolTipAchieved > 66) { ToolTip.Instance.ActivateTutorial(68); }
            }
        }

        animator.SetTrigger("Success");
    }
}
