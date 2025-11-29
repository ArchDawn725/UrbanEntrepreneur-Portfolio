using System.Collections.Generic;
using UnityEngine;
public class C_ClaimItem : StateMachineBehaviour
{
    private List<Item> possibleItems = new List<Item>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        customer.OutAI(out Transform targ, out int newTsk);
        customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        possibleItems.Clear();

        if (selectItem == null)
        {
            if (targShelf.transform.GetChild(1).childCount > 0)
            {
                foreach (Item item in targShelf.transform.GetChild(1).GetComponentsInChildren<Item>())
                {
                    if (item.itemTypeID == targItemID && !item.claimed)
                    {
                        possibleItems.Add(item);
                    }
                }
            }

            if (possibleItems.Count > 0) { selectItem = possibleItems[Random.Range(0, possibleItems.Count)]; selectItem.claimed = true; }
        }

        if (selectItem != null)
        {
            targ = targShelf.transform;
            customer.SetAI(targ, newTsk);
            targBuilding = targShelf;
            //targetPos = targShelf.customerLocation.position;
            targetPos = targShelf.queueLocations[targShelf.queueLocations.Count - 1].transform.position;
            customer.SetPathfinding(currentPath, pathList, targetPos);
            customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            customer.beforeLine = true;
            animator.SetTrigger("Success");
            /*
            targShelf.AddToQueue(null, customer);

            if (targShelf.customerQueue.Count > 0)
            {
                if (targShelf.customerQueue[0] == customer && targShelf.simultaneous)
                {
                    targ = targShelf.transform;
                    targetPos = targShelf.customerLocation.position;
                    targBuilding = targShelf;

                    customer.SetPathfinding(currentPath, pathList, targetPos);
                    customer.SetAI(targ, newTsk);
                    customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
                else
                {
                    targ = targShelf.transform;
                    targetPos = targShelf.GetLinePosition(null, customer);
                    targBuilding = targShelf;

                    customer.SetPathfinding(currentPath, pathList, targetPos);
                    customer.SetAI(targ, newTsk);
                    customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
            }
            else { Debug.Log("No queue"); animator.SetTrigger("Failure"); }
            */
        }
        else { animator.SetTrigger("Failure"); }
    }
}
