using UnityEngine;

public class PlayerDefendState : State<PlayerController>
{
    public PlayerDefendState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
        stateTimer = 1f; // 방어 지속 시간 초기화
        owner.anim.SetBool("isDefending", true);
    }

    public override void Execute()
    {
        base.Execute();
        if (stateTimer >= owner.perfectDefendTime)
        {
            //완벽가드
        }
        else if(stateTimer > 0f)
        {
            //일반가드
        }
        else
        {
            stateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.anim.SetBool("isDefending", false);
    }

}
