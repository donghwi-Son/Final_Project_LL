using UnityEngine;

public class BossCrowStrikeAttack : BossCrowState
{
    private Vector2 dashDir;
    public BossCrowStrikeAttack(BossCrow enemy, EnemyStateMachine<BossCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateType = 0;

        enemy.BossFlip(false);
    }

    public override void Update()
    {
        base.Update();

        if(stateType == 0)
        {
            // 뒤로 살짝 반동 후 공격으로 상태이동
            Vector2 retreatDir = (enemy.transform.position - enemy.playerTransform.position).normalized;
            enemy.rb.linearVelocity = retreatDir * enemy.moveSpeed;
            if (triggerCalled)
            {
                stateType = 1;
                triggerCalled = false;

                // 플레이어 거리 계산
                dashDir = (enemy.playerTransform.position - enemy.transform.position).normalized;
                // 회전값
                enemy.anim.transform.rotation = Quaternion.Euler(0, 0, enemy.BossPlayerGaze());

                enemy.anim.SetTrigger("OnStrike");
            }            
        }
        else if(stateType == 1)
        {
            enemy.rb.linearVelocity = dashDir * enemy.dashAttackSpeed;

            // 충돌 여부 등등 추가

            if (triggerCalled)
            {
                enemy.BossRotationZero();
                enemy.stateMachine.ChangeState(enemy.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetZeroVelocity();
    }
}
