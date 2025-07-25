using UnityEngine;

public class EnemyBabyCrowState : EnemyState<EnemyBabyCrow>
{
    public EnemyBabyCrowState(EnemyBabyCrow enemy, EnemyStateMachine<EnemyBabyCrow> stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
}
