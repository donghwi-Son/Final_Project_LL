using UnityEngine;

public class TaoistStats : EnemyStats
{
    private BossTaoist taoist;
    protected override void Start()
    {
        base.Start();
        taoist = GetComponent<BossTaoist>();

    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
    }
}
