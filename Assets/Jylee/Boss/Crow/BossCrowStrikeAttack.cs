using UnityEngine;

public class BossCrowStrikeAttack : State<BossCrow>
{
    private Vector2 dashDir;
    private int stateType;

    public BossCrowStrikeAttack(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateType = 0;

        owner.BossFlip(false);
    }

    public override void Execute()
    {
        base.Execute();

        if(stateType == 0)
        {
            // 뒤로 살짝 반동 후 공격으로 상태이동
            Vector2 retreatDir = (owner.transform.position - owner.playerTransform.position).normalized;
            owner.rb.linearVelocity = retreatDir * owner.moveSpeed;
            if (triggerCalled)
            {
                stateType = 1;
                triggerCalled = false;

                // 플레이어 거리 계산
                dashDir = (owner.playerTransform.position - owner.transform.position).normalized;
                // 회전값
                owner.anim.transform.rotation = Quaternion.Euler(0, 0, owner.BossPlayerGaze());

                owner.anim.SetTrigger("OnStrike");
            }            
        }
        else if(stateType == 1)
        {
            owner.rb.linearVelocity = dashDir * owner.dashAttackSpeed;

            // 충돌 여부 등등 추가

            if (triggerCalled)
            {
                owner.SetRotationZero();
                owner.stateMachine.ChangeState(owner.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.SetZeroVelocity();
    }
}
