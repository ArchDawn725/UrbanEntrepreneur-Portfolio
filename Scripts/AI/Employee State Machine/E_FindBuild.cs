using UnityEngine;
public class E_FindBuild : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
        Building targetBuilding = null;
        int progress = 2147483647;


        //check to see if player targeted a building
        if (targBuilding != null)
        {
            if (targBuilding.built || targBuilding.engineerClaim != employee) { targBuilding = null; }
        }

        if (targBuilding == null)
        {
            //get all buildings
            foreach (Building building in FindObjectsOfType<Building>())
            {
                //if building is not built and not claimed
                if (!building.built && (building.engineerClaim == null || building.engineerClaim == employee))
                {
                    //closest to being built
                    if (building.buildProgress - building.buildTicksRequired < progress)
                    {
                        progress = (int)(building.buildProgress - building.buildTicksRequired);
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
            employee.targetPosition = targBuilding.employeeLocation.position;
            targBuilding.engineerClaim = employee;
            animator.SetTrigger("Success");
        }
        else { animator.SetTrigger("Failure"); }
    }
}
