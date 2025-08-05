using UnityEngine;

public class BurstWispIdleState : State<BurstWisp>
{
    public BurstWispIdleState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
        stateTimer = owner.idleTime;
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
