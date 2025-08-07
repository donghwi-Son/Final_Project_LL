using UnityEngine;

public class BossTaoistDeath : State<BossTaoist>
{
    public BossTaoistDeath(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
