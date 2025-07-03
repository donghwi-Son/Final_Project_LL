using UnityEngine;

public class PiercingEffect : IProjectileEffect
{
    private int piercingCount = 3;
    public void UpdateEffect(Projectile projectile)
    {
        // 관통은 업데이트에서 특별한 처리 없음
    }

    public void OnHit(Projectile projectile, GameObject target)
    {
        // 관통 횟수 감소
        projectile.DecreasePiercingCount();
    }

    public void OnDestroy(Projectile projectile) { }
}
