using UnityEngine;

public class BossCrowSpawnEnemy : State<BossCrow>
{
    public BossCrowSpawnEnemy(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 1.2f;
        owner.BossSpawnEnemy();
    }

    public override void Execute()
    {
        base.Execute();

        if(stateTimer <= 0)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
