using UnityEngine;

public class BossCrowStrikeAttack : State<BossCrow>
{
    private Vector2 dashDir;
    private int stateType;
    private float reboundInvulTime; // 반동 후 충돌 무시 시간
    private float reboundDuration;

    public BossCrowStrikeAttack(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateType = 0;
        reboundInvulTime = 0.2f;
        reboundDuration = 0.3f;

        owner.BossFlip(false);
    }

    public override void Execute()
    {
        base.Execute();

        if(stateType == 0)
        {
            // 뒤로 살짝 반동 후 공격으로 상태이동
            Vector2 retreatDir = (owner.transform.position - owner.playerTrans.position).normalized;
            owner.rb.linearVelocity = retreatDir * owner.moveSpeed;
            if (triggerCalled)
            {
                stateType = 1;
                triggerCalled = false;

                // 플레이어 거리 계산
                dashDir = (owner.playerTrans.position - owner.transform.position).normalized;
                // 회전값
                owner.anim.transform.rotation = Quaternion.Euler(0, 0, owner.BossPlayerGaze());

                owner.anim.SetTrigger("OnStrike");

                owner.StrikeColliderSwitch(true);
            }            
        }
        else if(stateType == 1)
        {
            owner.rb.linearVelocity = dashDir * owner.dashAttackSpeed;
            reboundInvulTime -= Time.deltaTime;

            // 충돌 여부 등등 추가
            if (owner.IsWallDetected() && reboundInvulTime <= 0)
            {
                owner.rb.linearVelocity = new Vector2(-owner.rb.linearVelocity.x, owner.rb.linearVelocity.y).normalized * owner.dashReboundSpeed;
                stateType = 2;
            }
            else if (owner.IsGroundDetected() && reboundInvulTime <= 0)
            {
                owner.rb.linearVelocity = new Vector2(owner.rb.linearVelocity.x, Mathf.Abs(owner.rb.linearVelocity.y)).normalized * owner.dashReboundSpeed;
                stateType = 2;
            }

            if (stateType == 2)
            {
                owner.anim.SetTrigger("IsRebound");
                owner.SetRotationZero();
                owner.StrikeColliderSwitch(false);
            }
            else if (triggerCalled)
            {
                owner.stateMachine.ChangeState(owner.idleState);
                owner.SetRotationZero();
                owner.StrikeColliderSwitch(false);
            }

        }
        else if (stateType == 2)
        {
            reboundDuration -= Time.deltaTime;
            if (reboundDuration <= 0 && triggerCalled)
            {
                stateType = 3;
            }
        }
        else if (stateType == 3)
        {
            owner.stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.SetZeroVelocity();
    }
}
