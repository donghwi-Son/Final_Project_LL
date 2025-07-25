using UnityEngine;

public class BossCrowState : EnemyState<BossCrow>
{
    public BossCrowState(BossCrow enemy, EnemyStateMachine<BossCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
}
