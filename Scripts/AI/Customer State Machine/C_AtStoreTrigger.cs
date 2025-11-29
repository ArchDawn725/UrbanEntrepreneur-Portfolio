using System;
using UnityEngine;
public class C_AtStoreTrigger : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        //GameObject.Find("MainAudio").transform.GetChild(0).GetComponent<AudioSource>().Play();
        customer.transform.GetChild(2).GetComponent<AudioSource>().Play();
        animator.SetBool("Searching", true);
        animator.SetBool("Waiting", true);
        animator.SetBool("AtSite", true);
        customer.shopping = true;
        ToolTip.Instance.ActivateTutorial(27);
        customer.entrance.ChangeClaim(customer);
        customer.entrance = null;
        customer.insideStore = true;

        Controller.Instance.MoneyValueChange(Controller.Instance.customerEntry, customer.transform.position, true, false);
        customer.money -= Controller.Instance.customerEntry;
        customer.storeOpinion -= (Controller.Instance.customerEntry * 10);

        animator.SetTrigger("Success");
    }
}
