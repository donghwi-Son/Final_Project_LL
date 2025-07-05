using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public enum ProjectileType
{
    Normal,
    AAA,
    BBB,
    CCC
}

public enum ProjectileEffectType
{
    None,
    Homing,
    Explosive,
    Piercing
}

public interface IProjectileEffect
{

    void UpdateEffect(Projectile projectile);

    void OnHit(Projectile projectile, GameObject gameObject);

    void OnDestroy(Projectile projectile);
}

public class Projectile : MonoBehaviour
{

    public ProjectileData projectileData;
    public event Action<Projectile> OnProjectiledestroyed;
    float finalDamage;
    float finalSpeed;
    float finalLifeTime;
    float piercingCount;


    [Header("컴포넌트")]
    private Rigidbody2D rb;
    private List<IProjectileEffect> effects = new List<IProjectileEffect>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        finalLifeTime -= Time.deltaTime;
        if(rb.linearVelocity.magnitude > projectileData.speed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * projectileData.speed;
        }
        // 모든 효과의 업데이트 처리 실행
        foreach (var effect in effects)
        {
            effect.UpdateEffect(this);
        }

        if (finalLifeTime <= 0f)
        {
            DestroyProjectile();
        }
    }


    void CalculateFinalStat(float statdmg, float statlf, float speed)
    {
        finalDamage = projectileData.damageMultiplier * statdmg;
        finalLifeTime = statlf;
    }

    public void Fire(Vector2 pos, Vector2 dir, float statdmg, float statlf, float speed)
    {
        CalculateFinalStat(statdmg, statlf, speed);
        transform.position = pos;
        transform.up = dir;
        gameObject.SetActive(true);
        rb.linearVelocity = dir.normalized * projectileData.speed;
    }

    // 효과 관리 메소드들
    public void AddEffect(IProjectileEffect effect)
    {
        if (!effects.Contains(effect))
        {
            effects.Add(effect);
        }
    }

    public void RemoveEffect(IProjectileEffect effect)
    {
        if (effects.Contains(effect))
        {
            effects.Remove(effect);
        }
    }
    public void SetPiercingCount(int val)
    {
        piercingCount = val;
    }

    // 관통 횟수 감소
    public void DecreasePiercingCount()
    {
        piercingCount--;
    }

    public void DestroyProjectile()
    {
        OnProjectiledestroyed?.Invoke(this);
    }

    // 충돌 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 기본 데미지 적용

            // 모든 효과의 충돌 처리 실행
            foreach (var effect in effects)
            {
                effect.OnHit(this, other.gameObject);
            }

            // 관통이 아니라면 파괴
            if (piercingCount <= 0)
            {
                DestroyProjectile();
            }
        }
    }
}