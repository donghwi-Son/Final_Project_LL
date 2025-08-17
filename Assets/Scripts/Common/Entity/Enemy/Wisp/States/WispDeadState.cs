using UnityEngine;

public class WispDeadState : State<Wisp>
{
    public WispDeadState(Wisp owner, StateMachine<Wisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.Instance.PlaySFX(SFX.WispDeath);
    }

    public override void Execute()
    {
        base.Execute();

        owner.SetZeroVelocity();

        if (triggerCalled)
        {
            GameObject.Destroy(owner.gameObject);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
