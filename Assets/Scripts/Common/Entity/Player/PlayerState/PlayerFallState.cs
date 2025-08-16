using UnityEngine;

public class PlayerFallState : PlayerAirborneState
{
    public PlayerFallState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        float xVel;
        if (!Mathf.Approximately(owner.XInput, 0f))
        {
            xVel = owner.XInput * owner.MoveSpeed;
        }
        else
        {
            xVel = owner.rb.linearVelocity.x;
        }
        owner.SetVelocity(xVel, owner.rb.linearVelocityY);

        if (owner.IsGroundDetected())
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
