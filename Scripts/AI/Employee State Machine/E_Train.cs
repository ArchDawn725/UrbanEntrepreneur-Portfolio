using UnityEngine;
public class E_Train : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);

        if (employee.targetEmployee != null)
        {
            employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] -= managementSkill;
            employee.targetEmployee.XPIncrease((int)employee.targetEmployee.task);
        }

        if (employee.targetEmployee != null) { if (employee.targetEmployee.animator.GetBool("Fired") || !employee.targetEmployee.animator.GetBool("OnShift")) { employee.targetEmployee.manager = null; employee.targetEmployee = null; } } 
        if (employee.targetEmployee != null) { if (employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] <= 0) { employee.targetEmployee.manager = null; employee.targetEmployee = null; } }

        animator.SetTrigger("Success");
    }
}
