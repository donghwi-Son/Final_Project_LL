using UnityEngine;

public class PlayerAttackState : PlayerState
{
    PlayerController player => psm.player;

    bool CanCharge => player.stat.canChargeAttack;
    bool isHolding = false;
    float holdTime;
    float idleTimer = 0f;
    float requiredHoldTime = 1f;
    bool isHoldAttack = false;
    PlayerChargeBar chargeBar;

    float lastAutoFireTime;
    bool isAutoFiring = false;

    public PlayerAttackState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.rb.linearVelocityX = 0f;
        isHolding = true;
        holdTime = 0f;
        idleTimer = 0f;
        isHoldAttack = false;
        isAutoFiring = false;
        lastAutoFireTime = 0f;
        chargeBar = player.GetComponent<PlayerChargeBar>();

        // 근거리 모드에서만 차지바 표시
        if (CanCharge && player.attackMode == AttackMode.Melee)
            chargeBar?.ShowChargeBar();

        player.anim.speed = player.stat.attackSpeed.GetValue() / 300f;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (Input.GetMouseButton(0) && isHolding)
        {
            if (player.attackMode == AttackMode.Melee)
            {
                // 근거리 모드: 차지 공격 처리
                HandleMeleeChargeAttack();
            }
            else if (player.attackMode == AttackMode.Ranged)
            {
                // 원거리 모드: 자동 발사 처리
                HandleRangedAutoFire();
            }
            idleTimer = 0f; // 공격 중이므로 idle 타이머 초기화
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseButtonUp();
            idleTimer = 0f; // 공격이 끝났으므로 idle 타이머 초기화
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= 0.5f)
        {
            psm.ChangeState(player.IdleState);
        }
    }

    void HandleMeleeChargeAttack()
    {
        if (!CanCharge) return;

        holdTime += Time.deltaTime;
        chargeBar?.UpdateChargeBar(holdTime / requiredHoldTime);

        if (holdTime >= requiredHoldTime)
        {
            isHoldAttack = true;
        }
    }

    void HandleRangedAutoFire()
    {
        if (!isAutoFiring)
        {
            // 첫 발사
            player.AttackManager.Attack(player.attackMode, player.IsFacingRight);
            isAutoFiring = true;
            lastAutoFireTime = Time.time;
        }
        else if (Time.time >= lastAutoFireTime + player.stat.attackInterval)
        {
            // 자동 발사
            player.AttackManager.Attack(player.attackMode, player.IsFacingRight);
            lastAutoFireTime = Time.time;
        }
    }

    void HandleMouseButtonUp()
    {
        isHolding = false;
        isAutoFiring = false;

        if (player.attackMode == AttackMode.Melee)
        {
            // 근거리 모드: 차지바 숨기기 및 공격 실행
            if (CanCharge)
            {
                chargeBar?.HideChargeBar();
            }

            if (isHoldAttack && CanCharge)
            {
                player.AttackManager.ChargeAttack();
            }
            else
            {
                player.AttackManager.Attack(player.attackMode, player.IsFacingRight);
            }
        }
        else if (player.attackMode == AttackMode.Ranged && !isAutoFiring)
        {
            // 원거리 모드에서 즉시 떼는 경우 (자동발사가 시작되지 않은 경우)
            player.AttackManager.Attack(player.attackMode, player.IsFacingRight);
        }

        // 변수 초기화
        isHoldAttack = false;
        holdTime = 0f;
    }

    public override void ExitState()
    {
        base.ExitState();
        player.anim.ResetTrigger("Att");
        player.anim.speed = 1f;

        // 차지바 숨기기 (안전장치)
        if (chargeBar != null)
        {
            chargeBar.HideChargeBar();
        }
    }
}