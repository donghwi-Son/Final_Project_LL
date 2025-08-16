using UnityEngine;

public class PlayerDashState : State<PlayerController>
{
    public PlayerDashState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.IsInvincible = true;
        owner.lastDashTime = Time.time;
        stateTimer = owner.DashDuration;
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetVelocity(owner.DashSpeed * owner.FacingDir, 0);

        if (stateTimer <= 0f)
        {
            stateMachine.ChangeState(owner.IdleState);
        }
        else if(owner.AttackInput)
        {
            stateMachine.ChangeState(owner.DashAtkState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsInvincible = false;
    }
}
