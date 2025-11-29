using UnityEngine;
public class E_FindRepair : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
        Building targetBuilding = null;
        float lowestLifeRemaining = 101;

        //check to see if player targeted a building
        if (targBuilding != null)
        {
            if (targBuilding.life >= 100) { targBuilding.engineerClaim = null; targBuilding = null; }
        }

        if (targBuilding == null)
        {
            //get all buildings
            foreach (Building building in FindObjectsOfType<Building>())
            {
                //if building is not built and not claimed
                if (building.life < 100 && (building.engineerClaim == null || building.engineerClaim == employee))
                {
                    //lowest life value
                    if (building.life < lowestLifeRemaining)
                    {
                        lowestLifeRemaining = building.life;
                        targetBuilding = building;
                        //distance
                    }
                }
            }
        }
        else { targetBuilding = targBuilding; }


        if (targetBuilding != null)
        {
            targBuilding = targetBuilding;
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            //employee.targetPosition = targBuilding.transform.GetChild(0).transform.GetChild(2).transform.position;
            employee.targetPosition = targBuilding.employeeLocation.position;
            targBuilding.engineerClaim = employee;
            animator.SetTrigger("Success");
        }
        else 
        {
            if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("Nothing to do!", 1, 2); }

            if (Controller.Instance.ifDoneBuilding == "Do nothing")
            {
                //do nothing
                employee.SwitchObjective(1);
            }
            else if(Controller.Instance.ifDoneBuilding == "Go home")
            {
                //go home
                employee.SendHome();
            }
            else
            {
                //switch task
                employee.SwitchTask(null, Controller.Instance.ifDoneBuilding, null);
            }

            animator.SetTrigger("Failure"); 
        }
    }
}
