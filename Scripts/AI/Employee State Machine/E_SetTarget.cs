using ArchDawn.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class E_SetTarget : StateMachineBehaviour
{
    [SerializeField] private bool debugger;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();
        Officer officer = animator.GetComponent<Officer>();

        if (employee != null)
        {
            employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            if (animator.GetBool("OnShift")) { employee.SwitchObjective(2); }
            currentPath = 0;
            employee.RemoveClaim();
            pathList = MapController.Instance.FindPath(employee.GetPosition(), targetPos, false, false);

            if (pathList != null && pathList.Count > 1)
            {
                pathList.RemoveAt(0);
                employee.SetPathfinding(currentPath, pathList, targetPos);
                animator.SetTrigger("Success");
            }
            else if ((Vector3.Distance(employee.GetPosition(), targetPos) < 10f))
            {
                animator.SetTrigger("Failure");
            }
            else
            {
                if (employee.beforeLine) { if (employee.targetBuilding != null) { employee.targetBuilding.ResetQueuePositions(true); } }
                if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("I cannot move there!", 1, 2); }// UtilsClass.CreateWorldTextPopup("Cannot move there!", employee.transform.position); }
                animator.SetTrigger("Other");
            }
            employee.stuckCalls = 0;
        }

        if (customer != null)
        {
            customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            //customer.SwitchObjective(2);
            currentPath = 0;
            pathList = MapController.Instance.FindPath(customer.GetPosition(), targetPos, true, false);
            customer.RemoveClaim();

            if (pathList != null && pathList.Count > 1)
            {
                pathList.RemoveAt(0);
                customer.SetPathfinding(currentPath, pathList, targetPos);
                animator.SetTrigger("Success");
            }
            else if ((Vector3.Distance(customer.GetPosition(), targetPos) < 10f))
            {
                if (debugger && customer.beforeLine) { Debug.Log("Custoemr already at entrance"); }
                animator.SetTrigger("Failure");
            }
            else
            {
                if (customer.GetGrid() != null)
                {
                    if (!MapController.Instance.custoemrAllowedZones[customer.GetGrid().zone])
                    {
                        customer.StuckPositionFailSafe();
                        OnStateEnter(animator, stateInfo, layerIndex);
                        return;
                    }
                    else
                    {
                        if (customer.beforeLine) { if (customer.targetBuilding != null) { customer.targetBuilding.ResetQueuePositions(false); } }
                        if (!customer.messageCalled) { customer.messageCalled = true; customer.TalkBubble("I cannot move there!", 1, 2); }//UtilsClass.CreateWorldTextPopup("Cannot move there!", customer.transform.position); }
                        if (debugger) { Debug.Log("Custoemr could not move to entrance"); }
                        animator.SetTrigger("Other");
                    }
                }
                else
                {
                    if (debugger) { Debug.Log("Custoemr not on grid"); }
                    animator.SetTrigger("Other");
                }
            }
        }

        if (officer != null)
        {
            officer.currentPathIndex = 0;
            officer.pathVectorList = MapController.Instance.FindPath(officer.transform.position, officer.targetPosition, false, false);

            if (officer.pathVectorList != null && officer.pathVectorList.Count > 1)
            {
                officer.pathVectorList.RemoveAt(0);
                animator.SetTrigger("Success");
            }
            else if ((Vector3.Distance(officer.transform.position, officer.targetPosition) < 10f))
            {
                animator.SetTrigger("Failure");
            }
            else
            {
                officer.TalkBubble("I cannot move there!", 1, 2);
                animator.SetTrigger("Other");
            }
        }

    }
}
