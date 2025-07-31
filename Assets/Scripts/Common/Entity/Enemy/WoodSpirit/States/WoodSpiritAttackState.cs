using UnityEngine;

public class WoodSpiritAttackState : State<WoodSpirit>
{
    public WoodSpiritAttackState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.SetZeroVelocity();
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            stateMachine.ChangeState(owner.MoveState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.lastTimeAttacked = Time.time;
    }
}
