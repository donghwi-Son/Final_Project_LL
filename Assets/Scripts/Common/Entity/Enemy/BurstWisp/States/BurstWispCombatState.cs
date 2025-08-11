using UnityEngine;

public class BurstWispCombatState : State<BurstWisp>
{
    private Transform player;
    private int moveDir;

    public BurstWispCombatState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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
            stateTimer = owner.CombatTime;

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
        else
        {
            if(stateTimer <= 0)
            {
                stateMachine.ChangeState(owner.IdleState);
                return;
            }
        }

        if (player.position.x > owner.transform.position.x)
        {
            moveDir = 1;
        }
        else if (player.position.x < owner.transform.position.x)
        {
            moveDir = -1;
        }

        owner.SetVelocity(owner.MoveSpeed * moveDir, owner.rb.linearVelocityY);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
