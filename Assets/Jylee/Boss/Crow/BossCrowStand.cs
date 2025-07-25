using UnityEngine;

public class BossCrowStand : BossCrowState
{
    public BossCrowStand(BossCrow enemy, EnemyStateMachine<BossCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        enemy.DetectPlayer();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
