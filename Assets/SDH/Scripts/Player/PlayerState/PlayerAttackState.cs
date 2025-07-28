using UnityEngine;

public class PlayerAttackState : State<PlayerController>
{
    bool CanCharge => owner.stat.canChargeAttack;
    bool isHolding = false;
    float holdTime;
    float idleTimer = 0f;
    float requiredHoldTime = 1f;
    bool isHoldAttack = false;
    PlayerChargeBar chargeBar;

    float lastAutoFireTime;
    bool isAutoFiring = false;

    public PlayerAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.rb.linearVelocityX = 0f;
        isHolding = true;
        holdTime = 0f;
        idleTimer = 0f;
        isHoldAttack = false;
        isAutoFiring = false;
        lastAutoFireTime = 0f;
        chargeBar = owner.GetComponent<PlayerChargeBar>();

        // 근거리 모드에서만 차지바 표시
        if (CanCharge && owner.attackMode == AttackMode.Melee)
            chargeBar?.ShowChargeBar();

        owner.anim.speed = owner.stat.attackSpeed.GetValue() / 300f;
    }

    public override void Execute()
    {
        base.Execute();

        if (Input.GetMouseButton(0) && isHolding)
        {
            if (owner.attackMode == AttackMode.Melee)
            {
                // 근거리 모드: 차지 공격 처리
                HandleMeleeChargeAttack();
            }
            else if (owner.attackMode == AttackMode.Ranged)
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
            stateMachine.ChangeState(owner.IdleState);
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
            owner.AttackManager.Attack(owner.attackMode, owner.IsFacingRight);
            isAutoFiring = true;
            lastAutoFireTime = Time.time;
        }
        else if (Time.time >= lastAutoFireTime + owner.stat.attackInterval)
        {
            // 자동 발사
            owner.AttackManager.Attack(owner.attackMode, owner.IsFacingRight);
            lastAutoFireTime = Time.time;
        }
    }

    void HandleMouseButtonUp()
    {
        isHolding = false;
        isAutoFiring = false;

        if (owner.attackMode == AttackMode.Melee)
        {
            // 근거리 모드: 차지바 숨기기 및 공격 실행
            if (CanCharge)
            {
                chargeBar?.HideChargeBar();
            }

            if (isHoldAttack && CanCharge)
            {
                owner.AttackManager.ChargeAttack();
            }
            else
            {
                owner.AttackManager.Attack(owner.attackMode, owner.IsFacingRight);
            }
        }
        else if (owner.attackMode == AttackMode.Ranged && !isAutoFiring)
        {
            // 원거리 모드에서 즉시 떼는 경우 (자동발사가 시작되지 않은 경우)
            owner.AttackManager.Attack(owner.attackMode, owner.IsFacingRight);
        }

        // 변수 초기화
        isHoldAttack = false;
        holdTime = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        owner.anim.speed = 1f;

        // 차지바 숨기기 (안전장치)
        if (chargeBar != null)
        {
            chargeBar.HideChargeBar();
        }
    }
}