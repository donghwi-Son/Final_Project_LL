using UnityEngine;

public class PlayerDeadState : State<PlayerController>
{
    public PlayerDeadState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
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
            // 게임 오버 로직
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
