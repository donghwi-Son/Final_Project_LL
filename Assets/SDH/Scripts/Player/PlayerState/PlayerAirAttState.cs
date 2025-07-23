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
        Debug.Log("Player Air Attack State Entered");
        player.CanDoubleJump = false;
        player.CanAirAttack = false;
        player.AttackManager.AirAttack();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
