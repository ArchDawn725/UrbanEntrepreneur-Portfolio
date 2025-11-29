using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
public class E_FindTrainiee : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Employee2 targEmployee = null;
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        int thisEmployeeLevel = 0;
        float lowestTrainingRemaining = 2147483647;

        //check to see if player targeted a building
        if (employee.targetEmployee != null)
        {
            if (employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] <= 0) { if (employee.targetEmployee.manager = employee) { employee.targetEmployee.manager = null; }  employee.targetEmployee = null; }
        }

        //find level 0's that need training
        if (employee.targetEmployee == null)
        {
            //get all employees
            foreach (Employee2 aEmployee in FindObjectsOfType<Employee2>())
            {
                int employeeTaskLevel = 0;
                aEmployee.OutSkills(out int ainvSkill, out int acustSkill, out int ajanitorialSkill, out int aengineerSkill, out int amanagementSkill);
                switch ((int)aEmployee.task)
                {
                    case 1: employeeTaskLevel = ainvSkill; thisEmployeeLevel = invSkill; break;
                    case 2: employeeTaskLevel = acustSkill; thisEmployeeLevel = custSkill; break;
                    case 3: employeeTaskLevel = ajanitorialSkill; thisEmployeeLevel = janitorialSkill; break;
                    case 4: employeeTaskLevel = aengineerSkill; thisEmployeeLevel = engineerSkill; break;
                    case 5: employeeTaskLevel = amanagementSkill; thisEmployeeLevel = managementSkill; break;
                }

                //on shift
                if (!aEmployee.animator.GetBool("Fired") && aEmployee.animator.GetBool("OnShift") && aEmployee != employee)
                {
                    //needs training
                    if (aEmployee.trainingRequired[(int)aEmployee.task] > 0)
                    {
                        //not claimed
                        if (aEmployee.manager == null)
                        {
                            //level 0
                            if (employeeTaskLevel == 0)
                            {
                                //lowest training needed
                                if (aEmployee.trainingRequired[(int)aEmployee.task] < lowestTrainingRemaining)
                                {
                                    lowestTrainingRemaining = aEmployee.trainingRequired[(int)aEmployee.task];
                                    targEmployee = aEmployee;
                                    //distance?
                                }
                            }
                        }
                    }
                }
            }
        }
        else { targEmployee = employee.targetEmployee; }

        //find anyone that needs training
        if (targEmployee == null)
        {
            //get all employees
            foreach (Employee2 aEmployee in FindObjectsOfType<Employee2>())
            {
                int employeeTaskLevel = 0;
                aEmployee.OutSkills(out int ainvSkill, out int acustSkill, out int ajanitorialSkill, out int aengineerSkill, out int amanagementSkill);
                switch ((int)aEmployee.task)
                {
                    case 1: employeeTaskLevel = ainvSkill; thisEmployeeLevel = invSkill; break;
                    case 2: employeeTaskLevel = acustSkill; thisEmployeeLevel = custSkill; break;
                    case 3: employeeTaskLevel = ajanitorialSkill; thisEmployeeLevel = janitorialSkill; break;
                    case 4: employeeTaskLevel = aengineerSkill; thisEmployeeLevel = engineerSkill; break;
                    case 5: employeeTaskLevel = amanagementSkill; thisEmployeeLevel = managementSkill; break;
                }

                //on shift
                if (!aEmployee.animator.GetBool("Fired") && aEmployee.animator.GetBool("OnShift") && aEmployee != employee)
                {
                    //needs training
                    if (aEmployee.trainingRequired[(int)aEmployee.task] > 0)
                    {
                        //not claimed
                        if (aEmployee.manager == null)
                        {
                            //can train
                            if (employeeTaskLevel <= thisEmployeeLevel)
                            {
                                //lowest training needed
                                if (aEmployee.trainingRequired[(int)aEmployee.task] < lowestTrainingRemaining)
                                {
                                    lowestTrainingRemaining = aEmployee.trainingRequired[(int)aEmployee.task];
                                    targEmployee = aEmployee;
                                    //distance?
                                }
                            }
                        }
                    }
                }
            }
        }

        //employee.ResetManagerRange();
        employee.ManagerRange();

        if (targEmployee != null)
        {
            employee.targetEmployee = targEmployee;
            targEmployee.manager = employee;
            //get empolyee line position
            employee.targetPosition = targEmployee.StandNextToMe(employee, null);
            //employee.targetPosition = targEmployee.transform.position;
            targetPos = targEmployee.StandNextToMe(employee, null);
            employee.SetPathfinding(currentPath, pathList, targetPos);
            animator.SetTrigger("Success");
        }
        else if (employee.moveTo) { targetPos = employee.targetPosition; employee.SetPathfinding(currentPath, pathList, targetPos); employee.moveTo = false; animator.SetTrigger("Success"); }
        else 
        {
            if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("Noone to train.", 1, 2); }

            if (Controller.Instance.ifDoneManaging == "Do nothing")
            {
                //do nothing
                employee.SwitchObjective(1);
            }
            else if(Controller.Instance.ifDoneManaging == "Go home")
            {
                //go home
                employee.SendHome();
            }
            else
            {
                //switch task
                employee.SwitchTask(null, Controller.Instance.ifDoneManaging, null);
            }

            animator.SetTrigger("Failure"); 
        }
    }
}
