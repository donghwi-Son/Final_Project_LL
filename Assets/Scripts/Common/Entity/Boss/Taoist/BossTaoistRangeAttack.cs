using UnityEngine;

public class BossTaoistRangeAttack : State<BossTaoist>
{
    private int stateType;
    private int rangeQty;
    private float attackDelay;
    private float attackEndDelay;

    public BossTaoistRangeAttack(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.BossFlip();
        stateType = 1;
        rangeQty = owner.rangeAttackQty;
        attackDelay = owner.rangeAttackDelay;
        attackEndDelay = 0.5f;
    }

    public override void Execute()
    {
        base.Execute();

        if(stateType == 1)
        {
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 0)
            {
                // 발싸
                owner.BossRangeAttack();

                rangeQty -= 1;
                attackDelay = owner.rangeAttackDelay;

                if (rangeQty <= 0)
                {
                    stateType = 2;
                }
            }
        }
        else if(stateType == 2)
        {
            attackEndDelay -= Time.deltaTime;
            if(attackEndDelay <= 0)
            {
                stateMachine.ChangeState(owner.moveState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
