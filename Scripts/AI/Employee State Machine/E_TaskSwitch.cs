using System.Collections.Generic;
using UnityEngine;

public class E_TaskSwitch : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        //employee.SwitchObjective(1);

        if (targBuilding != null) { targBuilding.RemoveFromQueue(employee, null); }
        if (targRegistor != null) { targRegistor.RemoveFromQueue(employee, null); }
        if (targStockPile != null) { targStockPile.RemoveFromQueue(employee, null); }
        targ = null;
        if (employee.targetEmployee != null) { employee.targetEmployee.manager = null; }
        employee.targetEmployee = null;
        if ((animator.GetInteger("TaskEnum") == 4 && newTsk != 4) && targBuilding != null) { targBuilding.engineerClaim = null; }

        employee.beforeLine = false;
        targBuilding = null;
        targRegistor = null;
        targShelf = null;
        targStockPile = null;
        targItemID = -1;
        selectItem = null;
        employee.scanning = false;
        employee.RemoveAllClaims();
        employee.targetTile = new Vector2Int(-1, -1);
        employee.ResetManagerRange();
        if (employee.newTargetTile != new Vector2Int(-1,-1)) { employee.targetTile = employee.newTargetTile; employee.newTargetTile = new Vector2Int(-1, -1); }

        targBuilding = newTargBuilding;
        employee.targetEmployee = employee.newTargetEmployee;
        if (animator.GetInteger("TaskEnum") == 4 && targBuilding != null) { targBuilding.engineerClaim = employee; }
        employee.newTargetEmployee = null;
        if (newTargBuilding != null)
        {
            switch (newTargBuilding.type)
            {
                case BuildingSO.Type.shelf: targShelf = newTargBuilding; break;
                case BuildingSO.Type.stockPile: targStockPile = newTargBuilding; break;
                case BuildingSO.Type.register: targRegistor = newTargBuilding; break;
            }
        }

        if (targBuilding != null) { targItemID = targBuilding.selectedItemTypeID; }

        for (int x = 0; x < MapController.Instance.grid.GetWidth(); x++)
        {
            for (int y = 0; y < MapController.Instance.grid.GetHeight(); y++)
            {
                if (MapController.Instance.grid.GetGridObject(x, y).employee = employee) { MapController.Instance.grid.GetGridObject(x, y).employee = null; }
            }
        }

        //newTargBuilding = null;

        animator.SetBool("SwichTask", false);
        animator.SetInteger("TaskEnum", newTsk);

        //new tsk reset?

        employee.SetAI(targ, newTsk);
        employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);

        animator.SetTrigger("Success");
    }
}
