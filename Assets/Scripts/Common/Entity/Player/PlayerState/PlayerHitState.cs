using UnityEngine;

public class PlayerHitState : State<PlayerController>
{
    private const float HIT_STUN_DURATION = 0.4f;

    public PlayerHitState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = HIT_STUN_DURATION;
    }

    public override void Execute()
    {
        base.Execute();

        // 히트스턴 시간이 끝나면 적절한 상태로 전환
        if (stateTimer <= 0)
        {
            owner.IsGetHitStun = false;

            if (owner.IsGroundDetected())
            {
                stateMachine.ChangeState(owner.IdleState);
            }
            else
            {
                stateMachine.ChangeState(owner.FallState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsGetHitStun = false;
    }
}