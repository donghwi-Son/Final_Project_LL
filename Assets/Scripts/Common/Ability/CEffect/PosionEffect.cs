using UnityEngine;

public class PosionEffect : ICommonEffect
{
    public void OnDestroy(Projectile projectile = null)
    {
    }

    public void OnHit(GameObject gameObject, float dmg)
    {
        SpecialAttackManager.Instance.SpawnPoison(gameObject, dmg);
    }

    public void UpdateEffect(Projectile projectile = null)
    {
        if (projectile == null) return;
        
    }
}
