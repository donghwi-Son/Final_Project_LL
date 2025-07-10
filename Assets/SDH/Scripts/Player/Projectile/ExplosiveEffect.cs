using UnityEngine;

public class ExplosiveEffect : IProjectileEffect
{
    private float explosionRadius = 3f;
    public void UpdateEffect(Projectile projectile) { }

    public void OnHit(Projectile projectile, GameObject target)
    {
        // 폭발 효과
        Collider2D[] colliders = Physics2D.OverlapCircleAll(projectile.transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
            Debug.Log("폭발!");
        }

        // 폭발 이펙트 생성

    }

    public void OnDestroy(Projectile projectile) { }
}
