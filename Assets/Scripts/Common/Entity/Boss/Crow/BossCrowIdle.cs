using UnityEngine;

public class BossCrowIdle : State<BossCrow>
{
    private float delayTimer;

    public BossCrowIdle(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        delayTimer = 0;
    }

    public override void Execute()
    {
        base.Execute();
        owner.ChasePlayer();

        delayTimer += Time.deltaTime;

        if (delayTimer >= owner.attackDealy)
        {
            owner.nextAttackSelect();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
