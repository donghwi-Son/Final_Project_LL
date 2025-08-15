using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BossTaoist : Enemy
{
    [Header("공격 관련")]
    [SerializeField] private float attackRangeNear;
    public float attackCooldownTimer;
    public int nextAttackType;
    [SerializeField] private int attackSelect;
    private bool isSpecial;

    [Header("대쉬 공격 관련")]
    public float dashPower;
    [SerializeField] private Collider2D dashAttCol;

    [Header("영거리 공격 관련")]
    [SerializeField] private Collider2D meleeAttCol;

    [Header("원거리 공격 관련")]
    [SerializeField] private Transform rangeAttackPoint;
    [SerializeField] private GameObject rangeAttackObj;
    public int rangeAttackQty;
    public float rangeAttackDelay;

    [Header("빔 공격 관련")]
    [SerializeField] private Transform beamAttackPoint;
    [SerializeField] private GameObject beamAttackObj;
    public float beamPreheatTime;
    public float beamLockOnTime;
    public float beamDuration;
    private LineRenderer lineRenderer;

    [Header("특수 공격 관련")]
    [SerializeField] private GameObject sAttackObj;
    public float singleAttackDelay;
    public float doubleAttackDelay;
    public float finalDelay;
    public int singleAttackCount;
    public int doubleAttackCount;

    [Header("추적 관련")]
    public float moveDistance;
    public float moveDuration;
    public float dectedDistance;

    public Transform playerTrans;

    public StateMachine<BossTaoist> stateMachine { get; private set; }
    public BossTaoistIdle idleState { get; private set; }
    public BossTaoistMove moveState { get; private set; }
    public BossTaoistDashAttack dashAttack { get; private set; }
    public BossTaoistMeleeAttack meleeAttack { get; private set; }
    public BossTaoistRangeAttack rangeAttack { get; private set; }
    public BossTaoistBeamAttack beamAttack { get; private set; }
    public BossTaoistDeath deathState { get; private set; }
    public BossTaoistSpecialAttack specialAttack { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new StateMachine<BossTaoist>();
        idleState = new BossTaoistIdle(this, stateMachine, "IsIdle");
        moveState = new BossTaoistMove(this, stateMachine, "IsMove");
        dashAttack = new BossTaoistDashAttack(this, stateMachine, "IsDashAttack");
        meleeAttack = new BossTaoistMeleeAttack(this, stateMachine, "IsMeleeAttack");
        rangeAttack = new BossTaoistRangeAttack(this, stateMachine, "IsRangeAttack");
        beamAttack = new BossTaoistBeamAttack(this, stateMachine, "IsBeamAttack");
        deathState = new BossTaoistDeath(this, stateMachine, "IsDeath");
        specialAttack = new BossTaoistSpecialAttack(this, stateMachine, "IsPhaseChange");
    }

    protected override void Start()
    {
        stateMachine.ChangeState(idleState);

        lineRenderer = GetComponent<LineRenderer>();

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
        if (Input.GetKeyDown(KeyCode.U))
        {
            BossSpecialTrigger();
        }
    }

    public override bool IsPlayerDetected()
    => Physics2D.CircleCast(playerCheck.position, playerCheckRadius, Vector2.down * FacingDir, dectedDistance, whatIsPlayer);

    public void DetectPlayer()
    {
        if (IsPlayerDetected())
        {
            // 다음 동작
            stateMachine.ChangeState(moveState);
        }
    }

    public bool IsPlayerInAttackRange()
    => Physics2D.OverlapCircle(transform.position, attackCheckRadius, whatIsPlayer);

    public bool IsPlayerInAttackRangeNear()
    => Physics2D.OverlapCircle(transform.position, attackRangeNear, whatIsPlayer);

    public void ChasePlayer()
    {
        if (playerTrans == null) return;

        BossFlip(false);
        StartCoroutine(MoveStep());
    }

    private IEnumerator MoveStep()
    {
        Vector2 start = transform.position;
        Vector2 direction = (playerTrans.position - (Vector3)start).normalized;
        direction.y = 0f;
        direction.Normalize();

        Vector2 end = start + direction * moveDistance;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;
            transform.position = Vector2.Lerp(transform.position, end, t);
            yield return null;
        }

        transform.position = end;
    }

    public void NextAttackSelect()
    {
        if (isSpecial)
        {
            isSpecial = false;
            stateMachine.ChangeState(specialAttack);
            return;
        }

        while (true)
        {
            nextAttackType = Random.Range(1, attackSelect + 1);

            if (IsPlayerInAttackRangeNear()&& nextAttackType == 4)
            {
                continue;
            }
            else if(!IsPlayerInAttackRangeNear() && nextAttackType == 2)
            {
                continue;
            }

            break;
        }

        switch (nextAttackType)
        {
            case 1:
                stateMachine.ChangeState(dashAttack);
                break;
            case 2:
                stateMachine.ChangeState(meleeAttack);
                break;
            case 3:
                stateMachine.ChangeState(rangeAttack);
                break;
            case 4:
                stateMachine.ChangeState(beamAttack);
                break;
        }
    }

    public void AnimationTrigger()
    {
        stateMachine.CurrentState.AnimationFinishTrigger();
    }

    public void BossFlip(bool reverse = false)
    {
        float gazePos = transform.position.x > playerTrans.position.x ? -1 : 1;
        gazePos = reverse ? gazePos * -1 : gazePos;

        FlipController(gazePos);
    }

    public void BossRangeAttack()
    {
        // 플레이어 피벗이 하단이라 y값 보정
        Vector3 playerDir = new Vector3(playerTrans.position.x, playerTrans.position.y + 1, playerTrans.position.z);
        Vector2 fireDirection = (playerDir - rangeAttackPoint.position).normalized;
        float laserAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(rangeAttackObj, rangeAttackPoint.position, Quaternion.Euler(0, 0, laserAngle - 90)); // 보정각도 추가
        projectile.GetComponent<ProjectileBase>().SetParentsStats(stats);
        projectile.GetComponent<ProjectileBase>().SetDirection(fireDirection);
    }

    public void BossBeamTrajectorySwitch(bool value)
    {
        lineRenderer.enabled = value;
    }

    public void BossBeamTrajectoryUpdate()
    {
        // 플레이어 피벗이 하단이라 y값 보정
        Vector3 playerDir = new Vector3(playerTrans.position.x, playerTrans.position.y + 1, playerTrans.position.z);
        Vector2 fireDirection = (playerDir - beamAttackPoint.position).normalized;
        float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        Vector2 newDirection = Quaternion.Euler(0, 0, baseAngle) * Vector2.right;
        Vector2 endPosition = (Vector2)beamAttackPoint.position + newDirection * 20f;

        lineRenderer.SetPosition(0, beamAttackPoint.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    public void BossBeamFire()
    {
        Vector3 startPos = lineRenderer.GetPosition(0);
        Vector3 endPos = lineRenderer.GetPosition(1);

        Vector2 fireDirection = (endPos - startPos).normalized;

        float laserAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        GameObject beam = Instantiate(beamAttackObj, beamAttackPoint.position, Quaternion.Euler(0, 0, laserAngle));
        beam.GetComponent<ProjectileBase>().SetParentsStats(stats);
        beam.GetComponent<ProjectileBase>().SetDirection(fireDirection);

        BossBeamTrajectorySwitch(false);
    }

    public void BossSpecialTrigger()
    {
        isSpecial = true;
    }

    public void BossSpecialAttack(float zAngle)
    {
        Vector3 newDir = new Vector3(playerTrans.position.x, playerTrans.position.y + 1, playerTrans.position.z);

        GameObject sAtt = Instantiate(sAttackObj, newDir, Quaternion.Euler(0, 0, zAngle));
        sAtt.GetComponent<TaoistSpecialAttack>().SetParentsStats(stats);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deathState);
    }

    // 콜라이더
    public void DashAttackColliderSwitch(bool value)
    {
        dashAttCol.enabled = value;
    }

    public void MeleeAttackColliderSwitch(bool value)
    {
        meleeAttCol.enabled = value;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        /*Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRangeFar);*/

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRangeNear);
    }
}
