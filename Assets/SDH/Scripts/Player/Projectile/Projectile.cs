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
    Piercing,
    Bouncing
}

public interface IProjectileEffect
{ 

    void UpdateEffect(Projectile projectile);

    void OnHit(Projectile projectile, GameObject gameObject);

    void OnDestroy(Projectile projectile);
}

public class Projectile : MonoBehaviour
{
    [Header("기본 속성")]
    public ProjectileType projectileType;
    public float speed = 10f;
    public float damage = 20f;
    public float lifeTime = 5f;

    [Header("효과 플래그들")]
    private int piercingCount = 0;

    [Header("컴포넌트")]
    private Rigidbody2D rb;
    private List<IProjectileEffect> effects = new List<IProjectileEffect>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // 초기 속도 설정
        rb.linearVelocity = transform.right * speed;

        // 수명 설정
        Destroy(gameObject, lifeTime);

        Debug.Log($"투사체 생성: {projectileType}, 속도: {speed}, 데미지: {damage}");
    }

    void Update()
    {
        // 모든 효과의 업데이트 실행
        foreach (var effect in effects)
        {
            effect.UpdateEffect(this);
        }
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

    // 관통 횟수 감소
    public void DecreasePiercingCount()
    {
        piercingCount--;
    }

    // 투사체 파괴
    public void DestroyProjectile()
    {
        Destroy(gameObject);
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