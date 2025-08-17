using UnityEngine;

public class WoodSpiritDeadState : State<WoodSpirit>
{
    public WoodSpiritDeadState(WoodSpirit owner, StateMachine<WoodSpirit> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.Instance.PlaySFX(SFX.TreeDeath);
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
