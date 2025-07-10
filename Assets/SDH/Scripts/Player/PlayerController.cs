using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;




public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashPower = 30f;

    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public float specialAttackCooldown = 2f;
    public float skillCooldown = 3f;

    [Header("Defense")]
    public float rollDistance = 3f;
    public float rollDuration = 0.3f;
    public float defendReduction = 0.5f;

    //땅체크
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

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

    //컴포넌트
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer spriteRenderer;

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
    public bool IsGrounded { get; private set; }
    public bool CanAttack { get; private set; } = true;
    public bool CanSpecialAttack { get; private set; } = true;
    public bool CanUseSkill { get; private set; } = true;
    public bool CanAirAttack = true;
    public bool CanFlip = true;
    public bool DoubleJumpActive = false;
    public bool CanDoubleJump = false;
    public bool IsRolling { get; private set; }
    public bool IsDefending { get; private set; }
    public bool isFacingRight { get; private set; } = true;

    //타이머
    float lastAttackTime;
    float lastSpecialAttackTime;
    float lastSkillTime;

    //기타변수
    public int DoubleJumpCount = 0;
    public float perfectDefendTime = 0.5f; // 완벽 방어 시간
    Vector2 mousePosition;
    public AttackMode attackMode = AttackMode.Melee; // 기본 공격 모드

    private void Awake()
    {
        InitState();
        InitComponents();
    }

    private void Start()
    {
        StateMachine.InitState(IdleState);
        originmoveSpeed = moveSpeed; // 7월 3일 추가 : 초기 지정 속도 저장
    }

    private void Update()
    {
        HandleInput();
        CheckGround();

        if(CanFlip)
            FlipByMouse();

        if(MeleeChangeInput)
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
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    void CheckGround()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (IsGrounded && !CanDoubleJump && DoubleJumpActive)
        {
            CanDoubleJump = true;
            DoubleJumpCount = 0;
        }
        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius, IsGrounded ? Color.green : Color.red);
    }

    void FlipByMouse()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isFacingRight = mousePosition.x > transform.position.x;
        if(isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void Move()
    {
        rb.linearVelocityX = XInput * moveSpeed;
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
            anim.SetFloat("SpecialVal", 1f);
        }
        else
        {
            attackMode = AttackMode.Melee;
            anim.SetInteger("AttackMode", 0);
            anim.SetFloat("SpecialVal", 0f);
        }
    }

    public void AttackToIdle()
    {
        StateMachine.ChangeState(IdleState);
    }

    // 7월 3일 추가 부분 : 플레이어가 Finish 태그를 가진 오브젝트와 충돌하면, StageManager의 Onfinish() 발동
    public StageManager stageManager;
    private float originmoveSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            rb.linearVelocity = Vector2.zero;
            moveSpeed = 0f;
            stageManager.OnFinish();
        }
    }

    // Finish 도달 시 멈춰진 속도를 다음 스테이지에서 다시 원래대로
    public void ResetSpeed()
    {
        rb.linearVelocity = Vector2.zero;
        moveSpeed = originmoveSpeed;
        enabled = true;
    }
}
