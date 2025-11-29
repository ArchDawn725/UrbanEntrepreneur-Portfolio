using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
public class C_FindShelf : StateMachineBehaviour
{
    private List<Building> knownShelves = new List<Building>();
    private List<Building> possibleShelves = new List<Building>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        customer.OutAI(out Transform targ, out int newTsk);
        customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        targItemID = customer.shoppingList[0].itemID;
        possibleShelves.Clear();
        knownShelves.Clear();
        if (customer.Memory.Count > 0)
        {
            foreach (Building build in customer.Memory)
            {
                if (build.type == BuildingSO.Type.shelf)
                {
                    if (build.selectedItemTypeID == targItemID)
                    {
                        knownShelves.Add(build);
                    }
                }
            }
        }


        if (knownShelves.Count > 0)
        {
            foreach (Building shelf in knownShelves)
            {
                if (shelf.selectedItemTypeID == targItemID)
                {
                    if (shelf.transform.GetChild(1).childCount > 0 && shelf.allQueue.Count < shelf.maxQueue)
                    {
                        possibleShelves.Add(shelf);
                    }
                }
            }
        }

        if (possibleShelves.Count > 0) { targShelf = possibleShelves[Random.Range(0, possibleShelves.Count)]; }
        else { targShelf = null; }

        if (targShelf != null)
        {
            targBuilding = targShelf;
            customer.SetAI(targ, newTsk);
            customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
        }
        else 
        {
            customer.shoppingList.MoveFirstToLast();
            animator.SetTrigger("Failure"); 
        }
    }
}

