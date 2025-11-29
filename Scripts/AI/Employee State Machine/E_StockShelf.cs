using UnityEngine;
public class E_StockShelf : StateMachineBehaviour
{/*
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();

        if (employee.container.childCount > 0)
        {
            if (employee.targetShelf != null)
            {
                if (employee.targetShelf.selectedItemTypeID == employee.targetItemID)
                {
                    foreach (Item item in employee.container.GetComponentsInChildren<Item>())
                    {
                        if (employee.targetShelf.capacity > employee.targetShelf.transform.GetChild(1).childCount)//check if item is requested item
                        {
                            //StartWorking();
                            animator.SetTrigger("Success");
                            return;
                        }
                        else if (employee.targetShelf.capacity <= employee.targetShelf.transform.GetChild(1).childCount) { break; }
                    }
                }
                else { employee.targetShelf = null; }
            }
        }

        //employee.targetShelf.employee = null;
        employee.target = null; employee.targetItemID = 0;
        employee.targetShelf = null; employee.targetStockPile = null;
        employee.targetBuilding = null;
        //stocking = Stocking.waiting;
        //objective = Objective.idle;
        //OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);

        if (employee.container.childCount > 0) { animator.SetTrigger("Failure"); }
        else
        {
            //FindJob();
            animator.SetTrigger("Other");
        }
    }*/
}
