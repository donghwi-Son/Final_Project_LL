using UnityEngine;

public class PlayerMoveState : State<PlayerController>
{
    public PlayerMoveState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player Move State Entered");
    }

    public override void Execute()
    {
        base.Execute();
        owner.SetVelocity(owner.XInput * owner.moveSpeed, owner.rb.linearVelocityY);
        if (owner.JumpInput && owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.JumpState);
        }
        if (!owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.FallingState);
        }
        if (owner.AttackInput)
        {
            stateMachine.ChangeState(owner.AttackState);
        }
        if(owner.SpecialAttackInput && owner.CanUseSpecialAttack)
        {
            stateMachine.ChangeState(owner.SpecialAttackState);
        }
        if (owner.SkillInput)
        {
            stateMachine.ChangeState(owner.SkillState);
        }
        if (owner.DashInput && owner.CanUseDash)
        {
            stateMachine.ChangeState(owner.DashState);
        }
        if (owner.DefendInput)
        {
            stateMachine.ChangeState(owner.DefendState);
        }
        if (owner.XInput == 0 && owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
