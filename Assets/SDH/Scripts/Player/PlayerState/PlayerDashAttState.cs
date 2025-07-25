using UnityEngine;

public class PlayerDashAttState : State<PlayerController>
{
    public PlayerDashAttState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.AttackManager.DashAttack(owner.IsFacingRight);
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
