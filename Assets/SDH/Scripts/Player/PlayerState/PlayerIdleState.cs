using UnityEngine;

public class PlayerIdleState : State<PlayerController>
{
    public PlayerIdleState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player Idle State Entered");
        owner.SetZeroVelocity();
    }

    public override void Execute()
    {
        base.Execute();
        if(owner.JumpInput && owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.JumpState);
        }
        if (owner.AttackInput && owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.AttackState);
        }
        if (owner.XInput != 0 && owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.MoveState);
        }
        if (owner.SpecialAttackInput && owner.IsGroundDetected() && owner.CanUseSpecialAttack)
        {
            stateMachine.ChangeState(owner.SpecialAttackState);
        }
        if (owner.SkillInput)
        {
            stateMachine.ChangeState(owner.SkillState);
        }
        if (owner.DashInput && owner.IsGroundDetected() && owner.CanUseDash)
        {
            stateMachine.ChangeState(owner.DashState);
        }
        if (owner.DefendInput)
        {
            stateMachine.ChangeState(owner.DefendState);
        }
        if (!owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.FallingState);
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
