using UnityEngine;
public class E_Working : StateMachineBehaviour
{
    private Employee2 employee;
    private Animator _animator;

    private GameObject progressBar;
    private BarController progressBarCon;

    private int currentTick;
    private int ticksToFinish;
    private bool managerWatching;

    [SerializeField] private int job;
    [SerializeField] private bool returning;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        employee = animator.GetComponent<Employee2>();
        _animator = animator;
        employee.OutProgressBar(out GameObject bar, out BarController barCon);
        employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
        employee.GetTargets(out Item selectItem, out int targItemID, out Building targBuilding, out Building targRegistor, out Building targShelf, out Building targStockPile, out Building newTargBuilding);

        if (!Qualified()) { animator.SetTrigger("SwichTask"); animator.SetTrigger("Failure"); return; }

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
                case 1: if (employee.targetBuilding != null) { employee.targetBuilding.Used(5); } if (selectItem != null) { selectItem.DeleteMe(); } break;
                case 2: if (employee.targetRegistor != null) { employee.targetRegistor.Used(5); } if (selectItem != null) { selectItem.DeleteMe(); } break;
                case 3: MapController.Instance.CleanArea(employee.transform.position, 5); break;
                case 4: if (employee.targetBuilding != null) { employee.targetBuilding.Used(10); } break;
                case 5: if (employee.targetEmployee != null) { employee.targetEmployee.trainingRequired[(int)employee.targetEmployee.task] += 5; } break;
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
        if (animator.GetInteger("TaskEnum") != 3) { employee.ClaimTile(); }
        TickSystem.Instance.OnHalfTick += Working;
        employee.stuckCalls = 0;

        employee.OutTransforms(out BoxCollider2D box, out Transform vis, out Transform cont);
        if (targBuilding != null)
        {
            employee.transform.position = targBuilding.employeeLocation.position;

            Vector3 direction = targBuilding.transform.GetChild(2).transform.position - employee.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            vis.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            employee.transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().ChangeDirection(angle);
        }
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
            if (job == 0) { Debug.LogError("Job was 0"); }
            progressBar.GetComponent<FadeController>().Hide();
            //employee.XPIncrease(_animator.GetInteger("TaskEnum"));
            employee.XPIncrease(job);
            //if (_animator.GetInteger("TaskEnum") != 3) { employee.RemoveClaim(); }
            if (job != 3) { employee.RemoveClaim(); }
            TickSystem.Instance.OnHalfTick -= Working;
            _animator.SetTrigger("Success");
        }
    }

    private bool Qualified()
    {
        if (!returning)
        {
            employee.OutSkills(out int invSkill, out int custSkill, out int janitorialSkill, out int engineerSkill, out int managementSkill);
            int checkedLevel = 0;
            switch (_animator.GetInteger("TaskEnum"))
            {
                case 1: checkedLevel = invSkill; break;
                case 2: checkedLevel = custSkill; break;
                case 3: checkedLevel = janitorialSkill; break;
                case 4: checkedLevel = engineerSkill; break;
                case 5: checkedLevel = managementSkill; break;
            }

            if ((TransitionController.Instance.difficulty == 1 && TransitionController.Instance.tutorialLevel >= 5) || TransitionController.Instance.tutorialLevel == 5)
            {
                if (checkedLevel == 0 && employee.trainingRequired[(int)employee.task] > 0)
                {
                    if (employee.manager != null)
                    {
                        return true;
                    }
                    else
                    {
                        if (!employee.messageCalled) { employee.messageCalled = true; employee.TalkBubble("I don't know what to do! I need to be trained!", 1, 3); }
                        employee.SwitchObjective(1);
                        Controller.Instance.PriorityTaskCall("manager");
                        return false;
                    }
                }
                else { return true; }
            }
            else { return true; }
        }
        else { return true; }
    }
}
