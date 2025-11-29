using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class E_FindRegister : StateMachineBehaviour
{
    private List<Building> possibleRegistors = new List<Building>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        employee.OutAI(out UnityEngine.Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (targRegistor != null)
        {
            if (targRegistor.employeeQueue.Count != 0 || !targRegistor.built || targRegistor.automatic) { targRegistor.RemoveFromQueue(employee, null); targRegistor = null; }//need remove from queue?
            if (targRegistor != null) { if (targRegistor.electricityCost > 0 && !targRegistor.turnedOn) { targRegistor.RemoveFromQueue(employee, null); targRegistor = null; } }
        }
        else { targRegistor = null; }

        //attempt registors with customers first
        if (targRegistor == null)
        {
            possibleRegistors.Clear();

            if (Controller.Instance.registers.Count > 0)
            {
                foreach (Building register in Controller.Instance.registers)
                {
                    if (register.employeeQueue.Count == 0 && register.built && register.customerQueue.Count > 0 && !register.automatic)
                    {
                        if (register.electricityCost > 0 && !register.turnedOn) { continue; }
                        possibleRegistors.Add(register);
                    }
                }
            }

            if (possibleRegistors.Count > 0) { targRegistor = possibleRegistors[Random.Range(0, possibleRegistors.Count)]; }
        }

        //attempt registors with items secondly
        if (targRegistor == null)
        {
            possibleRegistors.Clear();

            if (Controller.Instance.registers.Count > 0)
            {
                foreach (Building register in Controller.Instance.registers)
                {
                    if (register.employeeQueue.Count == 0 && register.built && register.transform.GetChild(1).childCount > 0 && !register.automatic)
                    {
                        if (register.electricityCost > 0 && !register.turnedOn) { continue; }
                        possibleRegistors.Add(register);
                    }
                }
            }

            if (possibleRegistors.Count > 0) { targRegistor = possibleRegistors[Random.Range(0, possibleRegistors.Count)]; }
        }

        //find any
        if (targRegistor == null)
        {
            possibleRegistors.Clear();

            if (Controller.Instance.registers.Count > 0)
            {
                foreach (Building register in Controller.Instance.registers)
                {
                    if (register.employeeQueue.Count == 0 && register.built && !register.automatic)
                    {
                        if (register.electricityCost > 0 && !register.turnedOn) { continue; }
                        possibleRegistors.Add(register);
                    }
                }
            }

            if (possibleRegistors.Count > 0) { targRegistor = possibleRegistors[Random.Range(0, possibleRegistors.Count)]; }
        }

        if (targRegistor != null)
        {
            targBuilding = targRegistor;
            targBuilding.AddToQueue(employee, null);
            targetPos = targBuilding.employeeLocation.position;
            targ = targBuilding.transform;

            employee.SetPathfinding(currentPath, pathList, targetPos);
            employee.SetAI(targ, newTsk);
            employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
        }
        else
        {
            if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("No open registers!",1 , 2); }

            if (Controller.Instance.ifDoneCashiering == "Do nothing")
            {
                //do nothing
                employee.SwitchObjective(1);
            }
            else if(Controller.Instance.ifDoneCashiering == "Go home")
            {
                //go home
                employee.SendHome();
            }
            else
            {
                //switch task
                employee.SwitchTask(null, Controller.Instance.ifDoneCashiering, null);
            }

            animator.SetTrigger("Failure");
        }
    }
}
