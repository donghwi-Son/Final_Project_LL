using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerState
{
    PlayerController player => psm.player;

    public PlayerIdleState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Player Idle State Entered");
        player.rb.linearVelocityX = 0f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(player.JumpInput && player.IsGrounded)
        {
            psm.ChangeState(player.JumpState);
        }
        if (player.AttackInput && player.IsGrounded)
        {
            psm.ChangeState(player.AttackState);
        }
        if (player.XInput != 0 && player.IsGrounded)
        {
            psm.ChangeState(player.MoveState);
        }
        if (player.SpecialAttackInput && player.IsGrounded)
        {
            psm.ChangeState(player.SpecialAttackState);
        }
        if (player.SkillInput)
        {
            psm.ChangeState(player.SkillState);
        }
        if (player.DashInput && player.IsGrounded)
        {
            psm.ChangeState(player.DashState);
        }
        if (player.DefendInput)
        {
            psm.ChangeState(player.DefendState);
        }
        if (!player.IsGrounded)
        {
            psm.ChangeState(player.FallingState);
        }

    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
