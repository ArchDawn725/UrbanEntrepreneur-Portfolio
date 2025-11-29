using UnityEngine;
public class C_FindItems : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        /*
             public List<Item> possibleItems = new List<Item>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        employee.OutAI(out Transform targ, out Building targBuild, out int targItemID, out Item selectItem, out Building newTargBuilding, out int newTsk);

        possibleItems.Clear();

        if (employee.capacity > cont.childCount)
        {
            if (targBuild != null)
            {
                if (targItemID == 0)
                {
                    if (targBuild.transform.GetChild(1).childCount > 0)
                    {
                        foreach (Item item in targBuild.transform.GetChild(1).GetComponentsInChildren<Item>())
                        {
                            bool found = false;
                            foreach (Building shelf in Controller.Instance.shelves)
                            {
                                if (item.itemTypeID == shelf.selectedItemTypeID && shelf.capacity > shelf.transform.GetChild(1).childCount)// && shelf.customer == null)
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
                    if (targBuild.transform.GetChild(1).childCount > 0)
                    {
                        foreach (Item item in targBuild.transform.GetChild(1).GetComponentsInChildren<Item>())
                        {
                            bool found = false;
                            foreach (Building shelf in Controller.Instance.shelves)
                            {
                                if (item.itemTypeID == targItemID)
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
            if (targItemID != 0)
            {
                for (int i = 0; i < targBuild.transform.GetChild(1).childCount; i++)
                {
                    if (employee.capacity > cont.childCount)
                    {
                        int number = Random.Range(0, possibleItems.Count);
                        if (possibleItems[number].itemTypeID == targItemID)
                        {
                            selectItem = possibleItems[number];
                            employee.SetAI(targ, targBuild, targItemID, selectItem, newTargBuilding, newTsk);
                            animator.SetTrigger("Success");
                            return;
                        }
                    }
                    else { break; }
                }
            }
            else { if (!employee.messageCalled) { employee.messageCalled = true; UtilsClass.CreateWorldTextPopup("Could not find open shelves to stock!", employee.transform.position); } animator.SetTrigger("Failure"); Debug.Log("item is 0"); }
        }
        else { if (!employee.messageCalled) { employee.messageCalled = true; UtilsClass.CreateWorldTextPopup("Could not find open shelves to stock!", employee.transform.position); } animator.SetTrigger("Failure"); Debug.Log("Count is 0"); }
    }
         */

        /*
        if (objective != Objective.absent)
        {
            possibleItems.Clear();

            if (targetItem == null)
            {
                if (targetShelf != null)
                {
                    if (targetShelf.transform.GetChild(1).childCount > 0)
                    {
                        foreach (Item item in targetShelf.transform.GetChild(1).GetComponentsInChildren<Item>())//?
                        {
                            if (item.claimed == false)
                            {
                                possibleItems.Add(item);
                            }
                        }
                    }
                }

                if (possibleItems.Count > 0) { targetItem = possibleItems[Random.Range(0, possibleItems.Count)]; targetItem.claimed = true; }
            }

            if (targetItem != null)
            {
                if (targetShelf != null) { targetShelf.AddToQueue(null, this); }

                if (targetShelf.customerQueue.Count > 0)
                {
                    if (targetShelf.customerQueue[0] == this && targetShelf.employeeQueue.Count == 0)//&& !targetShelf.IfStaffed())//&& targetShelf.employeeQueue.Count == 0)
                    {
                        transform.GetChild(0).gameObject.SetActive(true);
                        transform.GetChild(1).gameObject.SetActive(true); GetComponent<BoxCollider2D>().enabled = true;
                        objective = Objective.toItems;
                        OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);
                        target = targetShelf.transform;
                        SetTargetPosition(targetShelf.transform.GetChild(0).transform.GetChild(3).transform.position);
                    }
                    else { waiting = true; }
                }
                else { waiting = true; }
            }
            else { waiting = true; }
        }
        else { AIStart(); }
         */
    }
}
