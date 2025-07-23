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
        player.lastSpecialAttackTime = Time.time;
        switch (player.attackMode)
        {
            case AttackMode.Melee:
                player.anim.SetTrigger("SpecialAtt");
                player.AttackManager.SpecialMeleeAttack();
                break;
            case AttackMode.Ranged:
                player.AttackManager.SpecialRangedAttack();
                psm.ChangeState(player.IdleState);
                break;
        }


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
