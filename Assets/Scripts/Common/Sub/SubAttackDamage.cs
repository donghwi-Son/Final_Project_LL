using UnityEngine;

public class SubAttackDamage : MonoBehaviour
{
    private int damage;
    private float lifeTime;
    private LayerMask enemyLayer;

    public void Init(int dmg, float duration, LayerMask layer)
    {
        damage = dmg;
        lifeTime = duration;
        enemyLayer = layer;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                //enemy.TakeDamage(damage);     //몬스터 피해
            }
        }
    }
}
