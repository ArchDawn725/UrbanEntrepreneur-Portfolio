using UnityEngine;
public class E_PlaceItem : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (employee != null)
        {
            employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            if (cont.childCount > 0)
            {
                if (targBuilding != null) 
                { 
                    if (targBuilding.selectedItemTypeID == -1 || targBuilding.selectedItemTypeID == cont.GetChild(0).GetComponent<Item>().itemTypeID)
                    {
                        if (targBuilding.capacity > targBuilding.transform.GetChild(1).childCount) { targBuilding.AddItem(cont.GetChild(0).GetComponent<Item>()); }
                    }
                    else { animator.SetTrigger("Failure"); return; }
                }
            }
        }

        if (customer != null)
        {
            customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

            if (targBuilding != null)
            {
                if (cont.childCount > 0)
                {
                    customer.storePreferance[0] += customer.storeImpact;
                    customer.ItemPreferences[cont.GetChild(0).GetComponent<Item>().myName][0] -= 10;
                    customer.ItemQuality(cont.GetChild(0).GetComponent<Item>());
                    targBuilding.AddItem(cont.GetChild(0).GetComponent<Item>());
                    customer.ClaimTile();
                }
            }

        }

        animator.SetTrigger("Success");
    }
}
