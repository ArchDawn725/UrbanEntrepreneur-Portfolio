using UnityEngine;
public class O_Left : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Officer>().DeleteMe();
    }
}
