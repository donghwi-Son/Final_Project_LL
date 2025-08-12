using UnityEngine;

public class TaoistStats : EnemyStats
{
    private BossTaoist taoist;
    private bool specialTrigger;
    protected override void Start()
    {
        base.Start();
        taoist = GetComponent<BossTaoist>();

        specialTrigger = true;
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        if(maxHealth.GetValue() / 2 >= currentHealth && specialTrigger)
        {
            specialTrigger = false;
            taoist.BossSpecialTrigger();
            Debug.Log("AAA");
        }
    }

    protected override void Die()
    {
        base.Die();
    }
}
