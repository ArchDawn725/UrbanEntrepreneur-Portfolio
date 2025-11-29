using System.Collections.Generic;
using UnityEngine;
public class C_StoreLine : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

        if (!customer.insideStore)
        {
            if (!customer.entrance.customerQueue.Contains(customer))
            {
                if (Vector3.Distance(customer.transform.position, customer.entrance.customerLocation.position) > 25 && customer.beforeLine) { animator.SetTrigger("Failure"); return; }
                customer.entrance.AddToQueue(customer);
                targetPos = customer.entrance.GetLinePosition(customer);
                customer.beforeLine = false;
                customer.SetPathfinding(currentPath, pathList, targetPos);
                animator.SetTrigger("Failure");
                return;
            }

            if (customer.entrance.customerQueue.IndexOf(customer) == 0 && targetPos == customer.entrance.customerLocation.position)
            {
                int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
                if (time >= Controller.Instance.storeOpen && time < Controller.Instance.storeClose && Controller.Instance.storeOpenDays[UIController.Instance.weekday]) { animator.SetTrigger("Success"); return; }
                else
                {
                    animator.SetTrigger("Failure");
                    return;
                }
            }
            else
            {
                targetPos = customer.entrance.GetLinePosition(customer);

                customer.SetPathfinding(currentPath, pathList, targetPos);

                animator.SetTrigger("Failure");
                return;
            }

            animator.SetTrigger("Failure");
        }
        else { animator.SetTrigger("Success"); }
    }
}
