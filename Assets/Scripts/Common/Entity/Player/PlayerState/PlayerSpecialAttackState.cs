using UnityEngine;

public class PlayerSpecialAttackState : State<PlayerController>
{
    public PlayerSpecialAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.lastSpecialAttackTime = Time.time;
        switch (owner.attackMode)
        {
            case AttackMode.Melee:
                owner.AttackManager.SpecialMeleeAttack();
                break;
            case AttackMode.Ranged:
                owner.AttackManager.SpecialRangedAttack();
                stateMachine.ChangeState(owner.IdleState);
                break;
        }
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            stateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
