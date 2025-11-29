using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Leave : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Officer>().done = true;
        animator.GetComponent<Officer>().targetPosition = Controller.Instance.startingPoints[Random.Range(0, Controller.Instance.startingPoints.Count)].transform.position;
        animator.SetTrigger("Success");
    }
}
