using UnityEngine;

public class LightningEffect : ICommonEffect
{
    public void OnDestroy(Projectile projectile = null)
    {
    }

    public void OnHit(GameObject gameObject, float dmg)
    {
        SpecialAttackManager.Instance.StartChain(gameObject, dmg);
    }

    public void UpdateEffect(Projectile projectile = null)
    {
    }
}
