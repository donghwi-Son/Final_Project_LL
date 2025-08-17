using UnityEngine;

public class BossCrowDeath : State<BossCrow>
{
    private int stateType;

    public BossCrowDeath(BossCrow owner, StateMachine<BossCrow> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = owner.dyingTime;
        stateType = 1;
    }

    public override void Execute()
    {
        base.Execute();

        if (stateType == 1)
        {
            if (stateTimer <= 0)
            {
                stateType = 2;
                owner.anim.SetBool("IsDying", true);
                owner.anim.SetBool("IsIdle", false);
                owner.rb.gravityScale = 1f;
                owner.rb.linearVelocity = new Vector2(owner.IsFacingRight ? owner.dieForceX : -owner.dieForceX, owner.dieForceY);

                stateTimer = 0.2f;
            }
        }
        else if (stateType == 2)
        {
            if (owner.IsGroundDetected() && stateTimer <= 0)
            {
                owner.SetZeroVelocity();
                owner.rb.bodyType = RigidbodyType2D.Kinematic;
                owner.anim.SetBool("IsDeath", true);
                owner.anim.SetBool("IsDying", false);
                stateType = 3;

                stateTimer = 2f;
            }
        }
        else if(stateType == 3)
        {
            if (stateTimer <= 0)
            {
                GameObject.Destroy(owner.gameObject);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
