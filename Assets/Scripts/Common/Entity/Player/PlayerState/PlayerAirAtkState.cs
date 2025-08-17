using UnityEngine;

public class PlayerAirAtkState : State<PlayerController>
{
    public PlayerAirAtkState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.CanDoubleJump = false;
        owner.CanAirAttack = false;
        owner.AttackManager.AirAttack(1.5f);
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            stateMachine.ChangeState(owner.FallState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
