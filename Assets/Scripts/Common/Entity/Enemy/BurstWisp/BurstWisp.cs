using UnityEngine;

public class BurstWisp : Enemy
{
    public StateMachine<BurstWisp> StateMachine { get; private set; }
    public BurstWispIdleState IdleState { get; private set; }
    public BurstWispMoveState MoveState { get; private set; }
    public BurstWispCombatState CombatState { get; private set; }
    public BurstWispAttackState AttackState { get; private set; }
    public BurstWispDeadState DeadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new StateMachine<BurstWisp>();
        IdleState = new BurstWispIdleState(this, StateMachine, "Idle");
        MoveState = new BurstWispMoveState(this, StateMachine, "Idle");
        CombatState = new BurstWispCombatState(this, StateMachine, "Idle");
        AttackState = new BurstWispAttackState(this, StateMachine, "Attack");
        DeadState = new BurstWispDeadState(this, StateMachine, "Dead");
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
        StateMachine.ChangeState(AttackState);
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        StateMachine.CurrentState.AnimationFinishTrigger();
    }
}
