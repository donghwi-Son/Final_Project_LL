using UnityEngine;

public class BossTaoistDashAttack : State<BossTaoist>
{
    private int stateType;
    public BossTaoistDashAttack(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        owner.BossFlip();
        stateType = 1;
    }

    public override void Execute()
    {
        base.Execute();
        if (stateType == 1 && triggerCalled)
        {
            triggerCalled = false;
            stateType = 2;
            owner.BossFlip();
            owner.DashAttackColliderSwitch(true);
            AudioManager.Instance.PlaySFX(SFX.SwordSwing);
        }
        else if(stateType == 2)
        {
            owner.rb.linearVelocity = new Vector2(owner.IsFacingRight ? owner.dashPower : -owner.dashPower, owner.rb.linearVelocityY);
            if (triggerCalled)
            {
                owner.SetZeroVelocity();
                owner.DashAttackColliderSwitch(false);
                triggerCalled = false;
                stateType = 3;
            }
        }
        else if(stateType == 3 && triggerCalled)
        {
            stateMachine.ChangeState(owner.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
