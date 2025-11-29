using System.Collections.Generic;
using UnityEngine;
public class C_WalkAroundStore : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);

        int number = Random.Range(0, Controller.Instance.startingPoints.Count);
        customer.transform.position = Controller.Instance.startingPoints[number].position;
        BoxCollider2D[] colliders = animator.GetComponents<BoxCollider2D>();
        colliders[0].enabled = true;
        colliders[1].enabled = true;
        vis.gameObject.SetActive(true);

        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        int newNumber = -1;
        while(newNumber == -1)
        {
            int newnewNumber = Random.Range(0, Controller.Instance.startingPoints.Count);
            if (newnewNumber != number) { newNumber = newnewNumber; }
        }
        targetPos = Controller.Instance.startingPoints[newNumber].position;
        customer.SetPathfinding(currentPath, pathList, targetPos);
        customer.personVis.UpdateEmotion(0);

        animator.SetTrigger("Success");
    }
}
