using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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

        if(owner.XInput == owner.FacingDir && owner.IsWallDetected())
        {
            return;
        }

        if (owner.XInput != 0)
        {
            stateMachine.ChangeState(owner.MoveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
