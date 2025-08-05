using UnityEngine;

public class WispIdleState : State<Wisp>
{
    public WispIdleState(Wisp owner, StateMachine<Wisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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
