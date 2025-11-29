using ArchDawn.Utilities;
using UnityEngine;
public class E_LookingForTile : StateMachineBehaviour
{
    //foreach tile, find dirtiest
    //claim tile
    //save tile location
    //move to tile location
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        float dirtyness = 0.09f;
        employee.targetBuilding = null;
        //Vector2Int targetTile = new Vector2Int(-1, -1);

        if (employee.targetTile != new Vector2(-1, -1))
        {
            MapController.NewGrid targetGrid = MapController.Instance.grid.GetGridObject(employee.targetTile.x, employee.targetTile.y);
            if (targetGrid.GetCleanNormalized() < 0.1f || targetGrid.taken) { employee.targetTile = new Vector2Int(-1, -1); }
        }

        if (employee.targetTile == new Vector2(-1, -1))
        {
            int playableGridStart = TransitionController.Instance.playablegridstart;
            int playableGridSize = TransitionController.Instance.playablegridsize;

            for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
            {
                for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
                {
                    MapController.NewGrid targetGrid = MapController.Instance.grid.GetGridObject(x, y);
                    if (targetGrid.GetCleanNormalized() > dirtyness && targetGrid.isWalkable && !targetGrid.taken)
                    { if (targetGrid.employee == null) { dirtyness = targetGrid.GetCleanNormalized(); employee.targetTile = new Vector2Int(x, y); } }
                }
            }
        }
        
        if (employee.targetTile != new Vector2(-1,-1))
        {
            Debug.Log(MapController.Instance.grid.GetGridObject(employee.targetTile.x, employee.targetTile.y).employee);
            MapController.Instance.grid.GetGridObject(employee.targetTile.x, employee.targetTile.y).employee = employee;
            employee.targetPosition = new Vector3(employee.targetTile.x * 10 + 5, employee.targetTile.y * 10 + 5, 0);
            animator.SetTrigger("Success");
        }
        else 
        {
            if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("Everything is clean!", 1, 2); }

            if (Controller.Instance.ifDoneCleaning == "Do nothing")
            {
                //do nothing
                employee.SwitchObjective(1);
            }
            else if (Controller.Instance.ifDoneCleaning == "Go home")
            {
                //go home
                employee.SendHome();
            }
            else
            {
                //switch task
                employee.SwitchTask(null, Controller.Instance.ifDoneCleaning, null);
            }

            animator.SetTrigger("Failure");
        }//UtilsClass.CreateWorldTextPopup("Everything is clean!", employee.transform.position);
    }
}
