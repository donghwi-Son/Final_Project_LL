using UnityEngine;

public class ExplosiveEffect : IProjectileEffect
{
    public void UpdateEffect(Projectile projectile) { }

    public void OnHit(Projectile projectile, GameObject target)
    {
        Debug.Log("폭발!");
    }

    public void OnDestroy(Projectile projectile) { }
}
