using UnityEngine;

public class BossCrowDeath : State<BossCrow>
{
    private float dyingDuration;
    private int stateType;
    public BossCrowDeath(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dyingDuration = owner.dyingTime;
        stateType = 1;
    }

    public override void Execute()
    {
        base.Execute();

        dyingDuration -= Time.deltaTime;
        if (stateType == 1)
        {
            if (dyingDuration <= 0)
            {
                stateType = 2;
                owner.anim.SetBool("IsDying", true);
                owner.anim.SetBool("IsIdle", false);
                owner.rb.gravityScale = 1f;
                owner.rb.linearVelocity = new Vector2(owner.IsFacingRight ? owner.dieForceX : -owner.dieForceX, owner.dieForceY);

                dyingDuration = 0.2f;
            }
        }
        else if (stateType == 2)
        {
            if (owner.IsGroundDetected() && dyingDuration <= 0)
            {
                owner.SetZeroVelocity();
                owner.rb.bodyType = RigidbodyType2D.Kinematic;
                owner.anim.SetBool("IsDeath", true);
                owner.anim.SetBool("IsDying", false);
                stateType = 3;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
