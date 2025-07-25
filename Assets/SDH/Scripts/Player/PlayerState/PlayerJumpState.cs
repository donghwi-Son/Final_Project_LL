using UnityEngine;

public class PlayerJumpState : State<PlayerController>
{
    public PlayerJumpState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.rb.linearVelocity = new Vector2(owner.rb.linearVelocityX, owner.jumpForce);
    }

    public override void Execute()
    {
        base.Execute();
        owner.SetVelocity(owner.XInput * owner.moveSpeed, owner.rb.linearVelocityY);
        if (owner.CanDoubleJump && owner.JumpInput && !owner.IsGroundDetected())
        {
            owner.DoubleJump();
        }
        else if (owner.AttackInput && owner.CanAirAttack)
        {
            stateMachine.ChangeState(owner.AirAttState);
        }
        else if (owner.rb.linearVelocityY < 0)
        {
            stateMachine.ChangeState(owner.FallingState);
        }
    }
 

    public override void Exit()
    {
        base.Exit();
        owner.anim.SetBool("isJumping", false);
    }
}
