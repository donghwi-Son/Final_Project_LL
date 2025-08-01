using UnityEngine;

public class Wisp : Enemy
{
    public StateMachine<Wisp> StateMachine { get; private set; }
    public WispIdleState IdleState { get; private set; }
    public WispMoveState MoveState { get; private set; }
    public WispAttackState AttackState { get; private set; }
    public WispDeadState DeadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new StateMachine<Wisp>();
        IdleState = new WispIdleState(this, StateMachine, "Idle");
        MoveState = new WispMoveState(this, StateMachine, "Move");
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
