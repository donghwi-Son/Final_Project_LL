using UnityEngine;

public class WoodSpiritMoveState : State<Enemy>
{
    public WoodSpiritMoveState(Enemy owner, StateMachine<Enemy> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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
