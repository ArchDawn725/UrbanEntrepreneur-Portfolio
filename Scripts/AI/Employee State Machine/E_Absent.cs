using System.Collections.Generic;
using UnityEngine;
public class E_Absent : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null) { employee.RemoveClaim(); employee.atWork = false; }
        if (customer != null) 
        { 
            customer.RemoveClaim();
            customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);

            BoxCollider2D[] colliders = animator.GetComponents<BoxCollider2D>();
            colliders[0].enabled = false;
            colliders[1].enabled = false;
            vis.gameObject.SetActive(false);

            if (customer.special && customer.dueForDeletion) { customer.DestroyMe(); } 
        }

        employee?.Absent();
        customer?.Absent();
    }
}
