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
            stateMachine.ChangeState(owner.DeadState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
