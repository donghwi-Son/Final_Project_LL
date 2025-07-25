using UnityEngine;

public class State<T> where T : Entity
{
    protected T owner;
    protected StateMachine<T> stateMachine;
    protected bool triggerCalled;
    public string animBoolName { get; private set; }

    protected float stateTimer;

    public State(T owner, StateMachine<T> stateMachine, string animBoolName)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        // Called when the state is entered
        owner.anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Execute()
    {
        // Called every frame while in the state
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        // Called when the state is exited
        owner.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
