using UnityEngine;

public class WoodSpiritGroundedState : State<WoodSpirit>
{
    private Transform player;

    public WoodSpiritGroundedState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.Instance.player.transform;
    }

    public override void Execute()
    {
        base.Execute();

        if (owner.IsPlayerDetected() || Vector2.Distance(owner.transform.position, player.position) < 2f)
        {
            stateMachine.ChangeState(owner.CombatState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
