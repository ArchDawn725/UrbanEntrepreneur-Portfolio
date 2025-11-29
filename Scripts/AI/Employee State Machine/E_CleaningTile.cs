using UnityEngine;
public class E_CleaningTile : StateMachineBehaviour
{
    //clean tile by 10 * level
    //if tile is still dirty > 10 - failure
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);

        float skill = janitorialSkill;
        if (skill == 0) { skill = 0.5f; }
        employee.GetMyGridPosXY(out int x, out int y);
        MapController.NewGrid targetGrid = MapController.Instance.grid.GetGridObject(x, y);
        MapController.Instance.CleanArea(employee.transform.position, -5 * skill);
        //targetGrid.IncreaseCleanValue(-5 * janitorialSkill);
        if (targetGrid.GetCleanNormalized() > 0.09f) { animator.SetTrigger("Failure"); }
        else { targetGrid.employee = null; employee.targetTile = new Vector2Int(-1, -1); animator.SetTrigger("Success"); }

    }
}
