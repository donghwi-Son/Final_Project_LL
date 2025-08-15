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
        stateTimer = 0.7f;
        owner.rb.linearVelocityX = 0f;
        Dash();
    }

    public override void Execute()
    {
        base.Execute();
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            stateMachine.ChangeState(owner.IdleState);
        }
        if(owner.AttackInput)
        {
            stateMachine.ChangeState(owner.DashAttState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsInvincible = false;
    }

    void Dash()
    {
        if(owner.XInput == 0)
        {
            owner.rb.AddForce(new Vector2(owner.IsFacingRight ? owner.dashPower : -owner.dashPower, 0), ForceMode2D.Impulse);
        }    
        owner.rb.AddForce(new Vector2(owner.XInput * owner.dashPower, 0), ForceMode2D.Impulse);
    }
}
