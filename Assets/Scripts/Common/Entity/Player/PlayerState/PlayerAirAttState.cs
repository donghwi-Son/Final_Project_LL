using UnityEngine;

public class PlayerAirAttState : State<PlayerController>
{
    public PlayerAirAttState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player Air Attack State Entered");
        owner.CanDoubleJump = false;
        owner.CanAirAttack = false;
        owner.AttackManager.AirAttack();
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            stateMachine.ChangeState(owner.FallingState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
