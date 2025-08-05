using UnityEngine;

public class WoodSpirit : Enemy
{
    public StateMachine<WoodSpirit> StateMachine { get; private set; }
    public WoodSpiritIdleState IdleState { get; private set; }
    public WoodSpiritMoveState MoveState { get; private set; }
    public WoodSpiritAttackState AttackState { get; private set; }
    public WoodSpiritDeadState DeadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new StateMachine<WoodSpirit>();
        IdleState = new WoodSpiritIdleState(this, StateMachine, "Idle");
        MoveState = new WoodSpiritMoveState(this, StateMachine, "Move");
        AttackState = new WoodSpiritAttackState(this, StateMachine, "Attack");
        DeadState = new WoodSpiritDeadState(this, StateMachine, "Dead");
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
