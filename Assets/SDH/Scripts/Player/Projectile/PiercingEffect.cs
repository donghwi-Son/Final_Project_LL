using UnityEngine;

public class PiercingEffect : IProjectileEffect
{
    private int piercingCount = 3;
    public void UpdateEffect(Projectile projectile) { }

    public void OnHit(Projectile projectile, GameObject target)
    {
        projectile.DecreasePiercingCount();
    }

    public void OnDestroy(Projectile projectile) { }
}
