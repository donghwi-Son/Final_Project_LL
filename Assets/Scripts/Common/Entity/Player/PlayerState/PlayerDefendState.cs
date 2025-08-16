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
        stateTimer = owner.DefendDuration;
    }

    public override void Execute()
    {
        base.Execute();

        if (stateTimer <= 0f)
        {
            stateMachine.ChangeState(owner.IdleState);
            
        }
        else if(stateTimer <= owner.perfectDefendTime)
        {
            owner.IsNormalDefend = true;
            owner.IsPerfectDefend = false;
        }  
        else
        {
            owner.IsPerfectDefend = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsNormalDefend = false;
    }
}
