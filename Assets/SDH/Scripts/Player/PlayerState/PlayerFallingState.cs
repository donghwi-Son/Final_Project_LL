using UnityEngine;

public class PlayerFallingState : State<PlayerController>
{
    public PlayerFallingState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player Falling State Entered");
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
