using UnityEngine;

public abstract class BossState
{
    protected BossBase boss;
    protected BossStateMachine stateMachine;
    protected string animBoolName;
    protected bool triggerCalled;
    protected float stateTimer;

    public BossState(BossBase boss, BossStateMachine stateMachine, string animBoolName)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter()
    {
        boss.anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        boss.anim.SetBool(animBoolName, false);
    }
}

public class BossStateMachine
{
    public BossState currentState { get; private set; }

    public void Initalize(BossState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(BossState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
}
