using UnityEngine;

public class Wisp : Enemy
{
    public GameObject Projectile { get; private set; }

    public StateMachine<Wisp> StateMachine { get; private set; }
    public WispIdleState IdleState { get; private set; }
    public WispMoveState MoveState { get; private set; }
    public WispCombatState CombatState { get; private set; }
    public WispAttackState AttackState { get; private set; }
    public WispDeadState DeadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Projectile = Resources.Load<GameObject>("Projectile/Wisp_Fireball");

        StateMachine = new StateMachine<Wisp>();
        IdleState = new WispIdleState(this, StateMachine, "Idle");
        MoveState = new WispMoveState(this, StateMachine, "Idle");
        CombatState = new WispCombatState(this, StateMachine, "Idle");
        AttackState = new WispAttackState(this, StateMachine, "Attack");
        DeadState = new WispDeadState(this, StateMachine, "Dead");
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.ChangeState(IdleState);
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.Execute();
    }

    public override void Die()
    {
        base.Die();
        StateMachine.ChangeState(DeadState);
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        StateMachine.CurrentState.AnimationFinishTrigger();
    }
}
