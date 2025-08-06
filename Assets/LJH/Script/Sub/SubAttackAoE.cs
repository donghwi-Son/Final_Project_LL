using UnityEngine;

public class SubAttackAoE : SubAttackBase
{
    [Header("AoE")]
    public float radius = 10f;
    public int damage = 10;

    private readonly Collider2D[] hits = new Collider2D[48];

    protected override void Fire()
    {
        int count = Physics2D.OverlapCircleNonAlloc(owner.position, radius, hits, enemyLayer);
        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (col && col.TryGetComponent(out Enemy enemy))
               // enemy.TakeDamage(damage);     //몬스터 데미ㅣ지 
            hits[i] = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var center = Application.isPlaying && owner ? owner.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}