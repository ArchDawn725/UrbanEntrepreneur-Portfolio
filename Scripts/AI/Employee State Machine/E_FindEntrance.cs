using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
public class E_FindEntrance : StateMachineBehaviour
{
    [SerializeField] private bool leaving;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null)
        {
            employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
            if (!employee.insideStore)
            {
                if (Controller.Instance.employeeEntrances.Count > 0) { targetPos = Controller.Instance.employeeEntrances[Random.Range(0, Controller.Instance.employeeEntrances.Count)].entranceNode.transform.position; }
                else if (Controller.Instance.anyoneEntrances.Count > 0) { targetPos = Controller.Instance.anyoneEntrances[Random.Range(0, Controller.Instance.anyoneEntrances.Count)].entranceNode.transform.position; }
                else { targetPos = Controller.Instance.entrances[Random.Range(0, Controller.Instance.entrances.Count)].entranceNode.transform.position; }
            }
            else { targetPos = employee.transform.position; }

            employee.SetPathfinding(currentPath, pathList, targetPos);
        }

        if (customer != null)
        {
            customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
            Wall targetEntrance = null;

            if (Controller.Instance.customerEntrances.Count > 0) { targetEntrance = Controller.Instance.customerEntrances[Random.Range(0, Controller.Instance.customerEntrances.Count)]; }
            else if (Controller.Instance.anyoneEntrances.Count > 0) { targetEntrance = Controller.Instance.anyoneEntrances[Random.Range(0, Controller.Instance.anyoneEntrances.Count)]; }
            else { targetEntrance = Controller.Instance.entrances[Random.Range(0, Controller.Instance.entrances.Count)]; }

            if (leaving)
            {
                targetPos = targetEntrance.entranceNode.transform.position;
            }
            else
            {
                if (!customer.insideStore)
                {
                    targetPos = targetEntrance.queueLocations[0].transform.position;
                }
                else 
                {
                    targetPos = customer.transform.position; 
                }

                /*
                targetEntrance.AddToQueue(customer);

                targetPos = targetEntrance.GetLinePosition(customer);
                */
            }
            customer.SetPathfinding(currentPath, pathList, targetPos);
            customer.entrance = targetEntrance;
            customer.beforeLine = true;

        }

        animator.SetTrigger("Success");
    }
}
