using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_ReturnItems : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    /*
     private void ReturnItems()
{
    print("Returning Items");
    if (targetStockPile != null)
    {
        if (transform.GetChild(0).GetChild(3).childCount > 0)
        {
            foreach (Item item in transform.GetChild(0).GetChild(3).GetComponentsInChildren<Item>())
            {
                if (targetStockPile.capacity > targetStockPile.transform.GetChild(1).childCount)//check if item is requested item
                {
                    //targetStockPile.storedItems.Add(item);
                    targetStockPile.AddItem(item);
                    //item.transform.position = targetShelf.gameObject.transform.position;//get random pos and rotation
                }
                else if (targetStockPile.capacity <= targetStockPile.transform.GetChild(1).childCount) { break; }
            }

            //foreach (Item item in targetStockPile.storedItems) { carriedItems.Remove(item); }
        }
    }


    //carriedItems.Clear();//test
    //if still have items
    //if (transform.GetChild(1).childCount > 0) { print("Still have items"); Returning(); return; } //causing crash //still detecting item becuase of no delay

    //objective = Objective.idle;
    target = null;
    targetStockPile = newTargetStockPile; newTargetStockPile = null;
    targetShelf = newTargetShelf; newTargetShelf = null;
    targetItemID = newTargetItemID; newTargetItemID = 0;
    building = null;
    stocking = Stocking.waiting;
    objective = Objective.idle;
    OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);
    FindJob();
}
     */
}
