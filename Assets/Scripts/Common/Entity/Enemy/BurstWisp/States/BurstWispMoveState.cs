using UnityEngine;

public class BurstWispMoveState : State<BurstWisp>
{
    public BurstWispMoveState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetVelocity(owner.moveSpeed * owner.FacingDir, owner.rb.linearVelocityY);

        if (owner.IsPlayerDetected())
        {
            stateMachine.ChangeState(owner.ChaseState);
            return;
        }
        if (!owner.IsGroundDetected() || owner.IsWallDetected())
        {
            owner.Flip();

            stateMachine.ChangeState(owner.IdleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
