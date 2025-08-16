using Unity.VisualScripting;
using UnityEngine;

public class BossTaoistSpecialAttack : State<BossTaoist>
{
    private int stateType;
    private int singleCount;
    private int doubleCount;
    private bool attackType;
    private float singleTimer;
    private float doubleTimer;
    private float finalTimer;
    public BossTaoistSpecialAttack(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateType = 1;
        attackType = true;
        singleCount = owner.singleAttackCount;
        doubleCount = owner.doubleAttackCount;
        singleTimer = owner.singleAttackDelay;
        doubleTimer = owner.doubleAttackDelay;
        finalTimer = owner.finalDelay;
    }

    public override void Execute()
    {
        base.Execute();
        if(stateType == 1)
        {
            if (triggerCalled)
            {
                stateType = 2;
                owner.anim.SetTrigger("IsSpecial");
            }
        }
        else if(stateType == 2)
        {
            singleTimer -= Time.deltaTime;
            if(singleTimer <= 0)
            {
                singleTimer = owner.singleAttackDelay;
                owner.BossSpecialAttack(attackType ? 0 : 90);

                singleCount -= 1;
                attackType = !attackType;

                if (singleCount <= 0)
                {
                    stateType = 3;
                }
            }
        }
        else if(stateType == 3)
        {
            doubleTimer -= Time.deltaTime;
            if (doubleTimer <= 0)
            {
                doubleTimer = owner.doubleAttackDelay;
                owner.BossSpecialAttack(attackType ? 0 : 45);
                owner.BossSpecialAttack(attackType ? 90 : -45);

                doubleCount -= 1;
                attackType = !attackType;

                if (doubleCount <= 0)
                {
                    stateType = 4;
                }
            }
        }
        else if(stateType == 4)
        {
            finalTimer -= Time.deltaTime;
            if(finalTimer <= 0)
            {
                stateType = 5;
                owner.anim.SetTrigger("IsSpecialEnd");
            }
        }
        else if(stateType == 5)
        {
            if (triggerCalled)
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
