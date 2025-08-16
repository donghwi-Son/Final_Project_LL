using UnityEngine;

public class BossCrowStand : State<BossCrow>
{
    public BossCrowStand(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        owner.DetectPlayer();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
