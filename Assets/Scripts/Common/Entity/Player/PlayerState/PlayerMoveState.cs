using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        owner.SetVelocity(owner.XInput * owner.MoveSpeed, owner.rb.linearVelocityY);

        if (owner.XInput == 0 || owner.IsWallDetected())
        {
            stateMachine.ChangeState(owner.IdleState);
        }

        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
