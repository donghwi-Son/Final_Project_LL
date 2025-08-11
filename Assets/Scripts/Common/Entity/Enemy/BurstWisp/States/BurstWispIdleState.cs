using UnityEngine;

public class BurstWispIdleState : BurstWispGroundedState
{
    public BurstWispIdleState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
        stateTimer = owner.IdleTime;
    }

    public override void Execute()
    {
        base.Execute();

        if (stateTimer <= 0f)
        {
            stateMachine.ChangeState(owner.MoveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
