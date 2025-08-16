using UnityEngine;

public class BossTaoistIdle : State<BossTaoist>
{
    public BossTaoistIdle(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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
