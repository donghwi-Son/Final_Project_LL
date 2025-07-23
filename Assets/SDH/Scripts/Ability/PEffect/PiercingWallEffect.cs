using UnityEngine;

public class PiercingWallEffect : IProjectileEffect
{
    public void OnDestroy(Projectile projectile)
    {
    }

    public void OnHit(Projectile projectile, GameObject gameObject)
    {
        Debug.Log("벽맞음");
        projectile.EnablePiercingWall();
    }

    public void UpdateEffect(Projectile projectile)
    {
    }
}
