using UnityEngine;

public class PlayerDashAtkState : State<PlayerController>
{
    public PlayerDashAtkState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.IsInvincible = true;
        owner.AttackManager.DashAttack(owner.IsFacingRight);
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            stateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsInvincible = false;
    }
}
