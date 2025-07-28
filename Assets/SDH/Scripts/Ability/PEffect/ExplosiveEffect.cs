using UnityEngine;

public class ExplosiveEffect : IProjectileEffect
{
    public void UpdateEffect(Projectile projectile) { }

    public void OnHit(Projectile projectile, GameObject target)
    {
        SpecialAttackManager.Instance.SpawnExplosive(target);
    }

    public void OnDestroy(Projectile projectile) { }
}
