using UnityEngine;

public class PlayerSpecialAttackState : PlayerState
{
    PlayerController player => psm.player;
    public PlayerSpecialAttackState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
       // player.anim.SetTrigger("SpecialAttack");
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
