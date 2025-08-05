using UnityEngine;

public class WoodSpiritMoveState : State<WoodSpirit>
{
    public WoodSpiritMoveState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetVelocity(owner.moveSpeed * owner.FacingDir, owner.rb.linearVelocityY);

        if(owner.IsPlayerDetected())
        {
            if(owner.IsPlayerDetected().distance <= owner.AttackDistance && owner.CanAttack())
            {
                stateMachine.ChangeState(owner.AttackState);
                return;
            }
        }
        if (!owner.IsGroundDetected() || owner.IsWallDetected())
        {
            owner.Flip();

            stateMachine.ChangeState(owner.IdleState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
