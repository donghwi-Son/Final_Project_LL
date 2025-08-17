using System.Collections;
using UnityEngine;

public class PlayerDeadState : State<PlayerController>
{
    public PlayerDeadState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.Instance.PlaySFX(SFX.PlayerDeath);
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetZeroVelocity();

        if (triggerCalled)
        {
            // 게임 오버 로직
            GameManager.Instance.OnGameOver();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
