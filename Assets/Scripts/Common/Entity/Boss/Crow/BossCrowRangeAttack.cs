using UnityEngine;

public class BossCrowRangeAttack : State<BossCrow>
{
    private int stateType;

    public BossCrowRangeAttack(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
        owner.BossFlip(false);

        stateType = 0;
    }

    public override void Execute()
    {
        base.Execute();

        if(stateType == 0 && triggerCalled)
        {
            triggerCalled = false;
            stateType = 1;
            owner.BossRangeAttack();
        }
        else if(stateType == 1 && triggerCalled)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
