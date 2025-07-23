using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;




public class PlayerController : Entity
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashPower = 30f;

    [Header("CoolDown")]
    public float specialAttackCooldown = 5f;
    public float dashCooldown = 3f;
    public float skillCooldown = 3f;

    [Header("Defense")]
    public float rollDistance = 3f;
    public float rollDuration = 0.3f;
    public float defendReduction = 0.5f;

    //스테이트 및 컨트롤
    public PlayerStateMachine StateMachine;
    public PlayerIdleState IdleState;
    public PlayerMoveState MoveState;
    public PlayerJumpState JumpState;
    public PlayerAttackState AttackState;
    public PlayerSpecialAttackState SpecialAttackState;
    public PlayerSkillState SkillState;
    public PlayerDashState DashState;
    public PlayerDefendState DefendState;
    public PlayerAirAttState AirAttState;
    public PlayerDashAttState DashAttState;
    public PlayerFallingState FallingState;
    public AttackManager AttackManager;
    public PlayerStatus stat;

    //인풋 변수들
    public float XInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool SpecialAttackInput { get; private set; }
    public bool SkillInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool DefendInput { get; private set; }
    public bool MeleeChangeInput { get; private set; }

    //불체크값
    public bool CanAttack { get; private set; } = true;
    public bool CanUseSkill { get; private set; } = true;
    public bool CanAirAttack = true;
    public bool DoubleJumpActive = false;
    public bool CanDoubleJump = false;
    public bool IsRolling { get; private set; }
    public bool IsDefending { get; private set; }
    public bool CanUseDash => Time.time >= lastDashTime + dashCooldown;
    public bool CanUseSpecialAttack => Time.time >= lastSpecialAttackTime + specialAttackCooldown;

    //타이머
    public float lastSpecialAttackTime = -999f;
    public float lastDashTime = -999f;

    //기타변수
    public int DoubleJumpCount = 0;
    public float perfectDefendTime = 0.5f; // 완벽 방어 시간
    Vector2 mousePosition;
    public AttackMode attackMode = AttackMode.Melee; // 기본 공격 모드

    protected override void Awake()
    {
        base.Awake();

        InitState();
        InitComponents();
    }

    private void Start()
    {
        StateMachine.InitState(IdleState);
        //originmoveSpeed = moveSpeed; // 7월 3일 추가 : 초기 지정 속도 저장
    }

    private void Update()
    {
        HandleInput();
        CheckGround();

        if (MeleeChangeInput)
            ChangeAttackMethod();

        StateMachine.Update();
    }


    void InitState()
    {
        StateMachine = new PlayerStateMachine(this);
        IdleState = new PlayerIdleState(StateMachine);
        MoveState = new PlayerMoveState(StateMachine);
        JumpState = new PlayerJumpState(StateMachine);
        AttackState = new PlayerAttackState(StateMachine);
        SpecialAttackState = new PlayerSpecialAttackState(StateMachine);
        SkillState = new PlayerSkillState(StateMachine);
        DefendState = new PlayerDefendState(StateMachine);
        DashState = new PlayerDashState(StateMachine);
        AirAttState = new PlayerAirAttState(StateMachine);
        DashAttState = new PlayerDashAttState(StateMachine);
        FallingState = new PlayerFallingState(StateMachine);
    }

    void InitComponents()
    {
        AttackManager = GetComponent<AttackManager>();
        stat = GetComponent<PlayerStatus>();
    }

    void HandleInput()
    {
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

    public void AttackToIdle()
    {
        StateMachine.ChangeState(IdleState);
    }

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
