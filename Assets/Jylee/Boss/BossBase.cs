using UnityEngine;

public abstract class BossBase : Entity
{
    public BossStateMachine stateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new BossStateMachine();
    }

    private void Start()
    {

    }
}
