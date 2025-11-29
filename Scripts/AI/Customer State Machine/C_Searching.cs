using System.Collections.Generic;
using UnityEngine;
using static MapController;

public class C_Searching : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Customer2 customer = animator.GetComponent<Customer2>();
        customer.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);

        customer.RemoveAllClaims();

        List<Vector3> list = new List<Vector3>();
        int playableGridStart = TransitionController.Instance.playablegridstart;
        int playableGridSize = TransitionController.Instance.playablegridsize;

        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                if (MapController.Instance.grid.GetGridObject(x, y).CanWalk() && MapController.Instance.custoemrAllowedZones[MapController.Instance.grid.GetGridObject(x, y).zone])
                {
                    list.Add(MapController.Instance.grid.GetWorldPosition(x, y));
                    //adding to list too consuming?
                }
            }
        }

        /*
        //get random square
        for (int x = 0; x < MapController.Instance.grid.GetWidth(); x++)
        {
            for (int y = 0; y < MapController.Instance.grid.GetHeight(); y++)
            {
                if (MapController.Instance.grid.GetGridObject(x, y).CanWalk() && MapController.Instance.custoemrAllowedZones[MapController.Instance.grid.GetGridObject(x, y).zone])
                {
                    list.Add(MapController.Instance.grid.GetWorldPosition(x, y));
                }
            }
        }
        */

        int number = Random.Range(0, list.Count);
        targetPos = list[number];

        customer.patroling = true;
        customer.SetPathfinding(currentPath, pathList, targetPos);
        animator.SetTrigger("Success");
    }
}
