using UnityEngine;

public class PlayerAirborneState : State<PlayerController>
{
    public PlayerAirborneState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        if (owner.CanDoubleJump && owner.JumpInput && !owner.IsGroundDetected())
        {
            owner.DoubleJump();
            return;
        }
        else if (owner.AttackInput && owner.CanAirAttack)
        {
            stateMachine.ChangeState(owner.AirAtkState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
