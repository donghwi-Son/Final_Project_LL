using UnityEngine;

public class BossCrowRangeAttack : BossCrowState
{
    public BossCrowRangeAttack(BossCrow enemy, EnemyStateMachine<BossCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetZeroVelocity();
        enemy.BossFlip(false);

        stateType = 0;
    }

    public override void Update()
    {
        base.Update();

        if(stateType == 0 && triggerCalled)
        {
            triggerCalled = false;
            stateType = 1;
            enemy.BossRangeAttack();
        }
        else if(stateType == 1 && triggerCalled)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
