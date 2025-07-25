using UnityEngine;

public class BossCrowIdle : BossCrowState
{
    private float delayTimer;

    public BossCrowIdle(BossCrow enemy, EnemyStateMachine<BossCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        delayTimer = 0;
    }

    public override void Update()
    {
        base.Update();
        enemy.ChasePlayer();

        delayTimer += Time.deltaTime;

        if (delayTimer >= enemy.attackDealy)
        {
            enemy.nextAttackSelect();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
