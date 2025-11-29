using System.Collections.Generic;
using UnityEngine;

public class E_FindWrongItems : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
        Item targetItem = null;
        Building targetShelf = null;

        if (targetItem != null) { targetItem = null; }

        foreach (Building shelf in Controller.Instance.shelves)
        {
            if (shelf.transform.GetChild(1).childCount > 0)
            {
                foreach (Item item in shelf.transform.GetChild(1).GetComponentsInChildren<Item>())
                {
                    if (shelf.selectedItemTypeID != item.itemTypeID)// && item.claimed == false)
                    {
                        bool found = false;
                        foreach (Building stock in Controller.Instance.stockPiles)
                        {
                            if (stock.built && stock.capacity >= stock.transform.GetChild(1).childCount && stock.allowedItemTypesID.Contains(item.itemTypeID))
                            { found = true; break; }
                        }

                        if (found)
                        {
                            targetItem = item;
                            targetShelf = shelf;
                            item.claimed = true;
                            break;
                        }
                        else { Debug.Log("Could not find stock"); }
                    }
                }
            }
        }

        if (targetItem != null) 
        {
            targ = targetShelf.transform;
            //targetPos = targetShelf.customerLocation.position;
            targetPos = targetShelf.queueLocations[targetShelf.queueLocations.Count - 1].transform.position;
            selectItem = targetItem;
            targItemID = targetItem.itemTypeID;
            targBuilding = targetShelf;
            targShelf = targetShelf;
            employee.beforeLine = true;

            employee.SetPathfinding(currentPath, pathList, targetPos);
            employee.SetAI(targ, newTsk);
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
            return;
            /*
            targetShelf.AddToQueue(employee, null);

            if (targetShelf.employeeQueue.Count > 0)
            {

                if (targetShelf.employeeQueue[0] == employee && targetShelf.simultaneous)
                {
                    targ = targetShelf.transform;
                    targetPos = targetShelf.transform.GetChild(0).transform.GetChild(2).transform.position;
                    selectItem = targetItem;
                    targItemID = targetItem.itemTypeID;
                    targBuilding = targetShelf;
                    targShelf = targetShelf;

                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                    return;
                }
                else
                {
                    targ = targetShelf.transform;
                    targetPos = targetShelf.GetLinePosition(employee, null);
                    selectItem = targetItem;
                    targItemID = targetItem.itemTypeID;
                    targBuilding = targetShelf;
                    targShelf = targetShelf;

                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                    return;
                }
            }
            else { Debug.Log("No queue"); animator.SetTrigger("Failure"); }
            */

        }

        if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("No items to stock?!", 1, 2); }

        if (Controller.Instance.ifDoneStocking == "Do nothing")
        {
            //do nothing
            employee.SwitchObjective(1);
        }
        else if(Controller.Instance.ifDoneStocking == "Go home")
        {
            //go home
            employee.SendHome();
        }
        else
        {
            //switch task
            employee.SwitchTask(null, Controller.Instance.ifDoneStocking, null);
        }

        animator.SetTrigger("Failure");
    }
}
