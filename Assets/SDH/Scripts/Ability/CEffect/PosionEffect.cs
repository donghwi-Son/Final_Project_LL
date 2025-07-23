using UnityEngine;

public class PosionEffect : ICommonEffect
{
    public void OnDestroy(Projectile projectile = null)
    {
    }

    public void OnHit(GameObject gameObject, Projectile projectile = null)
    {
        Debug.Log("상대 중독");
    }

    public void UpdateEffect(Projectile projectile = null)
    {
        if (projectile == null) return;
        
    }
}
