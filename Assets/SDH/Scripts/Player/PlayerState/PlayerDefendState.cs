using UnityEngine;

public class PlayerDefendState : PlayerState
{
    PlayerController player => psm.player;
    float defendTime; // 방어 지속 시간
    public PlayerDefendState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.rb.linearVelocityX = 0f;
        player.CanFlip = false;
        defendTime = 1f; // 방어 지속 시간 초기화
        player.anim.SetBool("isDefending", true);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        defendTime -= Time.deltaTime;
        if (defendTime >= player.perfectDefendTime)
        {
            //완벽가드
        }
        else if(defendTime > 0f)
        {
            //일반가드
        }
        else
        {
            psm.ChangeState(player.IdleState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        player.CanFlip = true;
        player.anim.SetBool("isDefending", false);
    }

}
