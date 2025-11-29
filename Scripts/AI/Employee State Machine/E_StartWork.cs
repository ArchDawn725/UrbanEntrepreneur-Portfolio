using UnityEngine;
public class E_StartWork : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);

        /*
        if (Controller.Instance.employeeEntrances.Count > 0) { employee.transform.position = Controller.Instance.employeeEntrances[Random.Range(0, Controller.Instance.employeeEntrances.Count)].position; }
        else if (Controller.Instance.anyoneEntrances.Count > 0) { employee.transform.position = Controller.Instance.anyoneEntrances[Random.Range(0, Controller.Instance.anyoneEntrances.Count)].position; }
        else if (Controller.Instance.entrances.Count > 0) { employee.transform.position = Controller.Instance.entrances[Random.Range(0, Controller.Instance.entrances.Count)].position; }
        */

        
        //employee.SwitchObjective(1);
        BoxCollider2D[] colliders = animator.GetComponents<BoxCollider2D>();
        colliders[0].enabled = true;
        colliders[1].enabled = true;
        vis.gameObject.SetActive(true);
        cont.gameObject.SetActive(true);
        employee.atWork = true;
        animator.SetInteger("TaskEnum", (int)employee.task);
        if (!employee.insideStore) { employee.transform.position = Controller.Instance.startingPoints[Random.Range(0, Controller.Instance.startingPoints.Count)].position; }
        animator.SetTrigger("Success");
    }
}
