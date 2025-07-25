using UnityEngine;

public class Entity : MonoBehaviour
{
    //컴포넌트
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    [Header("Collision Check")]
    [SerializeField] protected Transform groundCheck; // 바닥 체크 위치
    [SerializeField] protected float groundCheckRadius = 0.3f; // 바닥 체크 거리
    [SerializeField] protected Transform wallCheck; // 벽 체크 위치
    [SerializeField] protected float wallCheckDistance = 0.1f; // 벽 체크 거리
    [SerializeField] protected LayerMask groundLayer; // 바닥 레이어

    public float FacingDir { get; private set; } = 1f;
    public bool IsFacingRight { get; protected set; } = true;

    protected virtual void Awake()
    {
        // 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 땅체크 설정
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                Debug.LogError("GroundCheck transform not found!");
            }
        }
    }

    protected virtual void Update()
    {
        
    }

    // 피격 관련 메서드
    public virtual void DamageImpact()
    {

    }

    #region 충돌
    public virtual bool IsGroundDetected() => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDir, wallCheckDistance, groundLayer);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckRadius));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    #endregion

    #region 플립
    public virtual void Flip()
    {
        FacingDir = -FacingDir;
        IsFacingRight = !IsFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public virtual void FlipController(float _xVelocity)
    {
        if ((_xVelocity > 0 && !IsFacingRight) || (_xVelocity < 0 && IsFacingRight))
        {
            Flip();
        }
    }
    #endregion

    #region 속력
    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    // 사망 관련 메서드
    public virtual void Die()
    {

    }
}
