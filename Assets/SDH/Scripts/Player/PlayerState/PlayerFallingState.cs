using UnityEngine;

public class PlayerFallingState : PlayerState
{
    PlayerController player => psm.player;
    public PlayerFallingState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.anim.SetBool("isFalling", true);
        Debug.Log("Player Falling State Entered");
    }

    public override void UpdateState()
    {
        base.UpdateState();
        player.SetVelocity(player.XInput * player.moveSpeed, player.rb.linearVelocityY);
        if (player.CanDoubleJump && player.JumpInput && !player.IsGroundDetected())
        {
            player.DoubleJump();
        }
        else if (player.AttackInput && player.CanAirAttack)
        {
            psm.ChangeState(player.AirAttState);
        }
        else if (player.IsGroundDetected())
        {
            player.CanAirAttack = true;
            psm.ChangeState(player.IdleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
