using System.Collections.Generic;
using UnityEngine;
public class E_Returning : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.RemoveAllClaims();
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        //stocking = Stocking.returning;
        //registering = Registering.returning;
        employee.SwitchObjective(2);
        //targBuild = Controller.Instance.stockPiles[0];//get stockpiles
        //targBuilding = Controller.Instance.stockPiles[Random.Range(0, Controller.Instance.stockPiles.Count)];
        List<Building> stockpiles = new List<Building>();

        if (targBuilding != null)
        {
            if (targBuilding.type != BuildingSO.Type.stockPile) { targBuilding = null; }
        }

        if (targBuilding == null)
        {
            if (employee.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<Item>() != null)
            { selectItem = employee.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<Item>(); targItemID = selectItem.itemTypeID; }

            foreach (Building stock in Controller.Instance.stockPiles)
            {
                if (stock.built && stock.capacity >= stock.transform.GetChild(1).childCount && stock.allowedItemTypesID.Contains(targItemID))
                { stockpiles.Add(stock);  }
            }

            if (stockpiles.Count > 0) { targBuilding = stockpiles[Random.Range(0, stockpiles.Count)]; }
            else { targBuilding = Controller.Instance.stockPiles[Random.Range(0, Controller.Instance.stockPiles.Count)]; }
        }


        if (targBuilding != null)
        {
            targ = targBuilding.transform;
            //targetPos = targBuilding.employeeLocation.position;
            targetPos = targBuilding.queueLocations[targBuilding.queueLocations.Count - 1].transform.position;
            employee.beforeLine = true;

            employee.SetPathfinding(currentPath, pathList, targetPos);
            employee.SetAI(targ, newTsk);
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");

            /*
            targBuilding.AddToQueue(employee, null);

            if (targBuilding.employeeQueue.Count > 0)
            {

                if (targBuilding.employeeQueue[0] == employee && targBuilding.simultaneous)
                {
                    targ = targBuilding.transform;
                    targetPos = targBuilding.employeeLocation.position;

                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
                else
                {
                    targ = targBuilding.transform;
                    targetPos = targBuilding.GetLinePosition(employee, null);

                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
            }
            else { Debug.Log("No queue"); animator.SetTrigger("Failure"); }
            */
        }
        else { animator.SetTrigger("Failure"); }

    }
}
