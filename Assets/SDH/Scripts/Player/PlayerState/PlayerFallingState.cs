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
        else if (player.IsGrounded)
        {
            if(player.DoubleJumpActive && !player.CanDoubleJump)
            {
                player.CanDoubleJump = true;
            }
            player.CanAirAttack = true;
           // player.anim.SetBool("isGround", true);
            psm.ChangeState(player.IdleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        player.anim.SetBool("isFalling", false);
    }
}
