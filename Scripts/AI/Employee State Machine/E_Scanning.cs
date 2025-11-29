using System.Collections.Generic;
using UnityEngine;
public class E_Scanning : StateMachineBehaviour
{
    private List<Item> possibleItems = new List<Item>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        //employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (employee.scanning != true)
        {
            targRegistor.AddToQueue(employee, null);//targBuild.employee = employee;//?
            targRegistor.GetStaff();
            employee.scanning = true;
        }
        else if (employee.transform.position != targBuilding.transform.GetChild(0).transform.GetChild(2).transform.position)
        {
            targRegistor.RemoveFromQueue(employee, null);//targBuild.employee = employee;//?
            employee.RemoveAllClaims();
            employee.scanning = false;

            animator.SetTrigger("Other"); 
            return; 
        }


        if (targRegistor != null)
        {
            //isWorking = true;
            //targetRegister.ChangeClaim(this, null);
            //registering = Registering.scanning;
            //objective = Objective.working;
            //OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);

            possibleItems.Clear();

            if (targRegistor.transform.GetChild(1).childCount > 0)
            {
                foreach (Item item in targRegistor.transform.GetChild(1).GetComponentsInChildren<Item>())
                {
                    possibleItems.Add(item);
                }
            }

            if (possibleItems.Count > 0) { selectItem = possibleItems[Random.Range(0, possibleItems.Count)]; }


            /*
            if (possibleItems.Count > 0 && selectItem != null)
            {
                for (int i = 0; i < targBuild.transform.GetChild(1).childCount; i++)
                {
                    if (employee.capacity > cont.childCount)
                    {
                        int number = Random.Range(0, possibleItems.Count);
                        if (possibleItems[number].itemTypeID == selectItem.itemTypeID)
                        {
                            employee.SetAI(targ, targBuild, targItemID, selectItem, newTargBuilding);
                            animator.SetTrigger("Success"); return;
                        }
                    }
                    else { break; }
                }
            }
            else { animator.SetTrigger("Failure"); }
            */
            if (possibleItems.Count > 0 && selectItem != null)
            {
                employee.SetAI(targ, newTsk);
                employee.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
                employee.ClaimTile();
                animator.SetTrigger("Success");
                return;
            }
            else 
            { 
                if (targRegistor.customerQueue.Count > 0)
                {
                    if (targRegistor.customerQueue[0].transform.GetChild(0).GetChild(3).childCount == 0) { targRegistor.customerQueue[0].FinishCheckingOut(); }
                }
                else
                {
                    if (Controller.Instance.ifDoneCashiering == "Do nothing")
                    {
                        //do nothing
                        employee.ClaimTile();
                    }
                    else if (Controller.Instance.ifDoneCashiering == "Go home")
                    {
                        //go home
                        employee.SendHome();
                    }
                    else
                    {
                        Debug.Log("Switching to: " + Controller.Instance.ifDoneCashiering);
                        //switch task
                        employee.SwitchTask(null, Controller.Instance.ifDoneCashiering, null);
                    }
                }



                animator.SetTrigger("Failure"); 
                return; 
            }
        }
        else { employee.RemoveClaim(); animator.SetTrigger("Other"); return; }

        //failsafe
        animator.SetTrigger("Other");
    }
}
