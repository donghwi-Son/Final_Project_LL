using UnityEngine;

public class BossTaoistBeamAttack : State<BossTaoist>
{
    private int stateType;
    private float preheatTime;
    private float lockOnTime;
    private float duration;
    public BossTaoistBeamAttack(BossTaoist owner, StateMachine<BossTaoist> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateType = 1;
        preheatTime = owner.beamPreheatTime;
        lockOnTime = owner.beamLockOnTime;
        duration = owner.beamDuration;
        owner.BossBeamTrajectorySwitch(true);
    }

    public override void Execute()
    {
        base.Execute();
        if(stateType == 1)
        {
            preheatTime -= Time.deltaTime;
            owner.BossBeamTrajectoryUpdate();
            owner.BossFlip();
            if (preheatTime <= 0)
            {
                stateType = 2;
            }
        }
        else if(stateType == 2)
        {
            lockOnTime -= Time.deltaTime;
            if(lockOnTime <= 0)
            {
                stateType = 3;
                owner.BossBeamFire();
            }
        }
        else if(stateType == 3)
        {
            duration -= Time.deltaTime;
            if(duration <= 0)
            {
                stateType = 4;
                owner.anim.SetTrigger("IsBeamEnd");
            }
        }
        else if(stateType == 4)
        {
            if (triggerCalled)
            {
                stateMachine.ChangeState(owner.moveState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.BossBeamTrajectorySwitch(false);
    }
}
