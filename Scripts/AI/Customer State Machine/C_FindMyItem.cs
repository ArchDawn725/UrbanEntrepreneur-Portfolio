using UnityEngine;
public class C_FindMyItem : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        /*
        if (targItemID == 0)
        {
            targItemID = Random.Range(1, 6);
        }
        */


        if (customer.shoppingList.Count > 0) 
        { 
            targItemID = customer.shoppingList[0].itemID;
            customer.SetTargets(selectItem, targItemID, targBuilding, targRegistor, targShelf, targStockPile, newTargBuilding);
            animator.SetTrigger("Success");
        }
        else { animator.SetBool("AtSite", false); animator.SetTrigger("Failure"); }
    }
}
