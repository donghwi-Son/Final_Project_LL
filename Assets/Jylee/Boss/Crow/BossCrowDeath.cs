using UnityEngine;

public class BossCrowDeath : BossCrowState
{
    public BossCrowDeath(BossCrow enemy, EnemyStateMachine<BossCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
