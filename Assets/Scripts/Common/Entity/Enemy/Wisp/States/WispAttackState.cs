using UnityEngine;

public class WispAttackState : State<Wisp>
{
    private Transform player;

    public WispAttackState(Wisp owner, StateMachine<Wisp> stateMachine, string animBoolName) : base(owner, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player = PlayerManager.Instance.player.transform;
        
        owner.SetZeroVelocity();

        Shoot();
    }

    public override void Execute()
    {
        base.Execute();

        if (triggerCalled)
        {
            stateMachine.ChangeState(owner.MoveState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.lastTimeAttacked = Time.time;
    }

    private void Shoot()
    {
        //미사일 생성
        GameObject missile = GameObject.Instantiate(owner.Projectile, owner.transform.position, Quaternion.identity);

        //플레이어 방향으로 발사 방향 설정
        Vector2 direction = player.position - owner.transform.position;
        missile.GetComponent<EnemyProjectile>().Initialize(direction, owner.stats.damage.GetValue());
    }
}
