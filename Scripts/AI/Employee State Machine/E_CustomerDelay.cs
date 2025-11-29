using UnityEngine;
public class E_CustomerDelay : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();

        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (targRegistor != null)
        {
            if (targRegistor.customerQueue.Count > 0)
            {
                if (targRegistor.customerQueue[0].transform.GetChild(0).GetChild(3).childCount == 0 && targRegistor.transform.GetChild(1).childCount == 0) { targRegistor.customerQueue[0].FinishCheckingOut(); animator.SetTrigger("Success"); }
                else { animator.SetTrigger("Failure"); }
            }
            else { animator.SetTrigger("Failure"); }
        }
        else { animator.SetTrigger("Failure"); }
    }
}
