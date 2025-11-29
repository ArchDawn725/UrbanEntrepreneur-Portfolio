using System.Collections.Generic;
using UnityEngine;

public class E_MentalBreak : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        bool found = false;
        employee.targetBuilding = null;
        employee.targetEmployee = null;
        employee.targetCustomer = null;
        employee.targetPosition = new Vector3(-1, -1, -1);

        switch (animator.GetInteger("MentalBreak"))
        {
                //murder
            case 1:
                foreach (Customer2 customer in Controller.Instance.customers)
                {
                    employee.TalkBubble("Kill Kill!!!", 0, 3);
                    if (!customer.animator.GetBool("Dead") && customer.animator.GetBool("AtSite") && customer.insideStore == true)
                    {
                        employee.targetCustomer = customer;
                        employee.mentalBreakTarget = customer.gameObject;
                        employee.targetPosition = customer.StandNextToMe(employee, null);
                        found = true;
                        break;
                    }
                }
                /*
                if (!found)
                {
                    foreach (Employee2 employees in Controller.Instance.employees)
                    {
                        if (employees.animator.GetBool("OnShift") && employees.status != Employee2.Status.owner)
                        {
                            employee.targetEmployee = employees;
                            employee.targetPosition = employees.StandNextToMe(employee, null);
                            break;
                        }
                    }
                }
                */
                break;

                //fire
            case 2:
                foreach (Building building in Controller.Instance.buildings)
                {
                    employee.TalkBubble("Muahahaha!!!!!", 0, 3);
                    if (!building.onFire)
                    {
                        employee.targetBuilding = building;
                        employee.mentalBreakTarget = building.employeeLocation.gameObject;
                        employee.targetPosition = building.employeeLocation.position;
                        break;
                    }
                }
                break;

                //insult
            case 3:
                List<Customer2> possibleCustomers = new List<Customer2>();
                foreach (Customer2 customer in Controller.Instance.customers)
                {
                    //if dead
                    if (!customer.animator.GetBool("Dead") && customer.animator.GetBool("AtSite"))
                    {
                        possibleCustomers.Add(customer);
                    }
                }
                Customer2 newCustomer = possibleCustomers[Random.Range(0, possibleCustomers.Count)];
                employee.targetCustomer = newCustomer;
                employee.mentalBreakTarget = newCustomer.gameObject;
                employee.targetPosition = newCustomer.StandNextToMe(employee, null);
                if (!found)
                {
                    foreach (Employee2 employees in Controller.Instance.employees)
                    {
                        if (employees.animator.GetBool("OnShift"))
                        {
                            employee.targetEmployee = employees;
                            employee.mentalBreakTarget = employees.gameObject;
                            employee.targetPosition = employees.StandNextToMe(employee, null);
                            break;
                        }
                    }
                }
                break;
        }

        if (employee.targetPosition == new Vector3(-1, -1, -1)) { animator.SetTrigger("Failure"); }
        else { animator.SetTrigger("Success"); }
    }
}
