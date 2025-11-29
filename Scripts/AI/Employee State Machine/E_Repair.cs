using UnityEngine;
public class E_Repair : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);

        if (targBuilding != null)
        {
            float skill = engineerSkill;
            if (skill == 0) { skill = 0.2f; }

            if (targBuilding.life < 100) { targBuilding.Repair((int)(skill * 5)); }

            //check if build 
            if (targBuilding.life >= 100) 
            {
                targBuilding.engineerClaim = null;
                targBuilding = null;
                employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                animator.SetTrigger("Success"); 
            }
            else { employee.SwitchObjective(1); animator.SetTrigger("Failure"); }
        }
        else { animator.SetTrigger("Success"); }

    }
}
