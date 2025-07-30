using UnityEngine;

public class BabyCrowStats : CharacterStats
{
    private EnemyBabyCrow crow;
    protected override void Start()
    {
        base.Start();
        crow = GetComponent<EnemyBabyCrow>();

    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        // 이펙트 추가
        Destroy(gameObject);
    }
}
