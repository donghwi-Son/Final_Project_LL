using UnityEngine;

public class WoodSpiritMoveState : State<WoodSpirit>
{
    private Transform player;

    public WoodSpiritMoveState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.Instance.player.transform;
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetVelocity(owner.moveSpeed * owner.FacingDir, owner.rb.linearVelocityY);

        if(owner.IsPlayerDetected())
        {
            float dist = Vector2.Distance(owner.transform.position, player.position);
            if (dist <= owner.AttackDistance)
            {
                //공격 상태
                if (owner.CanAttack())
                {
                    stateMachine.ChangeState(owner.AttackState);
                    return;
                }
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
