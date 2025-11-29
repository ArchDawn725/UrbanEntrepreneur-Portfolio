using System.Collections.Generic;
using UnityEngine;
public class E_InLine : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null)
        {
            employee.OutAI(out Transform targ, out int newTsk);
            employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
            employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            if (targBuilding != null)
            {
                if (!targBuilding.employeeQueue.Contains(employee))
                {
                    targBuilding.AddToQueue(employee, null);
                    targetPos = targBuilding.GetLinePosition(employee, null);
                    employee.beforeLine = false;
                    employee.SetPathfinding(currentPath, pathList, targetPos);
                    animator.SetTrigger("Other");
                    return;
                }

                if (targBuilding.employeeQueue.IndexOf(employee) == 0 && targetPos == targBuilding.employeeLocation.position)
                {
                    animator.SetTrigger("Success");
                }
                else
                {
                    targetPos = targBuilding.GetLinePosition(employee, null);

                    employee.SetPathfinding(currentPath, pathList, targetPos);

                    animator.SetTrigger("Other");
                }
            }
            else { animator.SetTrigger("Failure"); }
        }

        if (customer != null)
        {
            customer.OutAI(out Transform targ, out int newTsk);
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
            customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            if (targBuilding != null)
            {
                if (!targBuilding.customerQueue.Contains(customer))
                {
                    animator.SetTrigger("Failure");
                    return;
                }



                if (targBuilding.customerQueue.IndexOf(customer) == 0 && targetPos == targBuilding.customerLocation.position)
                {
                    animator.SetTrigger("Success");
                }
                else
                {
                    animator.SetTrigger("Failure");
                }
            }
            else { animator.SetTrigger("Failure"); }
        }
    }
}
