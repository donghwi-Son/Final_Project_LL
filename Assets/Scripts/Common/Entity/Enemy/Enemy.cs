using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyStats stats { get; private set; }

    [Header("이동 정보")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("플레이어 탐지 정보")]
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckRadius;
    public float PlayerCheckRadius { get => playerCheckRadius; set => playerCheckRadius = value; }

    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected LayerMask whatIsObstacle;

    [Header("공격 정보")]
    [SerializeField] protected Transform attackCheck;
    public Transform AttackCheck { get => attackCheck; }

    [SerializeField] protected float attackCheckRadius;
    public float AttackCheckRadius { get => attackCheckRadius; }

    [SerializeField] protected float attackDistance;
    public float AttackDistance { get => attackDistance; }

    [SerializeField] protected float attackCooldown;
    public float AttackCooldown { get => attackCooldown; }

    [HideInInspector] public float lastTimeAttacked;

    protected override void Awake()
    {
        base.Awake();   

        stats = GetComponent<EnemyStats>();

        defaultMoveSpeed = moveSpeed;
    }

    public override void DamageImpact()
    {
        base.DamageImpact();
    }

    public bool CanAttack()
    {
        if (Time.time >= lastTimeAttacked + attackCooldown)
        {
            lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    protected virtual IEnumerator FreezeTimerFor(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
    }

    public virtual void AnimationFinishTrigger()
    {
        // 애니메이션 트리거가 끝났을 때 호출되는 메서드
        // 이 메서드는 EnemyAnimationTrigger에서 호출됩니다.
        // 기본 구현은 아무것도 하지 않습니다.
    }

    public virtual bool IsPlayerDetected()
    {
        if (DetectedPlayerCollider != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, DetectedPlayerCollider.transform.position - transform.position, playerCheckRadius, whatIsObstacle);

            if(hit.collider == DetectedPlayerCollider)
            {
                return true;
            }
        }

        return false;
    }

    protected Collider2D DetectedPlayerCollider => Physics2D.OverlapCircle(playerCheck.position, playerCheckRadius, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + AttackDistance * FacingDir, transform.position.y));
        Gizmos.DrawWireSphere(playerCheck.position, playerCheckRadius);
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    public void SetRotationZero()
    {
        anim.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
