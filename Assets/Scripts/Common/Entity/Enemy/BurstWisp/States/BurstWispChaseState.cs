using UnityEngine;

public class BurstWispChaseState : State<BurstWisp>
{
    private Transform player;
    private int moveDir;

    public BurstWispChaseState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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

        if (owner.IsPlayerDetected())
        {
            if (owner.IsPlayerDetected().distance < owner.AttackDistance)
            {
                //공격 상태
                if (owner.CanAttack())
                {
                    stateMachine.ChangeState(owner.AttackState);
                    return;
                }
            }
        }
        else
        {
            stateMachine.ChangeState(owner.IdleState);
            return;
        }

        if (player.position.x > owner.transform.position.x)
        {
            moveDir = 1;
        }
        else if (player.position.x < owner.transform.position.x)
        {
            moveDir = -1;
        }

        owner.SetVelocity(owner.moveSpeed * moveDir, owner.rb.linearVelocityY);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
