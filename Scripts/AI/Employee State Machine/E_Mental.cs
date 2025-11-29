using UnityEngine;
public class E_Mental : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();

        switch (animator.GetInteger("MentalBreak"))
        {
            //murder
            case 1:
                employee.TalkBubble("Die Die!!!", 0, 3);
                if (employee.targetCustomer != null)
                {
                    employee.targetCustomer.animator.SetBool("Dead", true);
                    employee.targetCustomer.transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
                    foreach (Customer2 customer in Controller.Instance.customers) { customer.storePreferance[0] -= 50; }
                    foreach (Employee2 employees in Controller.Instance.employees) { employees.AddStress(10); }
                }
                /*
                else if (employee.targetEmployee != null)
                {

                }
                */
                break;
                //fire
            case 2:
                employee.TalkBubble("More Fire!!", 0, 3);
                if (employee.targetBuilding != null)
                {
                    employee.targetBuilding.SetFire();
                }

                break;
                //anger
            case 3:
                employee.TalkBubble("**** you!!!", 0, 3);
                if (employee.targetCustomer != null)
                {
                    employee.targetCustomer.ContinueInteraction(0);
                }
                else if (employee.targetEmployee != null)
                {
                    employee.targetEmployee.Insulted();
                }

                break;
        }

        animator.SetTrigger("Success");
    }
}
