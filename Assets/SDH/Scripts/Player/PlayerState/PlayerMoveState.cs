using UnityEngine;

public class PlayerMoveState : PlayerState
{
    PlayerController player => psm.player;
    public PlayerMoveState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Player Move State Entered");
        player.anim.SetBool("isMoving", true);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        player.Move();
        if (player.JumpInput && player.IsGrounded)
        {
            psm.ChangeState(player.JumpState);
        }
        if (!player.IsGrounded)
        {
            psm.ChangeState(player.FallingState);
        }
        if (player.AttackInput)
        {
            psm.ChangeState(player.AttackState);
        }
        if(player.SpecialAttackInput)
        {
            psm.ChangeState(player.SpecialAttackState);
        }
        if (player.SkillInput)
        {
            psm.ChangeState(player.SkillState);
        }
        if (player.DashInput)
        {
            psm.ChangeState(player.DashState);
        }
        if (player.DefendInput)
        {
            psm.ChangeState(player.DefendState);
        }
        if (player.XInput == 0 && player.IsGrounded)
        {
            psm.ChangeState(player.IdleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        player.anim.SetBool("isMoving", false);
    }
}
