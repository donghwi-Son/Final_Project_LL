using UnityEngine;

public class PlayerJumpState : PlayerAirborneState
{
    public PlayerJumpState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.rb.gravityScale = 1.0f;
        owner.rb.linearVelocity = new Vector2(owner.rb.linearVelocityX, owner.JumpForce);
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetVelocity(owner.XInput * owner.MoveSpeed, owner.rb.linearVelocityY);

        if (owner.rb.linearVelocityY < 0 || Input.GetKeyUp(KeyCode.Space)) // 스페이스바를 떼면 낙하 상태로 전환
        {
            stateMachine.ChangeState(owner.FallState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        owner.rb.gravityScale = 2.5f;
    }
}
