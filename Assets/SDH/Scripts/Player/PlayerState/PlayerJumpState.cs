using UnityEngine;

public class PlayerJumpState : State<PlayerController>
{
    public PlayerJumpState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.rb.gravityScale = 1.0f;
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
        else if (owner.rb.linearVelocityY < 0 || Input.GetKeyUp(KeyCode.Space)) // 스페이스바를 떼면 낙하 상태로 전환
        {
            stateMachine.ChangeState(owner.FallingState);
        }
    }
 

    public override void Exit()
    {
        base.Exit();

        owner.rb.gravityScale = 2.5f;
    }
}
