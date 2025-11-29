using UnityEngine;
public class E_Waiting : StateMachineBehaviour
{
    [SerializeField] private int waitTime = 5;
    private Animator _animator;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Employee2 employee = animator.GetComponent<Employee2>();
        Customer2 customer = animator.GetComponent<Customer2>();
        if (employee != null) { employee.stuckCalls = 0; }
        //employee?.SwitchObjective(1);
        _animator = animator;
        switch(waitTime)
        {
            default: TickSystem.Instance.On5Tick += Waiting; break;
            case 5: TickSystem.Instance.On5Tick += Waiting; break;
            case 10: TickSystem.Instance.On10Tick += Waiting; break;
            case 25: TickSystem.Instance.On25Tick += Waiting; break;
        }
    }

    private void Waiting(object sender, TickSystem.OnTickEventArgs e)
    {
        if (_animator != null) { _animator.SetTrigger("Success"); }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (waitTime)
        {
            default: TickSystem.Instance.On5Tick -= Waiting; break;
            case 5: TickSystem.Instance.On5Tick -= Waiting; break;
            case 10: TickSystem.Instance.On10Tick -= Waiting; break;
            case 25: TickSystem.Instance.On25Tick -= Waiting; break;
        }
    }
}
