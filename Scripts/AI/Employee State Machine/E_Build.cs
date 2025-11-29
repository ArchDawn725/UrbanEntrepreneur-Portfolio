using UnityEngine;
public class E_Build : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);

        if (targBuilding != null)
        {
            int skill = engineerSkill;
            if (skill == 0) { skill = 1; }
            if (!targBuilding.built) { targBuilding.BuildTick(skill); }

            //check if build 
            if (targBuilding.built)
            {
                targBuilding.engineerClaim = null;
                targBuilding = null;
                employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                animator.SetTrigger("Success");
            }
            else { animator.SetTrigger("Failure"); }
        }
        else { animator.SetTrigger("Success"); }
    }
}
