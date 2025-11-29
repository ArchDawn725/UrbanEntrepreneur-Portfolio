using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
public class E_FindShelf : StateMachineBehaviour
{
    private List<Building> possibleShelves = new List<Building>();
    private Employee2 employee;
    private Building targetShelf;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        employee = animator.GetComponent<Employee2>();
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
        targetShelf = targShelf;

        if (targetShelf != null)
        {
            if (targetShelf.capacity <= targetShelf.transform.GetChild(1).childCount || targetShelf.selectedItemTypeID != targItemID) { targetShelf = null; }//need remove from queue?
        }


        if (targetShelf == null)
        {
            possibleShelves.Clear();
            //item id
            if (Controller.Instance.shelves.Count > 0)
            {
                foreach (Building shelf in Controller.Instance.shelves)
                {
                    if (shelf.capacity > shelf.transform.GetChild(1).childCount && shelf.selectedItemTypeID == targItemID)// && shelf.customer == null)
                    {
                        if (shelf.employeeQueue.Count == 0 && shelf.customerQueue.Count == 0 && shelf.built)//(shelf.employee == null || shelf.employee == employee)
                        {
                            possibleShelves.Add(shelf);
                        }
                    }
                }
            }


            if (possibleShelves.Count > 0) { FindNearest(); }// targShelf = possibleShelves[Random.Range(0, possibleShelves.Count)]; }
            else
            {
                //find any compatable
                if (Controller.Instance.shelves.Count > 0)
                {
                    foreach (Building shelf in Controller.Instance.shelves)
                    {
                        if (shelf.capacity > shelf.transform.GetChild(1).childCount && shelf.selectedItemTypeID == targItemID && shelf.allQueue.Count < shelf.maxQueue && shelf.built)// && shelf.customer == null)
                        {
                            possibleShelves.Add(shelf);
                        }
                    }
                }
            }

            if (possibleShelves.Count > 0) { FindNearest(); }// targShelf = possibleShelves[Random.Range(0, possibleShelves.Count)]; }
            else if (Controller.Instance.shelves.Count > 0)
            {
                int lowestAmount = Controller.Instance.shelves[0].maxQueue + 1;
                for (int i = 0; i < Controller.Instance.shelves.Count; i++) { if (Controller.Instance.shelves[i].employeeQueue.Count < lowestAmount && Controller.Instance.shelves[i].selectedItemTypeID == targItemID && Controller.Instance.shelves[i].capacity > Controller.Instance.shelves[i].transform.GetChild(1).childCount && Controller.Instance.shelves[i].allQueue.Count < Controller.Instance.shelves[i].maxQueue && Controller.Instance.shelves[i].built) { lowestAmount = Controller.Instance.shelves[i].employeeQueue.Count; targShelf = Controller.Instance.shelves[i]; } }
            }
            else { targShelf = null; }
        }


        if (targetShelf != null)
        {
            targBuilding = targetShelf;
            targ = targBuilding.transform;
            //targetPos = targBuilding.employeeLocation.position;
            targetPos = targBuilding.queueLocations[targBuilding.queueLocations.Count - 1].transform.position;
            employee.beforeLine = true;

            employee.SetPathfinding(currentPath, pathList, targetPos);
            employee.SetAI(targ, newTsk);
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targetShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
            /*
            targetShelf.AddToQueue(employee, null);

            if (targetShelf.employeeQueue.Count > 0)
            {
                if (targetShelf.employeeQueue[0] == employee && targetShelf.simultaneous)// && !targetShelf.IfStaffed())//&& targetShelf.employeeQueue.Count == 0)
                {
                    //targBuild.ChangeClaim(employee, null);
                    targBuilding = targetShelf;
                    targ = targBuilding.transform;
                    targetPos = targBuilding.employeeLocation.position;

                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targetShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
                else 
                {
                    targBuilding = targetShelf;
                    targ = targBuilding.transform;
                    targetPos = targBuilding.GetLinePosition(employee, null);


                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    employee.SetAI(targ, newTsk);
                    employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targetShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
            }
            else { Debug.Log("No queue"); animator.SetTrigger("Failure"); }
            */

        }
        else { if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("Could not find open shelves to stock!",1 ,2); } animator.SetTrigger("Failure"); }//{ Returning(); }//Invoke("Returning", 1); }
    }

    private void FindNearest()
    {
        Building choosen = null;
        float shortest = 999999999999;
        foreach(Building shelf in possibleShelves)
        {
            if (Vector3.Distance(employee.transform.position, shelf.transform.position) < shortest)
            {
                choosen = shelf;
                shortest = Vector3.Distance(employee.transform.position, shelf.transform.position);
            }
        }

        targetShelf = choosen;
    }
}
