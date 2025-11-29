using UnityEngine;
using Random = UnityEngine.Random;

public class C_Enter : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);

        if (!customer.insideStore) { customer.transform.position = Controller.Instance.startingPoints[Random.Range(0, Controller.Instance.startingPoints.Count)].position; customer.beforeLine = true; }
        BoxCollider2D[] colliders = animator.GetComponents<BoxCollider2D>();
        colliders[0].enabled = true;
        colliders[1].enabled = true;
        vis.gameObject.SetActive(true);
        cont.gameObject.SetActive(true);
        customer.activated = true;
        animator.SetBool("WaitingOutside", true);

        animator.SetTrigger("Success");
    }
}
