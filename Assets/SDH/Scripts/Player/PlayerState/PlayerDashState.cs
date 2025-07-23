using UnityEngine;

public class PlayerDashState : PlayerState
{
    PlayerController player => psm.player;
    
    float dashTime; // 대시 지속 시간
    public PlayerDashState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.lastDashTime = Time.time;
        player.anim.SetBool("isDashing", true);
        dashTime = 0.7f;
        player.rb.linearVelocityX = 0f;
        Dash();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        dashTime -= Time.deltaTime;
        if (dashTime <= 0f)
        {
            psm.ChangeState(player.IdleState);
        }
        if(player.AttackInput)
        {
            psm.ChangeState(player.DashAttState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        player.anim.SetBool("isDashing", false);
    }

    void Dash()
    {
        if(player.XInput == 0)
        {
            player.rb.AddForce(new Vector2(player.IsFacingRight ? player.dashPower : -player.dashPower, 0), ForceMode2D.Impulse);
        }    
        player.rb.AddForce(new Vector2(player.XInput * player.dashPower, 0), ForceMode2D.Impulse);
    }
}
