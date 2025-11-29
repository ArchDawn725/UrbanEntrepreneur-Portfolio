using UnityEngine;
public class O_Actiavte : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (animator.GetComponent<Officer>().type)
        {
            case 0:
                //arrest employee
                GameObject.Find("MainAudio").transform.GetChild(11).GetComponent<AudioSource>().Stop();
                if (animator.GetComponent<Officer>().targetEmployee != null) { animator.GetComponent<Officer>().targetEmployee.animator.SetInteger("MentalBreak", 0); }
                break;
            case 1:
                //delete customer
                if (animator.GetComponent<Officer>().targetCustomer != null) { animator.GetComponent<Officer>().targetCustomer.DestroyMe(); }
                break;
            case 2:
                //extiquish fire
                if (animator.GetComponent<Officer>().targetBuilding != null) { animator.GetComponent<Officer>().targetBuilding.ExtinguishFire(); }
                break;
        }

        animator.GetComponent<Officer>().targetEmployee = null;
        animator.GetComponent<Officer>().targetCustomer = null;
        animator.GetComponent<Officer>().targetBuilding = null;
        animator.GetComponent<Officer>().mentalBreakTarget = null;
        animator.GetComponent<Officer>().targetPosition = new Vector3(-1,-1,-1);
        animator.SetTrigger("Success");
    }
}
