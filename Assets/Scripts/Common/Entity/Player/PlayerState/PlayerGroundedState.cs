using UnityEngine;

public class PlayerGroundedState : State<PlayerController>
{
    public PlayerGroundedState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        if (owner.DashInput && owner.CanUseDash)
        {
            stateMachine.ChangeState(owner.DashState);
        }
        if (owner.SkillInput)
        {
            stateMachine.ChangeState(owner.SkillState);
        }
        if (owner.SpecialAttackInput && owner.CanUseSpecialAttack)
        {
            stateMachine.ChangeState(owner.SpecialAtkState);
        }
        if (owner.DefendInput)
        {
            stateMachine.ChangeState(owner.DefendState);
        }
        if (owner.AttackInput)
        {
            stateMachine.ChangeState(owner.AttackState);
        }

        if (!owner.IsGroundDetected())
        {
            stateMachine.ChangeState(owner.FallState);
        }
        else if (owner.JumpInput)
        {
            stateMachine.ChangeState(owner.JumpState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
