using System.Collections.Generic;
using UnityEngine;
public class O_Chase : StateMachineBehaviour
{
    Employee2 employee;
    Officer officer;

    int currentPathIndex;
    List<Vector3> pathVectorList;

    bool moving;
    private Transform visuals;

    GameObject target;
    Vector3 targetPosition;
    Animator animator;

    private Vector3 lastPosition;
    private MapController.NewGrid lastGrid;

    private float minDist = 2f;
    private float maxDist = 10;
    private float previousDistance = 100;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        employee = animator.GetComponent<Employee2>();
        officer = animator.GetComponent<Officer>();
        this.animator = animator;

        if (employee != null)
        {
            employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            visuals = vis;
        }

        animator.SetInteger("Animation", 1);
        GetTarget();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (target == null) { AtDestination(); return; }
        if (target.transform.position != targetPosition) { moving = false; GetTarget(); }
        if (moving) { Moving(); }
    }
    private void GetTarget()
    {
        if (employee != null)
        {
            target = employee.mentalBreakTarget;
            targetPosition = target.transform.position;
            currentPathIndex = 0;
            pathVectorList = MapController.Instance.FindPath(employee.GetPosition(), targetPosition, false, false);
            lastPosition = employee.transform.position;
            lastGrid = employee.GetGrid();
            previousDistance = 100;

            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
                employee.SetPathfinding(currentPathIndex, pathVectorList, targetPosition);
                moving = true;
            }
            else 
            {
                //animator.SetTrigger("Failure");
                AtDestination();
            }
            employee.stuckCalls = 0;
        }
        if (officer != null)
        {
            target = officer.mentalBreakTarget;
            targetPosition = target.transform.position;
            currentPathIndex = 0;
            pathVectorList = MapController.Instance.FindPath(officer.transform.position, targetPosition, false, false);
            lastPosition = officer.transform.position;
            previousDistance = 100;

            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
                moving = true;
            }
            else
            {
                //animator.SetTrigger("Failure");
                AtDestination();
            }
        }
    }
    private void Moving()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];

            if (employee != null)
            {
                if (Vector3.Distance(employee.transform.position, lastPosition) > maxDist * 2) { employee.transform.position = targetPosition; }
                if (Vector3.Distance(employee.transform.position, targetPosition) > previousDistance) { employee.transform.position = targetPosition; previousDistance = 200; }
                else { previousDistance = Vector3.Distance(employee.transform.position, targetPosition); }
                if (currentPathIndex == pathVectorList.Count - 1)
                {
                    if (Vector3.Distance(employee.transform.position, targetPosition) > minDist && Vector3.Distance(employee.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - employee.transform.position).normalized;

                        Vector3 direction = targetPosition - employee.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        employee.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);

                        employee.transform.position = employee.transform.position + moveDir * employee.speedCalc * Time.fixedDeltaTime;
                    }
                    else
                    {
                        pathVectorList = null;
                        animator.SetTrigger("Success");
                        return;
                    }
                }
                else
                {
                    if (Vector3.Distance(employee.transform.position, targetPosition) > minDist && Vector3.Distance(employee.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - employee.transform.position).normalized;

                        Vector3 direction = targetPosition - employee.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        employee.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);

                        employee.transform.position = employee.transform.position + moveDir * employee.speedCalc * Time.fixedDeltaTime;
                    }
                    else
                    {
                        lastPosition = targetPosition;
                        currentPathIndex++;
                        previousDistance = 200;
                    }
                }
                if (employee.GetGrid() != lastGrid) { employee.EnterNewTile(employee.GetGrid()); lastGrid = employee.GetGrid(); }
            }

            if (officer != null)
            {
                if (Vector3.Distance(officer.transform.position, lastPosition) > maxDist * 2) { officer.transform.position = targetPosition; }
                if (Vector3.Distance(officer.transform.position, targetPosition) > previousDistance) { officer.transform.position = targetPosition; previousDistance = 200; }
                else { previousDistance = Vector3.Distance(officer.transform.position, targetPosition); }
                if (currentPathIndex == pathVectorList.Count - 1)
                {
                    if (Vector3.Distance(officer.transform.position, targetPosition) > minDist && Vector3.Distance(officer.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - officer.transform.position).normalized;

                        Vector3 direction = targetPosition - officer.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        //visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        officer.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);

                        officer.transform.position = officer.transform.position + moveDir * officer.GetSpeed() * Time.fixedDeltaTime;
                    }
                    else
                    {
                        pathVectorList = null;
                        animator.SetTrigger("Success");
                        return;
                    }
                }
                else
                {
                    if (Vector3.Distance(officer.transform.position, targetPosition) > minDist && Vector3.Distance(officer.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - officer.transform.position).normalized;

                        Vector3 direction = targetPosition - officer.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        //visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        officer.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);

                        officer.transform.position = officer.transform.position + moveDir * officer.GetSpeed() * Time.fixedDeltaTime;
                    }
                    else
                    {
                        lastPosition = targetPosition;
                        currentPathIndex++;
                        previousDistance = 200;
                    }
                }
            }
        }
    }
    private void AtDestination()
    {
        if (employee != null) { employee.ToggleWalkingSounds(false); }
        animator.SetInteger("Animation", 0);
        animator.SetTrigger("Success");
    }
}
