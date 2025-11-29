using ArchDawn.Utilities;
using UnityEngine;
public class E_TrainingChecker : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        int checkedLevel = 0;
        switch (animator.GetInteger("TaskEnum"))
        {
            case 1: checkedLevel = invSkill; break;
            case 2: checkedLevel = custSkill; break;
            case 3: checkedLevel = janitorialSkill; break;
            case 4: checkedLevel = engineerSkill; break;
            case 5: checkedLevel = managementSkill; break;
        }

        if ((TransitionController.Instance.difficulty == 1 && TransitionController.Instance.tutorialLevel >= 5) || TransitionController.Instance.tutorialLevel == 5)
        {
            if (checkedLevel == 0 && employee.trainingRequired[(int)employee.task] > 0)
            {
                if (employee.manager != null)
                {
                    animator.SetTrigger("Success");
                }
                else
                { 
                    if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("I don't know what to do! I need to be trained!", 1, 3); ToolTip.Instance.ActivateTutorial(73); } 
                    employee.SwitchObjective(1);
                    Controller.Instance.PriorityTaskCall("manager");
                    animator.SetTrigger("Failure"); 
                }
            }
            else { animator.SetTrigger("Success"); }
        }
        else { animator.SetTrigger("Success"); }
    }
}
