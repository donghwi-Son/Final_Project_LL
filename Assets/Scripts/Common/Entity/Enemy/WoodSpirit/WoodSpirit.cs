using UnityEngine;

public class WoodSpirit : Enemy
{
    public StateMachine<WoodSpirit> stateMachine { get; private set; }
    public WoodSpiritIdleState IdleState { get; private set; }
    public WoodSpiritMoveState MoveState { get; private set; }
    public WoodSpiritAttackState AttackState { get; private set; }
    public WoodSpiritDeadState DeadState { get; private set; }
}
