using UnityEngine;

public class WoodSpirit : Enemy
{
    public StateMachine<WoodSpirit> stateMachine { get; private set; }
    public WoodSpiritIdleState IdleState { get; private set; }
    public WoodSpiritMoveState MoveState { get; private set; }
    public WoodSpiritAttackState AttackState { get; private set; }
    public WoodSpiritDeadState DeadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new StateMachine<WoodSpirit>();
        IdleState = new WoodSpiritIdleState(this, stateMachine, "Idle");
        MoveState = new WoodSpiritMoveState(this, stateMachine, "Move");
        AttackState = new WoodSpiritAttackState(this, stateMachine, "Attack");
        DeadState = new WoodSpiritDeadState(this, stateMachine, "Dead");
    }
}
