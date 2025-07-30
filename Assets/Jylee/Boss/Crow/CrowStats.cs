using UnityEngine;

public class CrowStats : CharacterStats
{
    private BossCrow crow;
    protected override void Start()
    {
        base.Start();
        crow = GetComponent<BossCrow>();

    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        crow.BossDefeat();
    }

}
