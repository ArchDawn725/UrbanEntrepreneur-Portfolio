using System.Collections.Generic;
using UnityEngine;
public class C_GoingToLine : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        customer.OutAI(out Transform targ, out int newTsk);
        customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (targBuilding != null)
        {
            if (!targBuilding.customerQueue.Contains(customer))
            {
                targBuilding.AddToQueue(null, customer);
                targetPos = targBuilding.GetLinePosition(null, customer);
                customer.beforeLine = false;
                customer.SetPathfinding(currentPath, pathList, targetPos);
                animator.SetTrigger("Success");
                return;
            }

            if (targBuilding.customerQueue.Count > 0)
            {
                if (targBuilding.customerQueue[0] == customer && targBuilding.simultaneous)
                {
                    targ = targBuilding.transform;
                    targetPos = targBuilding.customerLocation.position;

                    customer.SetPathfinding(currentPath, pathList, targetPos);
                    customer.SetAI(targ, newTsk);
                    animator.SetTrigger("Success");
                    return;
                }
                else
                {
                    targ = targBuilding.transform;
                    targetPos = targBuilding.GetLinePosition(null, customer);


                    customer.SetPathfinding(currentPath, pathList, targetPos);
                    customer.SetAI(targ, newTsk);
                    animator.SetTrigger("Success");
                    return;
                }
            }
            else { Debug.Log("No queue"); }
        }
        else { Debug.Log("No queue"); }

        animator.SetTrigger("Failure");
    }
}
