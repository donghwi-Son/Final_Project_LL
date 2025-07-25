using UnityEngine;

public abstract class EnemyState<T> where T : EnemyBase
{
    protected T enemy;
    protected EnemyStateMachine<T> stateMachine;
    protected string animBoolName;
    protected bool triggerCalled;
    protected float stateTimer;
    protected int stateType;

    public EnemyState(T enemy, EnemyStateMachine<T> stateMachine, string animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter()
    {
        enemy.anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        enemy.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}

public class EnemyStateMachine<T> where T : EnemyBase
{
    public EnemyState<T> currentState { get; private set; }

    public void Initalize(EnemyState<T> state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(EnemyState<T> state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
}
