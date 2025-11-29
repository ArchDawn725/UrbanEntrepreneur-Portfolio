using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Enter : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<Officer>().targetPosition == new Vector3(-1,-1,-1))
        {
            switch (animator.GetComponent<Officer>().type)
            {
                case 0:
                    foreach (Employee2 employee in Controller.Instance.employees)
                    {
                        //if mental break
                        if (employee.animator.GetInteger("MentalBreak") > 0)
                        {
                            animator.transform.GetComponent<Officer>().targetEmployee = employee;
                            animator.transform.GetComponent<Officer>().mentalBreakTarget = employee.gameObject;
                            animator.transform.GetComponent<Officer>().targetPosition = employee.StandNextToMe(null, animator.transform.GetComponent<Officer>());
                            //animator.transform.GetComponent<Officer>().targetPosition = employee.transform.position;
                            break;
                        }
                    }
                    break;
                case 1:
                    foreach (Customer2 customer in Controller.Instance.customers)
                    {
                        //if dead
                        if (customer.animator.GetBool("Dead"))
                        {
                            animator.transform.GetComponent<Officer>().targetCustomer = customer;
                            animator.transform.GetComponent<Officer>().mentalBreakTarget = customer.gameObject;
                            animator.transform.GetComponent<Officer>().targetPosition = customer.StandNextToMe(null, animator.transform.GetComponent<Officer>());
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (Building building in Controller.Instance.buildings)
                    {
                        //if on fire
                        if (building.onFire)
                        {
                            animator.transform.GetComponent<Officer>().targetBuilding = building;
                            animator.transform.GetComponent<Officer>().mentalBreakTarget = building.employeeLocation.gameObject;
                            animator.transform.GetComponent<Officer>().targetPosition = building.transform.GetChild(0).transform.GetChild(2).transform.position;
                            break;
                        }
                    }
                    break;
            }
        }

        if (animator.GetComponent<Officer>().targetPosition == new Vector3(-1, -1, -1)) 
        { 
            Debug.Log("Cannot find target");
            foreach (Officer officer in Controller.Instance.officers) { if (officer.type == 0 && officer.done) { animator.SetBool("Completed", true); } }
            animator.SetTrigger("Failure"); 
        }
        else { animator.SetTrigger("Success"); }
    }
}
