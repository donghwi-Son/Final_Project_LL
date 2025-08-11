using UnityEngine;

public class WoodSpiritMoveState : WoodSpiritGroundedState
{
    public WoodSpiritMoveState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetVelocity(owner.MoveSpeed * owner.FacingDir, owner.rb.linearVelocityY);

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
