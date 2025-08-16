using UnityEngine;

public class PlayerController : Entity
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    [SerializeField] private float jumpForce = 12f;
    public float JumpForce { get => jumpForce; }
    [SerializeField] private float dashSpeed = 10f;
    public float DashSpeed { get => dashSpeed; }
    [SerializeField] private float dashDuration = 0.5f;
    public float DashDuration { get => dashDuration; }
    [SerializeField] private int maxDashCount = 1; // 최대 대시 횟수
    public int MaxDashCount { get => maxDashCount; set => maxDashCount = value; }

    [Header("CoolDown")]
    [SerializeField] private float speicalAttackCooldown = 5f;
    public float SpecialAttackCooldown { get => speicalAttackCooldown; set => speicalAttackCooldown = value; }
    [SerializeField] private float dashCooldown = 3f;
    public float DashCooldown { get => dashCooldown; set => dashCooldown = value; }
    [SerializeField] private float skillCooldown = 3f;
    public float SkillCooldown { get => skillCooldown; set => skillCooldown = value; }

    [Header("Offense")]
    [SerializeField] private float attackSpeed = 1f; // 공격 속도
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    [SerializeField] private float attackRange = 1.5f; // 공격 범위
    public float AttackRange { get => attackRange; set => attackRange = value; }

    [Header("Defense")]
    [SerializeField] private float defendReduction = 0.5f; // 방어 시 피해 감소 비율
    public float DefendReduction { get => defendReduction; set => defendReduction = value; }

    public float AttackInterval => 1f / AttackSpeed;

    public AttackManager AttackManager { get; private set; }
    public PlayerStats Stats { get; private set; }

    #region States
    public StateMachine<PlayerController> StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerSpecialAtkState SpecialAtkState { get; private set; }
    public PlayerSkillState SkillState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerDefendState DefendState { get; private set; }
    public PlayerAirAtkState AirAtkState { get; private set; }
    public PlayerDashAtkState DashAtkState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }
    #endregion

    #region Inputs
    public float XInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool SpecialAttackInput { get; private set; }
    public bool SkillInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool DefendInput { get; private set; }
    public bool MeleeChangeInput { get; private set; }
    #endregion

    #region Boolean Flags
    public bool CanAttack { get; private set; } = true;
    public bool CanUseSkill { get; private set; } = true;
    public bool CanAirAttack = true;
    public static bool DoubleJumpActive = false;
    public bool CanDoubleJump = false;
    public bool IsRolling { get; private set; }
    public bool IsDefending { get; private set; }
    public bool CanUseDash => Time.time >= lastDashTime + DashCooldown;
    public bool CanUseSpecialAttack => Time.time >= lastSpecialAttackTime + SpecialAttackCooldown;
    public bool IsGetHitStun = false;
    public bool IsPerfectDefend = false; // 완벽 방어 여부
    public bool IsNormalDefend = false;
    public bool IsInvincible = false; // 무적 상태 여부
    #endregion

    //타이머
    public float lastSpecialAttackTime = -999f;
    public float lastDashTime = -999f;

    //기타변수
    public int DoubleJumpCount = 0;
    public float perfectDefendTime = 0.3f; // 완벽 방어 시간
    Vector2 mousePosition;
    public AttackMode attackMode = AttackMode.Melee; // 기본 공격 모드
    public Material defaultMaterial;
    public Material hitMaterial;

    protected override void Awake()
    {
        base.Awake();

        InitState();
        InitComponents();

        PlayerManager.Instance.player = this; // 플레이어 매니저에 플레이어 설정
    }

    protected override void Start()
    {
        base.Start();

        StateMachine.ChangeState(IdleState);

        //originmoveSpeed = moveSpeed; // 7월 3일 추가 : 초기 지정 속도 저장
    }

    protected override void Update()
    {
        base.Update();

        HandleInput();
        CheckGround();
        anim.SetFloat("yVelocity", rb.linearVelocityY);

        if (MeleeChangeInput)
            ChangeAttackMethod();

        StateMachine.CurrentState.Execute();
    }

    void InitState()
    {
        StateMachine = new StateMachine<PlayerController>();
        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AttackState = new PlayerAttackState(this, StateMachine, "Attack");
        SpecialAtkState = new PlayerSpecialAtkState(this, StateMachine, "SpecialAttack");
        SkillState = new PlayerSkillState(this, StateMachine, "Skill");
        DefendState = new PlayerDefendState(this, StateMachine, "Defend");
        DashState = new PlayerDashState(this, StateMachine, "Dash");
        AirAtkState = new PlayerAirAtkState(this, StateMachine, "AirAttack");
        DashAtkState = new PlayerDashAtkState(this, StateMachine, "DashAttack");
        FallState = new PlayerFallState(this, StateMachine, "Jump");
        HitState = new PlayerHitState(this, StateMachine, "Idle");
        DeadState = new PlayerDeadState(this, StateMachine, "Dead");
    }

    private void InitComponents()
    {
        AttackManager = GetComponent<AttackManager>();
        Stats = GetComponent<PlayerStats>();
    }

    private void HandleInput()
    {
        if (IsGetHitStun)
        {
            XInput = 0f; // 피격 상태에서는 이동 입력을 무시
            JumpInput = false;
            AttackInput = false;
            SpecialAttackInput = false;
            SkillInput = false;
            DashInput = false;
            DefendInput = false;
            MeleeChangeInput = false;
            return; // 피격 상태에서는 다른 입력을 처리하지 않음
        }
        XInput = Input.GetAxisRaw("Horizontal");
        JumpInput = Input.GetKeyDown(KeyCode.Space);
        AttackInput = Input.GetMouseButtonDown(0);
        SpecialAttackInput = Input.GetMouseButtonDown(1);
        SkillInput = Input.GetKeyDown(KeyCode.Q);
        DashInput = Input.GetKeyDown(KeyCode.LeftShift);
        DefendInput = Input.GetKeyDown(KeyCode.LeftControl);
        MeleeChangeInput = Input.GetKeyDown(KeyCode.Tab);
    }

    private void CheckGround()
    {
        if (IsGroundDetected() && !CanDoubleJump && DoubleJumpActive)
        {
            CanDoubleJump = true;
            DoubleJumpCount = 0;
        }
        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius, IsGroundDetected() ? Color.green : Color.red);
    }

    public void FlipByMouse()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float delta = mousePosition.x - transform.position.x;
        FlipController(delta);
    }

    public void DoubleJump()
    {
        CanDoubleJump = false;
        rb.linearVelocityY = 0;
        StateMachine.ChangeState(JumpState);
    }

    public void ChangeAttackMethod()
    {
        if(attackMode == AttackMode.Melee)
        {
            attackMode = AttackMode.Ranged;
            anim.SetInteger("AttackMode", 1);
        }
        else
        {
            attackMode = AttackMode.Melee;
            anim.SetInteger("AttackMode", 0);
        }
    }

    // 간이 유틸리티 매니저 ===============
    public void EnableDoubleJump()
    {
        DoubleJumpActive = true;
    }

    public void AddDashCount(int count)
    {
        MaxDashCount += count;
    }

    public void DecreaseDashCooldown(float percent)
    {
        DashCooldown *= 1f - percent / 100f; // 감소 비율 적용
    }

    public void ApplyUtility(ItemInfo.UtilityType type, float amount)
    {
        switch (type)
        {
            case ItemInfo.UtilityType.DoubleJump:
                EnableDoubleJump();
                break;
            case ItemInfo.UtilityType.AddDashCount:
                AddDashCount((int)amount);
                break;
            case ItemInfo.UtilityType.DashCoolDown:
                DecreaseDashCooldown(amount);
                break;
            default:
                Debug.LogWarning("알 수 없는 유틸리티 타입: " + type);
                break;
        }
    }

    public override void DamageImpact()
    {
        base.DamageImpact();

        if (IsGetHitStun) return;
        if (IsInvincible) return; // 무적 상태에서는 피격 처리하지 않음
        if (IsPerfectDefend)
        {
            spriteRenderer.material = hitMaterial; // 피격 시 머티리얼 변경  
            IsInvincible = true; // 무적 상태로 전환
            Invoke("ResetMaterial", 0.3f);
            Invoke("ResetInvincible", 1f); // 무적 상태 해제 타이머 설정
        }
        else if (IsNormalDefend)
        {
            // 일반 방어 상태에서는 피해를 반감
            spriteRenderer.material = hitMaterial; // 피격 시 머티리얼 변경
            IsInvincible = true; // 무적 상태로 전환
            Invoke("ResetMaterial", 0.3f);
            Invoke("ResetInvincible", 0.5f); // 무적 상태 해제 타이머 설정
        }
        else
        {
            IsGetHitStun = true;
            IsInvincible = true; // 무적 상태로 전환
            Invoke("ResetInvincible", 1f); // 무적 상태 해제 타이머 설정
            GetKnockBack();
            spriteRenderer.material = hitMaterial;
            StateMachine.ChangeState(HitState);

            Invoke("ResetMaterial", 0.3f);
        }
    }

    public void GetHit(int dmg)
    {
        if (IsGetHitStun) return;
        if (IsInvincible) return; // 무적 상태에서는 피격 처리하지 않음
        if (IsPerfectDefend)
        {
            spriteRenderer.material = hitMaterial; // 피격 시 머티리얼 변경  
            IsInvincible = true; // 무적 상태로 전환
            Invoke("ResetMaterial", 0.3f);
            Invoke("ResetInvincible", 1f); // 무적 상태 해제 타이머 설정
        }
        else if (IsNormalDefend)
        {
            // 일반 방어 상태에서는 피해를 반감
            dmg = Mathf.CeilToInt(dmg * (1 - DefendReduction));
            Stats.TakeDamage(dmg);
            spriteRenderer.material = hitMaterial; // 피격 시 머티리얼 변경
            IsInvincible = true; // 무적 상태로 전환
            Invoke("ResetMaterial", 0.3f);
            Invoke("ResetInvincible", 0.5f); // 무적 상태 해제 타이머 설정
        }
        else
        {
            IsGetHitStun = true;
            IsInvincible = true; // 무적 상태로 전환
            Invoke("ResetInvincible", 1f); // 무적 상태 해제 타이머 설정
            Stats.TakeDamage(dmg);
            GetKnockBack();
            spriteRenderer.material = hitMaterial;
            StateMachine.ChangeState(HitState);

            Invoke("ResetMaterial", 0.3f);
        }
    }

    void ResetMaterial()
    {
        spriteRenderer.material = defaultMaterial; // 원래 머티리얼로 되돌림
    }

    void ResetInvincible()
    {
        IsInvincible = false; // 무적 상태 해제
    }

    void GetKnockBack()
    {
        rb.linearVelocity = new Vector2(0, 0);
        rb.AddForce(new Vector2(-FacingDir * 5f, 5f), ForceMode2D.Impulse);
    }

    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public override void Die()
    {
        base.Die();

        StateMachine.ChangeState(DeadState);
    }

    // 간이 유틸리티 매니저 ===============



    //// 7월 3일 추가 부분 : 플레이어가 Finish 태그를 가진 오브젝트와 충돌하면, StageManager의 Onfinish() 발동
    //public StageManager stageManager;
    //private float originmoveSpeed;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Finish"))
    //    {
    //        rb.linearVelocity = Vector2.zero;
    //        moveSpeed = 0f;
    //        stageManager.OnFinish();
    //    }
    //}

    //// Finish 도달 시 멈춰진 속도를 다음 스테이지에서 다시 원래대로
    //public void ResetSpeed()
    //{
    //    rb.linearVelocity = Vector2.zero;
    //    moveSpeed = originmoveSpeed;
    //    enabled = true;
    //}



}
