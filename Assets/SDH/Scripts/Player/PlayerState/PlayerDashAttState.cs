using UnityEngine;

public class PlayerDashAttState : PlayerState
{
    PlayerController player => psm.player;

    public PlayerDashAttState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
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
