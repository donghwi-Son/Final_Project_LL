using UnityEngine;

public class BossTaoistMeleeAttack : State<BossTaoist>
{
    private int stateType;
    public BossTaoistMeleeAttack(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.ChasePlayer();

        stateType = 1;
    }

    public override void Execute()
    {
        base.Execute();

        if(stateType == 1 && triggerCalled)
        {
            stateType = 2;
            triggerCalled = false;
            owner.MeleeAttackColliderSwitch(true);
            AudioManager.Instance.PlaySFX(SFX.SwordSwing);
        }
        else if (stateType == 2 && triggerCalled)
        {
            owner.MeleeAttackColliderSwitch(false);
            stateMachine.ChangeState(owner.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
