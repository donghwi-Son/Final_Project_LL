using UnityEngine;

public class BurstWispAttackState : State<BurstWisp>
{
    public BurstWispAttackState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            // 여기에 사망 코드를 추가할 것
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
