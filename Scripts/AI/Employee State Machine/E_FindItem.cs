using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
public class E_FindItem : StateMachineBehaviour
{
    private List<Item> possibleItems = new List<Item>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (targItemID != -1)
        {
            bool found = false;
            foreach (Building shelf in Controller.Instance.shelves)
            {
                if (shelf.selectedItemTypeID == targItemID && shelf.built)
                {
                    if (shelf.transform.GetChild(1).childCount < shelf.capacity) { found = true; break; }
                }
            }

            if (!found) { targItemID = -1; }
        }

        possibleItems.Clear();

        if (employee.capacity > cont.childCount)
        {
            if (targBuilding != null)
            {
                if (targItemID == -1)
                {
                    if (targBuilding.transform.GetChild(1).childCount > 0)
                    {
                        foreach (Item item in targBuilding.transform.GetChild(1).GetComponentsInChildren<Item>())
                        {
                            bool found = false;
                            foreach (Building shelf in Controller.Instance.shelves)
                            {
                                if (item.itemTypeID == shelf.selectedItemTypeID && shelf.capacity > shelf.transform.GetChild(1).childCount && shelf.allQueue.Count < shelf.maxQueue && shelf.built)// && shelf.customer == null)
                                {
                                    //if (shelf.employeeQueue.Count == 0)//if (shelf.employee == null || shelf.employee == employee)
                                    //{
                                        found = true; break;
                                    //}
                                }
                            }
                            if (found) { possibleItems.Add(item); }
                        }
                    }

                    if (possibleItems.Count > 0) { targItemID = possibleItems[Random.Range(0, possibleItems.Count)].itemTypeID; }
                }
                else
                {
                    if (targBuilding.transform.GetChild(1).childCount > 0)
                    {
                        foreach (Item item in targBuilding.transform.GetChild(1).GetComponentsInChildren<Item>())
                        {
                            bool found = false;
                            foreach (Building shelf in Controller.Instance.shelves)
                            {
                                if (item.itemTypeID == targItemID && shelf.built)
                                {
                                    found = true; break;
                                }
                            }
                            if (found) { possibleItems.Add(item); }
                        }
                    }
                }
            }
        }

        if (possibleItems.Count > 0)
        {
            if (targItemID != -1)
            {
                for (int i = 0; i < targBuilding.transform.GetChild(1).childCount; i++)
                {
                    if (employee.capacity > cont.childCount)
                    {
                        int number = Random.Range(0, possibleItems.Count);
                        if (possibleItems[number].itemTypeID == targItemID)
                        {
                            selectItem = possibleItems[number];
                            employee.SetAI(targ, newTsk);
                            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                            animator.SetTrigger("Success");
                            return;
                        }
                    }
                    //else { break; }
                }
            }
            else { if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("Could not find open shelves to stock!", 1, 2); } animator.SetTrigger("Failure"); }
        }
        else if (cont.childCount > 0) 
        {
            animator.SetTrigger("Other"); return;
        }
        else {
            if (!employee.messageCalled)
            {
                employee.messageCalled = true;
                //UtilsClass.CreateWorldTextPopup("Could not find open shelves to stock!", employee.transform.position);
            }

            targItemID = -1;
            targBuilding = null;
            targStockPile = null;
            employee.SetAI(targ, newTsk);
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Failure");
            employee.RemoveAllClaims();
            return;
        }

        animator.SetTrigger("Failure");
    }
}
