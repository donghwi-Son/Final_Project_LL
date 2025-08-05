using UnityEngine;

public class PlayerSpecialAttackState : State<PlayerController>
{
    public PlayerSpecialAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        owner.lastSpecialAttackTime = Time.time;
        switch (owner.attackMode)
        {
            case AttackMode.Melee:
                owner.anim.SetTrigger("SpecialAtt");
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
    }

    public override void Exit()
    {
    }
}
