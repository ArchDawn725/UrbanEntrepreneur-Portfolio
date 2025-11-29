using System.Collections.Generic;
using UnityEngine;
public class E_Leave : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null)
        {
            employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

            employee.SwitchObjective(0);
            box.enabled = false;
            /*
            float Distance = 99999999999; int selectedEntrance = 0;
            if (Controller.Instance.employeeEntrances.Count > 0)
            {
                for (int i = 0; i < Controller.Instance.employeeEntrances.Count; i++)
                {
                    float dist = Vector3.Distance(employee.transform.position, Controller.Instance.employeeEntrances[i].position);
                    if (dist < Distance) { Distance = dist; selectedEntrance = i; }
                }
                targetPos = Controller.Instance.employeeEntrances[selectedEntrance].position;
            }
            else if (Controller.Instance.anyoneEntrances.Count > 0)
            {
                for (int i = 0; i < Controller.Instance.anyoneEntrances.Count; i++)
                {
                    float dist = Vector3.Distance(employee.transform.position, Controller.Instance.anyoneEntrances[i].position);
                    if (dist < Distance) { Distance = dist; selectedEntrance = i; }
                }
                targetPos = Controller.Instance.anyoneEntrances[selectedEntrance].position;
            }
            else
            {
                for (int i = 0; i < Controller.Instance.entrances.Count; i++)
                {
                    float dist = Vector3.Distance(employee.transform.position, Controller.Instance.entrances[i].position);
                    if (dist < Distance) { Distance = dist; selectedEntrance = i; }
                }
                targetPos = Controller.Instance.entrances[selectedEntrance].position;
            }
            */
            targetPos = Controller.Instance.startingPoints[Random.Range(0, Controller.Instance.startingPoints.Count)].position;
            employee.SetPathfinding(currentPath, pathList, targetPos);
            employee.insideStore = false;
        }
        
        if (customer != null)
        {
            customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
            customer.OutAI(out Transform targ, out int newTsk);
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            //customer.SwitchObjective(0)
            if (targBuilding != null) { targBuilding.RemoveFromQueue(null, customer); }//targBuild.employee = null; };
            targ = null;
            targBuilding = null;
            targRegistor = null; targShelf = null; targStockPile = null;
            targItemID = 0;
            selectItem = null;
            box.enabled = false;

            if (customer.shoppingList.Count > 0)
            {
                customer.storePreferance[0] -= customer.shoppingList.Count;
            }
            if (customer.couldNotAffordList.Count > 0)
            {
                customer.storePreferance[0] -= customer.shoppingList.Count * 2;
            }

            animator.SetBool("AtSite", false);
            if (customer.entrance != null) { customer.entrance.ChangeClaim(customer); }

            /*
            float Distance = 99999999999; int selectedEntrance = 0;
            if (Controller.Instance.customerEntrances.Count > 0)
            {
                for (int i = 0; i < Controller.Instance.customerEntrances.Count; i++)
                {
                    float dist = Vector3.Distance(customer.transform.position, Controller.Instance.customerEntrances[i].position);
                    if (dist < Distance) { Distance = dist; selectedEntrance = i; }
                }
                targetPos = Controller.Instance.customerEntrances[selectedEntrance].position;
            }
            else if (Controller.Instance.anyoneEntrances.Count > 0)
            {
                for (int i = 0; i < Controller.Instance.anyoneEntrances.Count; i++)
                {
                    float dist = Vector3.Distance(customer.transform.position, Controller.Instance.anyoneEntrances[i].position);
                    if (dist < Distance) { Distance = dist; selectedEntrance = i; }
                }
                targetPos = Controller.Instance.anyoneEntrances[selectedEntrance].position;
            }
            else
            {
                for (int i = 0; i < Controller.Instance.entrances.Count; i++)
                {
                    float dist = Vector3.Distance(customer.transform.position, Controller.Instance.entrances[i].position);
                    if (dist < Distance) { Distance = dist; selectedEntrance = i; }
                }
                targetPos = Controller.Instance.entrances[selectedEntrance].position;
            }
            */
            targetPos = Controller.Instance.startingPoints[Random.Range(0, Controller.Instance.startingPoints.Count)].position;
            customer.SetPathfinding(currentPath, pathList, targetPos);
            customer.SetAI(targ, newTsk);
            customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            Controller.Instance.activeCustomers--;
            customer.insideStore = false;
        }

        animator.SetTrigger("Success");
    }
}
