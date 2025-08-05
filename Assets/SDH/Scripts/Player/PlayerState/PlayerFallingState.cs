using UnityEngine;

public class PlayerFallingState : State<PlayerController>
{
    public PlayerFallingState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        float xVel;
        if (!Mathf.Approximately(owner.XInput, 0f))
        {
            xVel = owner.XInput * owner.moveSpeed;
        }
        else
        {
            xVel = owner.rb.linearVelocity.x;
        }
        owner.SetVelocity(xVel, owner.rb.linearVelocityY);
        if (owner.CanDoubleJump && owner.JumpInput && !owner.IsGroundDetected())
        {
            owner.DoubleJump();
        }
        else if (owner.AttackInput && owner.CanAirAttack)
        {
            stateMachine.ChangeState(owner.AirAttState);
        }
        else if (owner.IsGroundDetected())
        {
            owner.CanAirAttack = true;
            stateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
