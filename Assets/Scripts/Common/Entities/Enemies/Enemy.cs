using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("이동 정보")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;
    [SerializeField] public float fallSpeed = 2f;

    private Transform playerTransform;
    [Header("플레이어 탐지 정보")]
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckRadius;
    public float PlayerCheckRadius { get => playerCheckRadius; set => playerCheckRadius = value; }

    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected float playerCheckDistance;
    public float PlayerCheckDistance { get => playerCheckDistance; set => playerCheckDistance = value; }

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

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void DamageImpact()
    {
        base.DamageImpact();
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

    //public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected()
    => Physics2D.CircleCast(playerCheck.position, playerCheckRadius, Vector2.down * FacingDir, PlayerCheckDistance, whatIsPlayer);

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
