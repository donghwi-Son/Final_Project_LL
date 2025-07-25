using System.Collections;
using UnityEngine;

public class IceEffect : ICommonEffect
{
    public void OnDestroy(Projectile projectile = null)
    {
    }

    public void OnHit(GameObject gameObject, float dmg)
    {
        SpecialAttackManager.Instance.SpawnIce(gameObject);
    }

    public void UpdateEffect(Projectile projectile = null)
    {
    }

}
