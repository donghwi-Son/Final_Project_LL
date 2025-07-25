using System.Reflection;
using UnityEngine;

public class BossCrow : EnemyBase
{
    [Header("공격 관련")]
    public int nextAttackType;
    [SerializeField] private int attackSelect;

    [Header("돌진 공격 관련")]
    public float dashAttackSpeed;

    [Header("원거리 공격 관련")]
    [SerializeField] private GameObject rangeAttackObj;
    [SerializeField] private int rangeAttackQty;


    [Header("추적 관련")]
    public float moveSpeed;
    public float baseHeight;
    public float moveRadius;
    public float changeTargetTime;
    public float turnCooldown; // 방향전환 쿨타임
    public float turnLerpSpeed; // 방향 부드럽게 전환
    public float playerYLimit;
    private float lastDirX;
    private Vector2 moveDir;

    [Header("기타")]
    [SerializeField] public float detectionRange;
    [SerializeField] private LayerMask playerLayer;
    public float attackDealy;


    private Collider2D detectionCol;
    public Transform playerTransform;

    private Vector2 targetPos;
    private float timer;
    private float turnTimer;


    // 머신
    public EnemyStateMachine<BossCrow> stateMachine { get; private set; }
    public BossCrowStand standState { get; private set; }
    public BossCrowIdle idleState { get; private set; }
    public BossCrowRangeAttack rangeAttack { get; private set; }
    public BossCrowStrikeAttack strikeAttack { get; private set; }
    public BossCrowDeath deathState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine<BossCrow>();
        standState = new BossCrowStand(this, stateMachine, "IsStand");
        idleState = new BossCrowIdle(this, stateMachine, "IsIdle");
        rangeAttack = new BossCrowRangeAttack(this, stateMachine, "IsRange");
        strikeAttack = new BossCrowStrikeAttack(this, stateMachine, "IsStrike");
        deathState = new BossCrowDeath(this, stateMachine, "IsDeath");
    }

    protected override void Start()
    {
        stateMachine.Initalize(standState);
        nextAttackType = 0;

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public void DetectPlayer()
    {
        detectionCol = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (detectionCol != null)
        {
            // 다음 동작
            stateMachine.ChangeState(idleState);
        }
    }

    public void ChasePlayer()
    {
        if (playerTransform == null) return;

        timer += Time.deltaTime;
        turnTimer += Time.deltaTime;

        if (timer >= changeTargetTime)
        {
            PickNewTarget();
            timer = 0f;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector2 currentPos = transform.position;
        Vector2 desiredDir = (targetPos - currentPos).normalized;

        moveDir = Vector2.Lerp(moveDir, desiredDir, Time.deltaTime * turnLerpSpeed).normalized;

        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        // 방향 전환 체크
        float dirX = Mathf.Sign(moveDir.x);
        if (dirX != lastDirX && turnTimer >= turnCooldown)
        {
            lastDirX = dirX;
            turnTimer = 0f;

            Flip();
        }

        // 플레이어 안닿게 하기
        float minY = playerTransform.position.y + playerYLimit;
        if (transform.position.y < minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }
    }

    private void PickNewTarget()
    {
        Vector2 playerPos = playerTransform.position;

        float targetX = playerPos.x + Random.Range(-moveRadius, moveRadius);

        float targetY = playerPos.y + baseHeight + Random.Range(0f, 1f); // 항상 위쪽으로만 타겟팅

        targetPos = new Vector2(targetX, targetY);
    }

    public void nextAttackSelect()
    {
        nextAttackType = Random.Range(1, attackSelect+1);

        switch (nextAttackType)
        {
            case 1:
                stateMachine.ChangeState(rangeAttack);
                break;
            case 2:
                stateMachine.ChangeState(strikeAttack);
                break;
        }
    }

    public void BossFlip(bool reverse)
    {
        float gazePos = transform.position.x > playerTransform.position.x ? -1 : 1;
        gazePos = reverse ? gazePos * -1 : gazePos;

        FlipController(gazePos);
    }

    public float BossPlayerGaze()
    {
        Vector2 direction = (new Vector3(playerTransform.position.x, playerTransform.position.y) - transform.position).normalized; // 플레이어 방향 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 각도로 변환

        return angle;
    }

    public void BossRangeAttack()
    {
        Vector2 fireDirection = (playerTransform.position - transform.position).normalized;
        float laserAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(rangeAttackObj, transform.position, Quaternion.Euler(0, 0, laserAngle-90)); // 보정각도 추가
        projectile.GetComponent<PlayerTargetRangeAttack>().SetDirection(fireDirection);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // 감 지거리
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
