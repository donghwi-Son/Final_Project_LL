using UnityEngine;

public class WoodSpirit : Enemy
{
    
    public WoodSpiritIdleState IdleState { get; private set; }
    public WoodSpiritMoveState MoveState { get; private set; }
    public WoodSpiritAttackState AttackState { get; private set; }
    public WoodSpiritDeadState DeadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
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
        
    }

    public override void Die()
    {
        base.Die();
        StateMachine.ChangeState(DeadState);
    }
}
