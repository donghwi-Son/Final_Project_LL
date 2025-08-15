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
        stateTimer = 0.5f; // 방어 지속 시간 초기화
        owner.anim.SetBool("isDefending", true);
    }

    public override void Execute()
    {
        base.Execute();
        if (stateTimer >= owner.perfectDefendTime)
        {
            owner.IsPerfectDefend = true;
        }
        else if(stateTimer > 0f)
        {
            owner.IsNormalDefend = true;
            owner.IsPerfectDefend = false;
        }  
        else
        {
            stateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsNormalDefend = false;
        owner.anim.SetBool("isDefending", false);
    }

}
