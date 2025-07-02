using UnityEngine;

public class PlayerAttackState : PlayerState
{
    PlayerController player => psm.player;

    public PlayerAttackState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Player Attack State Entered");
        player.rb.linearVelocityX = 0f;
        player.AttackManager.Attack(player.attackMode);
        player.CanFlip = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (player.AttackInput)
        {
            player.AttackManager.Attack(player.attackMode);
        }    
    }

    public override void ExitState()
    {
        base.ExitState();
        player.CanFlip = true;
    }

}
