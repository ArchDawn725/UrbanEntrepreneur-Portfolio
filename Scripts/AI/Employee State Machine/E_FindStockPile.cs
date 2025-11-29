using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class E_FindStockPile : StateMachineBehaviour
{
    private List<Building> possibleStockPiles = new List<Building>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (targStockPile != null)
        {
            if (targStockPile.transform.GetChild(1).childCount == 0) { targStockPile.RemoveFromQueue(employee, null); targStockPile = null; }//need remove from queue?
        }

        if (targStockPile != null && targItemID != -1)
        {
            bool found = false;
            foreach (Item item in targStockPile.transform.GetChild(1).GetComponentsInChildren<Item>())
            {
                if (item.itemTypeID == targItemID) { found = true; break; }
            }
            if (!found) { targStockPile.RemoveFromQueue(employee, null); targStockPile = null; }
        }

        if (targStockPile == null)
        {
            possibleStockPiles.Clear();

            if (Controller.Instance.stockPiles.Count > 0)
            {
                foreach (Building stockPile in Controller.Instance.stockPiles)
                {
                    bool found = false;

                    if (stockPile.transform.GetChild(1).childCount > 0)
                    {
                        if (stockPile.employeeQueue.Count == 0 && stockPile.built)//if (stockPile.employee == null || stockPile.employee == employee)
                        {
                            foreach (Item item in stockPile.transform.GetChild(1).GetComponentsInChildren<Item>())
                            {
                                if (found) { break; }
                                if (targShelf == null)
                                {
                                    foreach (Building shelf in Controller.Instance.shelves)
                                    {
                                        if (item.itemTypeID == shelf.selectedItemTypeID && shelf.transform.GetChild(1).childCount < shelf.capacity && shelf.built)
                                        {
                                            found = true; break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.itemTypeID == targShelf.selectedItemTypeID && targShelf.transform.GetChild(1).childCount < targShelf.capacity && targShelf.built)
                                    {
                                        found = true; break;
                                    }
                                    else { targShelf = null; }
                                }

                            }

                            if (found) { possibleStockPiles.Add(stockPile); }
                        }
                    }
                }
            }

            if (possibleStockPiles.Count > 0) { targStockPile = possibleStockPiles[Random.Range(0, possibleStockPiles.Count)]; }
            else if (Controller.Instance.stockPiles.Count > 0)
            {
                foreach (Building stockPile in Controller.Instance.stockPiles)
                {
                    bool found = false;

                    if (stockPile.transform.GetChild(1).childCount > 0 && stockPile.employeeQueue.Count < stockPile.maxQueue && stockPile.built)
                    {
                        foreach (Item item in stockPile.transform.GetChild(1).GetComponentsInChildren<Item>())
                        {
                            if (found) { break; }
                            if (targShelf == null)
                            {
                                foreach (Building shelf in Controller.Instance.shelves)
                                {
                                    if (item.itemTypeID == shelf.selectedItemTypeID && shelf.transform.GetChild(1).childCount < shelf.capacity && shelf.built)
                                    {
                                        found = true; break;
                                    }
                                }
                            }
                            else
                            {
                                if (item.itemTypeID == targShelf.selectedItemTypeID && targShelf.transform.GetChild(1).childCount < targShelf.capacity && targShelf.built)
                                {
                                    found = true; break;
                                }
                                else { targShelf = null; }
                            }

                        }

                        if (found) { possibleStockPiles.Add(stockPile); }
                    }
                }
            }


            if (possibleStockPiles.Count > 0) { targStockPile = possibleStockPiles[Random.Range(0, possibleStockPiles.Count)]; }
            /*
            else if (Controller.Instance.stockPiles.Count > 0)
            {
                int lowestAmount = Controller.Instance.stockPiles[0].maxQueue + 1;
                for (int i = 0; i < Controller.Instance.stockPiles.Count; i++) 
                { 
                    if (Controller.Instance.stockPiles[i].employeeQueue.Count < lowestAmount && Controller.Instance.stockPiles[i].transform.GetChild(1).childCount > 0 && Controller.Instance.stockPiles[i].employeeQueue.Count < Controller.Instance.stockPiles[i].maxQueue) 
                    { 
                        lowestAmount = Controller.Instance.stockPiles[i].employeeQueue.Count; targStockPile = Controller.Instance.stockPiles[i];
                    } 
                }
            }
            else { targStockPile = null; }
            */
        }

        if (targStockPile != null)
        {
            targBuilding = targStockPile;
            targ = targBuilding.transform;
            //targetPos = targBuilding.employeeLocation.position;
            targetPos = targBuilding.queueLocations[targBuilding.queueLocations.Count - 1].transform.position;
            employee.beforeLine = true;

            employee.SetPathfinding(currentPath, pathList, targetPos);
            employee.SetAI(targ, newTsk);
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
            /*
            targStockPile.AddToQueue(employee, null);

            if (targStockPile.employeeQueue.Count > 0)
            {

                if (targStockPile.employeeQueue[0] == employee && targStockPile.simultaneous)
                {
                    //targBuild.ChangeClaim(employee, null);
                    targBuilding = targStockPile;
                    targ = targBuilding.transform;
                    targetPos = targBuilding.employeeLocation.position;


                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
                else 
                {
                    targBuilding = targStockPile;
                    targ = targBuilding.transform;
                    targetPos = targBuilding.GetLinePosition(employee, null);


                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
            }
            else { Debug.Log("No queue"); employee.SwitchObjective(1); animator.SetTrigger("Failure"); }
            */
        }
        else
        {
            employee.SwitchObjective(1);
            animator.SetTrigger("Failure");
        }
    }
}
