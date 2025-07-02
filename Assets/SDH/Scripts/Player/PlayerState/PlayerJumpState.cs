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
        Jump();
        player.anim.SetBool("isJumping", true);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        player.Move();
        if (player.CanDoubleJump && player.JumpInput && !player.IsGrounded)
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

    void Jump()
    {
        player.rb.AddForce(new Vector2(0, player.jumpForce), ForceMode2D.Impulse);
    }

}
