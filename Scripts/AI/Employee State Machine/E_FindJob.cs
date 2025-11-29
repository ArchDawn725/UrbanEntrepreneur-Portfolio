using UnityEngine;
public class E_FindJob : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        employee.stuckCalls = 0;
        employee.insideStore = true;
        if (cont.childCount > 0) { animator.SetTrigger("Failure"); }
        else { animator.SetTrigger("Success"); }
    }
}
