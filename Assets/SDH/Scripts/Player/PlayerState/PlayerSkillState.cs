using UnityEngine;

public class PlayerSkillState : PlayerState
{
    PlayerController player => psm.player;
    public PlayerSkillState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        //player.anim.SetTrigger("Skill");
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
