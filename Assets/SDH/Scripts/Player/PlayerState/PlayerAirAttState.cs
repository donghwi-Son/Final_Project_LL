using UnityEngine;

public class PlayerAirAttState : PlayerState
{
    PlayerController player => psm.player;

    public PlayerAirAttState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.CanDoubleJump = false;
        player.CanAirAttack = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(player.rb.linearVelocityY < 0 )
            psm.ChangeState(player.FallingState);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
