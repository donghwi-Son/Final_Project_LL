using UnityEngine;

public class WoodSpiritIdleState : State<WoodSpirit>
{
    public WoodSpiritIdleState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
        stateTimer = owner.idleTime;
    }

    public override void Execute()
    {
        base.Execute();

        if(stateTimer <= 0f)
        {
            owner.StateMachine.ChangeState(owner.MoveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
