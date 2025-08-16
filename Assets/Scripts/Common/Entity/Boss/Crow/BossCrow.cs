using System.Reflection;
using System.Xml;
using UnityEngine;

public class BossCrow : Enemy
{
    [Header("공격 관련")]
    public int nextAttackType;
    [SerializeField] private int attackSelect;

    [Header("돌진 공격 관련")]
    public float dashAttackSpeed;
    public float dashReboundSpeed;
    [SerializeField] private Collider2D strikeCol;

    [Header("원거리 공격 관련")]
    [SerializeField] private GameObject rangeAttackObj;
    [SerializeField] private int rangeAttackQty;

    [Header("소환 공격 관련")]
    [SerializeField] private GameObject spawnEnemyObj;
    [SerializeField] private int spawnEnemyQty;
    [SerializeField] private float spawnSpaceX;
    [SerializeField] private float spawnSpaceY;

    [Header("추적 관련")]
    public float baseHeight;
    public float moveRadius;
    public float changeTargetTime;
    public float turnCooldown; // 방향전환 쿨타임
    public float turnLerpSpeed; // 방향 부드럽게 전환
    public float playerYLimit;
    private float lastDirX;
    private Vector2 moveDir;
    public float dectedDistance;

    [Header("죽음 관련")]
    public float dyingTime;
    public float dieForceX;
    public float dieForceY;

    [Header("기타")]
    public float attackDealy;

    public Transform playerTrans;

    private Vector2 targetPos;
    private float timer;
    private float turnTimer;


    // 머신
    public StateMachine<BossCrow> stateMachine { get; private set; }
    public BossCrowStand standState { get; private set; }
    public BossCrowIdle idleState { get; private set; }
    public BossCrowRangeAttack rangeAttack { get; private set; }
    public BossCrowStrikeAttack strikeAttack { get; private set; }
    public BossCrowSpawnEnemy spawnEnemyState { get; private set; }
    public BossCrowDeath deathState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new StateMachine<BossCrow>();
        standState = new BossCrowStand(this, stateMachine, "IsStand");
        idleState = new BossCrowIdle(this, stateMachine, "IsIdle");
        rangeAttack = new BossCrowRangeAttack(this, stateMachine, "IsRange");
        strikeAttack = new BossCrowStrikeAttack(this, stateMachine, "IsStrike");
        spawnEnemyState = new BossCrowSpawnEnemy(this, stateMachine, "IsIdle");
        deathState = new BossCrowDeath(this, stateMachine, "IsIdle");
    }

    protected override void Start()
    {
        stateMachine.ChangeState(standState);
        nextAttackType = 0;

        if (playerTrans == null)
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.CurrentState.Execute();

        if (Input.GetKeyDown(KeyCode.R))
        {
            stats.TakeDamage(10);
        }
    }

    public void AnimationTrigger()
    {
        stateMachine.CurrentState.AnimationFinishTrigger();
    }

    public override bool IsPlayerDetected()
    => Physics2D.CircleCast(playerCheck.position, playerCheckRadius, Vector2.down * FacingDir, dectedDistance, whatIsPlayer);

    public void DetectPlayer()
    {
        if (IsPlayerDetected())
        {
            // 다음 동작
            stateMachine.ChangeState(idleState);
        }
    }

    public void ChasePlayer()
    {
        if (playerTrans == null) return;

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

        transform.position += (Vector3)(moveDir * MoveSpeed * Time.deltaTime);

        // 방향 전환 체크
        float dirX = Mathf.Sign(moveDir.x);
        if (dirX != lastDirX && turnTimer >= turnCooldown)
        {
            lastDirX = dirX;
            turnTimer = 0f;

            Flip();
        }

        // 플레이어 안닿게 하기
        float minY = playerTrans.position.y + playerYLimit;
        if (transform.position.y < minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }
    }

    private void PickNewTarget()
    {
        Vector2 playerPos = playerTrans.position;

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
            case 3:
                stateMachine.ChangeState(spawnEnemyState);
                break;
        }
    }

    public void BossFlip(bool reverse)
    {
        float gazePos = transform.position.x > playerTrans.position.x ? -1 : 1;
        gazePos = reverse ? gazePos * -1 : gazePos;

        FlipController(gazePos);
    }

    public float BossPlayerGaze()
    {
        Vector2 direction = (new Vector3(playerTrans.position.x, playerTrans.position.y) - transform.position).normalized; // 플레이어 방향 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 각도로 변환

        return angle;
    }

    public void BossRangeAttack()
    {
        Vector2 fireDirection = (playerTrans.position - transform.position).normalized;
        float laserAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(rangeAttackObj, transform.position, Quaternion.Euler(0, 0, laserAngle-90)); // 보정각도 추가
        projectile.GetComponent<ProjectileBase>().SetParentsStats(stats);
        projectile.GetComponent<ProjectileBase>().SetDirection(fireDirection);
    }

    public void BossSpawnEnemy()
    {
        for(int i = 0; i < spawnEnemyQty; i++)
        {
            float xPos = Random.Range(0, spawnSpaceX);
            float yPos = Random.Range(0, spawnSpaceY);

            Instantiate(spawnEnemyObj, new Vector3(transform.position.x + xPos, transform.position.y + yPos), Quaternion.identity);
        }
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deathState);
    }

    // 콜라이더
    public void StrikeColliderSwitch(bool value)
    {
        strikeCol.enabled = value;
    }
}
