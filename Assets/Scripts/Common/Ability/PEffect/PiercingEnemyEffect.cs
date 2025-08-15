using UnityEngine;

public class PiercingEnemyEffect : IProjectileEffect
{
    public void OnDestroy(Projectile projectile)
    {
    }

    public void OnHit(Projectile projectile, GameObject gameObject)
    {
        projectile.EnablePiercingEnemy();
    }

    public void UpdateEffect(Projectile projectile)
    {
    }
}
