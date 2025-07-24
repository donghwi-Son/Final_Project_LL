using UnityEngine;

public class BossCrow : BossBase
{

    public BossCrowStand standState { get; private set; }
    public BossCrowIdle idleState { get; private set; }
    public BossCrowRangeAttack rangeAttack { get; private set; }
    public BossCrowStrikeAttack strikeAttack { get; private set; }
    public BossCrowDeath deathState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        standState = new BossCrowStand(this, stateMachine, "IsStand");
        idleState = new BossCrowIdle(this, stateMachine, "IsIdle");
        rangeAttack = new BossCrowRangeAttack(this, stateMachine, "IsStrike");
        strikeAttack = new BossCrowStrikeAttack(this, stateMachine, "IsRange");
        deathState = new BossCrowDeath(this, stateMachine, "IsDeath");
    }

    void Start()
    {
        stateMachine.Initalize(standState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(idleState);
            Debug.Log(stateMachine);
            stateMachine.ChangeState(idleState);
        }
    }
}
