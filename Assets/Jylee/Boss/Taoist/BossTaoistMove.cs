using UnityEngine;

public class BossTaoistMove : State<BossTaoist>
{
    public BossTaoistMove(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        owner.attackCooldownTimer -= Time.deltaTime;

        if (owner.attackCooldownTimer <= 0 && owner.IsPlayerInAttackRange())
        {
            owner.attackCooldownTimer = owner.AttackCooldown;
            owner.NextAttackSelect();
        }
        else if (triggerCalled)
        {
            triggerCalled = false;
            owner.ChasePlayer();
        }
    }

    public override void Exit()
    {
        base.Exit();
        triggerCalled = false;
    }
}
