using UnityEngine;
public class C_Main : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);

        box.enabled = true;
        vis.gameObject.SetActive(true);
        cont.gameObject.SetActive(true);

        if (animator.GetBool("Searching")) 
        { 
            if (customer.shoppingList.Count == 0) { animator.SetBool("Searching", false); animator.SetBool("Waiting", true); animator.SetTrigger("Success"); return; }
            else { animator.SetTrigger("Success"); return; }
        }

        if (animator.GetBool("Waiting") && cont.childCount > 0) { animator.SetTrigger("Success"); return; }
        else { animator.SetBool("Waiting", false); animator.SetTrigger("Success"); }
    }
}
