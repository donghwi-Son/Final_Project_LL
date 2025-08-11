using UnityEngine;

public class BurstWispDeadState : State<BurstWisp>
{
    public BurstWispDeadState(BurstWisp owner, StateMachine<BurstWisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetZeroVelocity();

        if (triggerCalled)
        {
            GameObject.Destroy(owner.gameObject);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
