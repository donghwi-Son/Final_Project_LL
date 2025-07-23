using UnityEngine;

public class PlayerJumpState : PlayerState
{
    PlayerController player => psm.player;
    public PlayerJumpState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        player.rb.linearVelocity = new Vector2(player.rb.linearVelocityX, player.jumpForce);
        player.anim.SetBool("isJumping", true);
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
        else if (player.rb.linearVelocityY < 0)
        {
            psm.ChangeState(player.FallingState);
        }
    }
 

    public override void ExitState()
    {
        base.ExitState();
        player.anim.SetBool("isJumping", false);
    }
}
