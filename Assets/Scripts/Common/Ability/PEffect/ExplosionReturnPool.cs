using UnityEngine;

public class ExplosionReturnPool : MonoBehaviour
{
    public void ReturnToPool()
    {
        EffectPool.Instance.ReturnExplosiveEffect(gameObject);
    }
}
