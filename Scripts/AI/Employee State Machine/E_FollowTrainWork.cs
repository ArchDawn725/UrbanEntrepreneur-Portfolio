using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class E_FollowTrainWork : StateMachineBehaviour
{
    private Employee2 employee;
    private Animator _animator;

    private GameObject progressBar;
    private BarController progressBarCon;

    private int currentTick;
    private int ticksToFinish;
    private bool managerWatching;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        employee = animator.GetComponent<Employee2>();
        _animator = animator;
        employee.OutProgressBar(out GameObject bar, out BarController barCon);
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        int failChance = 0;
        failChance += 100;
        failChance += Random.Range(0, 100);
        failChance -= (int)employee.stress;
        switch (animator.GetInteger("TaskEnum"))
        {
            case 1: failChance += invSkill * 20; break;
            case 2: failChance += custSkill * 20; break;
            case 3: failChance += janitorialSkill * 20; break;
            case 4: failChance += engineerSkill * 20; break;
            case 5: failChance += managementSkill * 20; break;
        }
        if (employee.traits["Clumsy"]) { failChance /= 2; failChance -= 100; }

        if (Random.Range(0, 100) > failChance)
        {
            switch (animator.GetInteger("TaskEnum"))
            {
                case 1: employee.targetBuilding.Used(5); if (selectItem != null) { selectItem.DeleteMe(); } break;
                case 2: employee.targetRegistor.Used(5); if (selectItem != null) { selectItem.DeleteMe(); } break;
                case 3: MapController.Instance.CleanArea(employee.transform.position, 25); break;
                case 4: employee.targetBuilding.Used(25); break;
                case 5: employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] += 10; break;
            }
            employee.TalkBubble("Oops", 1, 2);
            animator.SetTrigger("Failure");
            return;
        }

        employee.SwitchObjective(2);

        progressBar = bar;
        progressBarCon = barCon;

        progressBar.GetComponent<FadeController>().Activate();

        currentTick = 0;
        progressBarCon.Reset();
        progressBar.SetActive(true);


        int additionalTicks = 0;
        if (animator.GetInteger("TaskEnum") == 3)
        {
            employee.GetMyGridPosXY(out int x, out int y);
            MapController.NewGrid targetGrid = MapController.Instance.grid.GetGridObject(x, y);
            additionalTicks = targetGrid.GetCleaningSpeed();
        }

        switch (animator.GetInteger("TaskEnum"))
        {
            case 1: ticksToFinish = employee.GetWorkTicks() - (invSkill) - 5 + employee.targetBuilding.speedReducer; break;
            case 2: ticksToFinish = employee.GetWorkTicks() - (custSkill) - 5 + employee.targetBuilding.speedReducer; break;
            case 3: ticksToFinish = employee.GetWorkTicks() - (janitorialSkill) + 5; break;
            case 4: ticksToFinish = employee.GetWorkTicks() - (engineerSkill) + 15; break;
            case 5: ticksToFinish = employee.GetWorkTicks() - (managementSkill) + 10; break;
                //building time
        }

        ticksToFinish += additionalTicks;

        if (employee.trainingRequired[animator.GetInteger("TaskEnum")] > 0) { ticksToFinish *= 2; }
        employee.ShowTaskImage(animator.GetInteger("TaskEnum"), true);
        progressBar.transform.GetChild(0).GetComponent<SpriteRenderer>().color = employee.taskColors[animator.GetInteger("TaskEnum") - 1];
        animator.SetInteger("Animation", 2);
        //employee.ClaimTile();
        TickSystem.Instance.OnHalfTick += Working;
        employee.stuckCalls = 0;
    }

    private void Working(object sender, TickSystem.OnTickEventArgs e)
    {
        if (!managerWatching)
        {
            int managerLevel = employee.GetGrid().managerLevel;
            if (managerLevel > 0)
            {
                managerWatching = true;
                currentTick += managerLevel;
            }
        }
        currentTick++;

        if (currentTick + 2f <= ticksToFinish)
        {
            progressBarCon.Activate((currentTick + 2) * 1f / ticksToFinish);
        }

        if (currentTick >= ticksToFinish)
        {
            progressBar.GetComponent<FadeController>().Hide();
            employee.XPIncrease(_animator.GetInteger("TaskEnum"));
            //employee.RemoveClaim();
            TickSystem.Instance.OnHalfTick -= Working;
            Train();
        }
    }

    private void Train()
    {
        Employee2 employee = _animator.GetComponent<Employee2>();
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);

        if (employee.targetEmployee != null)
        {
            employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] -= managementSkill;
            employee.targetEmployee.XPIncrease((int)employee.targetEmployee.task);
        }

        if (employee.targetEmployee != null) { if (employee.targetEmployee.animator.GetBool("Fired") || !employee.targetEmployee.animator.GetBool("OnShift")) { employee.targetEmployee.manager = null; employee.targetEmployee = null; } }
        if (employee.targetEmployee != null) { if (employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] <= 0) { employee.targetEmployee.manager = null; employee.targetEmployee = null; } }

        _animator.SetTrigger("Success");
    }


    [SerializeField] bool foundPath;
    private Transform visuals;

    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private Vector3 lastPosition;
    private MapController.NewGrid lastGrid;

    private float minDist = 2f;
    private float maxDist = 10;
    private float previousDistance = 100;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        previousDistance = 100;
        employee.OutPathfinding(out int currentPath, out List<Vector3> pathList, out Vector3 targetPos);
        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        employee.OutAI(out Transform targ, out int newTsk);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (foundPath == false)
        {
            if (pathList != null) { pathList.Clear(); }
            currentPath = 0;
            //employee.RemoveClaim();
            pathList = MapController.Instance.FindPath(employee.GetPosition(), targetPos, false, false);

            if (pathList != null && pathList.Count > 1)
            {
                pathList.RemoveAt(0);
                employee.SetPathfinding(currentPath, pathList, targetPos);
                foundPath = true;
                employee.stuckCalls = 0;
            }
            else
            {
                foundPath = false;
            }

            if (foundPath)
            {
                visuals = vis;
                currentPathIndex = currentPath;
                pathVectorList = pathList;
                lastPosition = employee.transform.position;
                lastGrid = employee.GetGrid();
                employee.ToggleWalkingSounds(true);
                animator.SetInteger("Animation", 1);
            }
        }

        if (foundPath)
        {
            if (pathVectorList != null)
            {
                Vector3 targetPosition = pathVectorList[currentPathIndex];
                //Debug.Log("Found path to: " + employee.transform.position + " x " + targetPosition + " IDX" + currentPathIndex + "Count: " + pathVectorList.Count);

                if (Vector3.Distance(employee.transform.position, lastPosition) > maxDist * 2) { employee.transform.position = targetPosition; }
                if (Vector3.Distance(employee.transform.position, targetPosition) > previousDistance) { employee.transform.position = targetPosition; previousDistance = 200; }
                else { previousDistance = Vector3.Distance(employee.transform.position, targetPosition); }
                if (currentPathIndex == pathVectorList.Count - 1)
                {
                    if (Vector3.Distance(employee.transform.position, targetPosition) > minDist && Vector3.Distance(employee.transform.position, lastPosition) < maxDist)
                    {
                        //Debug.Log("Moving" + " IDX" + currentPathIndex);
                        Vector3 moveDir = (targetPosition - employee.transform.position).normalized;

                        Vector3 direction = targetPosition - employee.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        employee.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);

                        employee.transform.position = employee.transform.position + moveDir * employee.speedCalc * Time.fixedDeltaTime;
                    }
                    else
                    {
                        //Debug.Log("Made it" + " IDX" + currentPathIndex);
                        pathVectorList = null;
                        foundPath = false;
                    }
                }
                else
                {
                    if (Vector3.Distance(employee.transform.position, targetPosition) > minDist && Vector3.Distance(employee.transform.position, lastPosition) < maxDist)
                    {
                        Vector3 moveDir = (targetPosition - employee.transform.position).normalized;

                        Vector3 direction = targetPosition - employee.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        visuals.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        employee.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);

                        employee.transform.position = employee.transform.position + moveDir * employee.speedCalc * Time.fixedDeltaTime;
                    }
                    else
                    {
                        lastPosition = targetPosition;
                        currentPathIndex++;
                        previousDistance = 200;
                    }
                }
                if (employee.GetGrid() != lastGrid) { employee.EnterNewTile(employee.GetGrid()); lastGrid = employee.GetGrid(); }

                if (TickSystem.Instance.timeMultiplier == 0) { animator.transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Stop(); }


                if (!foundPath)
                {
                    pathList = null;
                    employee.SetPathfinding(currentPath, pathList, targetPos);

                    employee.transform.position = targetPos;
                    employee.ToggleWalkingSounds(false);
                    employee.ManagerRange();

                    animator.SetInteger("Animation", 0);
                    animator.SetTrigger("Success");
                }
            }
        }
    }
}
