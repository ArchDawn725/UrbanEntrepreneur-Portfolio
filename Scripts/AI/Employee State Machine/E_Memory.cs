using System.Linq;
using UnityEngine;
public class E_Memory : StateMachineBehaviour
{
    [SerializeField] private string lookingFor;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool found = false;
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();

        if (customer != null)
        {
            //customer.Memory = customer.Memory.Where(kvp => kvp.Key != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            customer.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);
            switch(lookingFor)
            {
                case "Shelf":
                    foreach (var kvp in customer.Memory)
                    {
                       // if (kvp.Key.)
                    }
                break;
            }
        }
    }
}
