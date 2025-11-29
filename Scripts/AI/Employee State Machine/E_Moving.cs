using System.Collections.Generic;
using UnityEngine;
public class E_Moving : StateMachineBehaviour
{
    private Employee2 employee;
    private Customer2 customer;
    private Officer officer;

    private Transform visuals;

    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private Vector3 lastPosition;
    private MapController.NewGrid lastGrid;

    private float minDist = 2f;
    private float maxDist = 10;
    private float previousDistance = 100;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        previousDistance = 100;
        employee = animator.GetComponent<Employee2>();
        customer = animator.GetComponent<Customer2>();
        officer = animator.GetComponent<Officer>();

        if (employee != null)
        {
            employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            visuals = vis;
            currentPathIndex = currentPath; pathVectorList = pathList;
            lastPosition = employee.transform.position;
            lastGrid = employee.GetGrid();
        }

        if (customer != null)
        {
            customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            visuals = vis;
            currentPathIndex = currentPath; pathVectorList = pathList;
            lastPosition = customer.transform.position;
            lastGrid = customer.GetGrid();
        }

        if (officer != null)
        {
            currentPathIndex = officer.currentPathIndex; pathVectorList = officer.pathVectorList;
            lastPosition = officer.transform.position;
        }

        //animator.transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Play();
        if (employee != null) { employee.ToggleWalkingSounds(true); }
        if (customer != null) { customer.ToggleWalkingSounds(true); }
        animator.SetInteger("Animation", 1);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];


            if (employee != null)
            {
                if (Vector3.Distance(employee.transform.position, lastPosition) > maxDist * 2) { employee.transform.position = targetPosition; }
                if (!employee.beforeLine)
                {
                    if (Vector3.Distance(employee.transform.position, targetPosition) > previousDistance) { employee.transform.position = targetPosition; previousDistance = 200; }
                    else { previousDistance = Vector3.Distance(employee.transform.position, targetPosition); }
                }
                if (employee.beforeLine) { if (Vector3.Distance(employee.transform.position, pathVectorList[pathVectorList.Count - 1]) < maxDist) { pathVectorList = null; animator.SetTrigger("Success"); return; } }
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
            

            if (customer != null)
            {
                if (Vector3.Distance(customer.transform.position, lastPosition) > maxDist * 2) { customer.transform.position = targetPosition; }
                if (!customer.beforeLine)
                {
                    if (Vector3.Distance(customer.transform.position, targetPosition) > previousDistance) { customer.transform.position = targetPosition; previousDistance = 200; }
                    else { previousDistance = Vector3.Distance(customer.transform.position, targetPosition); }
                }
                if (customer.beforeLine) { if (Vector3.Distance(customer.transform.position, pathVectorList[pathVectorList.Count - 1]) < maxDist) { pathVectorList = null; animator.SetTrigger("Success"); return; } }
                if (currentPathIndex == pathVectorList.Count - 1)
                {
                    if (Vector3.Distance(customer.transform.position, targetPosition) > minDist && Vector3.Distance(customer.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - customer.transform.position).normalized;

                        Vector3 direction = targetPosition - customer.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        customer.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);


                        //float distanceBefore = Vector3.Distance(customer.transform.position, targetPosition);
                        customer.transform.position = customer.transform.position + moveDir * customer.speedCalc * Time.fixedDeltaTime;
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
                    if (Vector3.Distance(customer.transform.position, targetPosition) > minDist && Vector3.Distance(customer.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - customer.transform.position).normalized;

                        Vector3 direction = targetPosition - customer.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        customer.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);


                        //float distanceBefore = Vector3.Distance(customer.transform.position, targetPosition);
                        customer.transform.position = customer.transform.position + moveDir * customer.speedCalc * Time.fixedDeltaTime;
                    }
                    else
                    {
                        lastPosition = targetPosition;
                        currentPathIndex++;
                        previousDistance = 200;
                        /*
                        if (currentPathIndex >= pathVectorList.Count)
                        {
                            pathVectorList = null;
                            animator.SetTrigger("Success");
                        }
                        */
                    }
                }
                if (customer.GetGrid() != lastGrid) { customer.EnterNewTile(customer.GetGrid()); lastGrid = customer.GetGrid(); }
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
                        //officer.visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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

        if (TickSystem.Instance.timeMultiplier == 0) { animator.transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Stop(); }
    }
}
