using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
public class C_FindRegister : StateMachineBehaviour
{
    private List<Building> knownRegistors = new List<Building>();
    private List<Building> possibleRegistors = new List<Building>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        customer.OutAI(out Transform targ, out int newTsk);
        customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        knownRegistors.Clear();
        if (customer.Memory.Count > 0)
        {
            foreach (Building build in customer.Memory)
            {
                if (build.type == BuildingSO.Type.register)
                {
                    knownRegistors.Add(build);
                }
            }
        }

        if (knownRegistors.Count == 0)
        {
            if (!customer.messageCalled) { customer.messageCalled = true; customer.TalkBubble("I cannot find a register!", 1, 2); }//UtilsClass.CreateWorldTextPopup("No open registers!", customer.transform.position); }

            animator.SetTrigger("Failure");
        }


        if (targRegistor == null)
        {
            possibleRegistors.Clear();

            //any registor that is staffed and empty
            if (knownRegistors.Count > 0)
            {
                foreach (Building register in knownRegistors)
                {
                    if (register.customerQueue.Count == 0 && (register.IfStaffed() || (register.automatic && register.turnedOn)) && register.built)
                    {
                        possibleRegistors.Add(register);
                    }
                }
            }

            //any registor that is staffed
            if (possibleRegistors.Count == 0)
            {
                if (knownRegistors.Count > 0)
                {
                    foreach (Building register in knownRegistors)
                    {
                        if ((register.IfStaffed() || (register.automatic && register.turnedOn)) && register.customerQueue.Count < register.maxQueue && register.built)
                        {
                            possibleRegistors.Add(register);
                        }
                    }
                }
            }

            //any registor
            if (possibleRegistors.Count == 0)
            {
                if (knownRegistors.Count > 0)
                {
                    foreach (Building register in knownRegistors)
                    {
                        if (register.customerQueue.Count < register.maxQueue && register.built)
                        {
                            possibleRegistors.Add(register);
                        }
                    }
                    Controller.Instance.PriorityTaskCall("cashier");
                }
            }

            if (possibleRegistors.Count > 0) { targRegistor = possibleRegistors[Random.Range(0, possibleRegistors.Count)]; }
            /*
            else if (Controller.Instance.registers.Count > 0)
            {
                // && Controller.Instance.registers[i].IfStaffed()
                int lowestAmount = Controller.Instance.registers[0].maxQueue + 1;
                for (int i = 0; i < Controller.Instance.registers.Count; i++) { if (Controller.Instance.registers[i].customerQueue.Count < lowestAmount && Controller.Instance.registers[i].customerQueue.Count < Controller.Instance.registers[i].maxQueue && Controller.Instance.registers[i].built && (Controller.Instance.registers[i].IfStaffed() || Controller.Instance.registers[i].automatic && Controller.Instance.registers[i].turnedOn)) { lowestAmount = Controller.Instance.registers[i].customerQueue.Count; targRegistor = Controller.Instance.registers[i]; } }
            }
            */
            else { targRegistor = null; }
        }

        if (targRegistor != null)
        {
            targBuilding = targRegistor;
            targ = targBuilding.transform;
            //targetPos = targBuilding.customerLocation.position;
            targetPos = targBuilding.queueLocations[targBuilding.queueLocations.Count - 1].transform.position;
            customer.beforeLine = true;

            customer.SetPathfinding(currentPath, pathList, targetPos);
            customer.SetAI(targ, newTsk);
            customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
            /*
            targRegistor.AddToQueue(null, customer);

            if (targRegistor.customerQueue.Count > 0)
            {
                if (targRegistor.customerQueue[0] == customer && targRegistor.simultaneous)
                {
                    targBuilding = targRegistor;
                    targ = targBuilding.transform;
                    targetPos = targBuilding.customerLocation.position;

                    customer.SetPathfinding(currentPath, pathList, targetPos);
                    customer.SetAI(targ, newTsk);
                    customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
                else
                {
                    targBuilding = targRegistor;
                    targ = targBuilding.transform;
                    targetPos = targBuilding.GetLinePosition(null, customer);


                    customer.SetPathfinding(currentPath, pathList, targetPos);
                    customer.SetAI(targ, newTsk);
                    customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                    animator.SetTrigger("Success");
                }
            }
            else { Debug.Log("No queue"); animator.SetTrigger("Failure"); }
            */
        }
        else
        {
            if (!customer.messageCalled) { customer.messageCalled = true; customer.TalkBubble("No open registers!",1, 2); }//UtilsClass.CreateWorldTextPopup("No open registers!", customer.transform.position); }
            if (Controller.Instance.registers.Count > 0) { bool foundOne = false; foreach (Building registor in Controller.Instance.registers) { if (registor.IfStaffed() || (registor.automatic && registor.turnedOn)) { foundOne = true; break; } } if (!foundOne) { Controller.Instance.PriorityTaskCall("cashier"); } }
            animator.SetTrigger("Failure");
        }
    }
}
