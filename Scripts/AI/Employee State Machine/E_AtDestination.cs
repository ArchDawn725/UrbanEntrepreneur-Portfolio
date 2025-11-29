using System.Collections.Generic;
using UnityEngine;
public class E_AtDestination : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();
        Officer officer = animator.GetComponent<Officer>();

        if (employee != null)
        {
            employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
            employee.OutAI(out Transform targ, out int newTsk);
            employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            pathList = null;
            employee.SetPathfinding(currentPath, pathList, targetPos);

            //rotate towards building
            if (targBuilding != null)
            {
                if (!employee.beforeLine) { employee.transform.position = targetPos; }

                if (targBuilding.employeeQueue.IndexOf(employee) == 0 && targetPos == targBuilding.employeeLocation.position)
                {
                    Vector3 direction = targBuilding.transform.GetChild(2).transform.position - employee.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    employee.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);
                }
                /*
                else if (targBuild.simultaneous)
                {
                    Vector3 direction = targBuild.GetLinePosition(targBuild.employeeQueue[targBuild.employeeQueue.IndexOf(employee) - 1], null) - employee.transform.position;//targBuild.transform.GetChild(2).transform.position - customer.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
                else//get position in any
                {
                    Vector3 direction = new Vector3(0, 0, 0);
                    if (targBuild.allQueue[targBuild.allQueue.IndexOf(employee.gameObject) - 1].GetComponent<Employee2>() != null)
                    { direction = targBuild.GetLinePosition(targBuild.allQueue[targBuild.allQueue.IndexOf(employee.gameObject) - 1].GetComponent<Employee2>(), null) - employee.transform.position; }

                    if (targBuild.allQueue[targBuild.allQueue.IndexOf(employee.gameObject) - 1].GetComponent<Customer2>() != null)
                    { direction = targBuild.GetLinePosition(null, targBuild.allQueue[targBuild.allQueue.IndexOf(employee.gameObject) - 1].GetComponent<Customer2>()) - employee.transform.position; }

                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
                */
                /*
                else
                {
                    Vector3 direction = targBuild.GetLinePosition(employee, null) - employee.transform.position;//targBuild.transform.GetChild(2).transform.position - customer.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
                */
            }


            //employee.GetGrid().taken = true;
        }

        if (customer != null)
        {
            customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
            customer.OutAI(out Transform targ, out int newTsk);
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            pathList = null;
            customer.SetPathfinding(currentPath, pathList, targetPos);

            //rotate towards building
            if (targBuilding != null)
            {
                if (targBuilding.customerQueue.IndexOf(customer) == 0 && targetPos == targBuilding.customerLocation.position)
                {
                    Vector3 direction = targBuilding.transform.GetChild(2).transform.position - customer.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    //vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    customer.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);
                }
                /*
                else
                {
                    Vector3 direction = targBuild.GetLinePosition(null, customer) -customer.transform.position;//targBuild.transform.GetChild(2).transform.position - customer.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
                */
            }
            if (customer.beforeLine) { }
            else if (!customer.patroling) { customer.transform.position = targetPos; }
            else { customer.patroling = false; }
            //customer.GetGrid().taken = true;
        }

        if (officer != null)
        {
            officer.pathVectorList.Clear();

            //rotate towards building
            if (officer.targetBuilding != null)
            {
                Vector3 direction = officer.targetBuilding.transform.GetChild(2).transform.position - officer.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                officer.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);
            }
        }

        //animator.transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Stop();
        if (employee != null) { employee.ToggleWalkingSounds(false); }
        if (customer != null) { customer.ToggleWalkingSounds(false); }
        animator.SetInteger("Animation", 0);
        animator.SetTrigger("Success");
    }
}
