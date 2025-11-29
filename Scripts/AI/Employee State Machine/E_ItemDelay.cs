using UnityEngine;
public class E_ItemDelay : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null)
        {
            employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            employee.OutAI(out Transform targ, out int newTsk);
            employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            //placed / gathered all items
            if (cont.childCount >= employee.capacity) 
            {
                if (employee.scanning != true)
                { targBuilding.ChangeClaim(employee, null); employee.RemoveAllClaims(); }
                animator.SetTrigger("Success"); return; 
            }
            else if (cont.childCount <= 0) 
            {
                if (employee.scanning != true)
                { targBuilding.ChangeClaim(employee, null); employee.RemoveAllClaims(); }
                animator.SetTrigger("Success"); return;
            }

            //building cannot hold more or is empty
            if (targBuilding.transform.GetChild(1).childCount >= targBuilding.capacity && cont.childCount > 0) { if (employee.scanning != true) { targBuilding.ChangeClaim(employee, null); employee.RemoveAllClaims(); } animator.SetTrigger("Failure"); return; }
            else if (targBuilding.transform.GetChild(1).childCount <= 0 && cont.childCount < employee.capacity)
            { if (employee.scanning != true) { targBuilding.ChangeClaim(employee, null); employee.RemoveAllClaims(); } animator.SetTrigger("Failure"); return; }
            /*
            if (targBuild.transform.GetChild(1).childCount >= targBuild.capacity || targBuild.transform.GetChild(1).childCount <= 0)
            {
                targBuild.ChangeClaim(employee, null);
                animator.SetTrigger("Failure"); return;
            }
            */
        }

        if (customer != null)
        {
            customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            customer.OutAI(out Transform targ, out int newTsk);
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            if (targBuilding == null) { customer.RemoveAllClaims(); animator.SetBool("AtSite", false); animator.SetTrigger("Success"); return; }
            else
            {
                if (targBuilding.type == BuildingSO.Type.shelf)
                {
                    customer.RemoveAllClaims(); targBuilding.ChangeClaim(null, customer); animator.SetTrigger("Success");
                }//shelf
                else
                {
                    animator.SetTrigger("Success");
                    /*
                    if (cont.childCount >= customer.capacity) { customer.RemoveAllClaims(); targBuilding.ChangeClaim(null, customer); animator.SetTrigger("Success"); return; }
                    else if (cont.childCount <= 0) { if (!animator.GetBool("AtSite")) { targBuilding.ChangeClaim(null, customer); } animator.SetTrigger("Success"); return; }

                    if (targBuilding.transform.GetChild(1).childCount >= targBuilding.capacity && cont.childCount > 0) { targBuilding.ChangeClaim(null, customer); animator.SetTrigger("Failure"); return; }
                    else if (targBuilding.transform.GetChild(1).childCount <= 0 && cont.childCount < customer.capacity) { targBuilding.ChangeClaim(null, customer); animator.SetTrigger("Failure"); return; }
                    */
                }
            }
        }


        //can still do more
        animator.SetTrigger("Other");
    }
}
