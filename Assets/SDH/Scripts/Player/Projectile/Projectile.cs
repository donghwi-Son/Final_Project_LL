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
    [Header("기본 속성")]
    public ProjectileType projectileType;
    public float speed = 10f;
    public float damage = 20f;
    public float lifeTime = 5f;
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


    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        foreach (var effect in effects)
        {
            effect.UpdateEffect(this);
        }

        if (lifeTime <= 0f)
        {
            DestroyProjectile();
        }
    }

    public void InitProjectile(Vector2 dir, Vector2 pos)
    {
        // 초기 위치 설정
        transform.position = pos;
        rb.linearVelocity = dir.normalized * speed;
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

    // 투사체 파괴(풀링용 active false)
    public void DestroyProjectile()
    {
        // 모든 효과의 파괴 처리 실행
        foreach (var effect in effects)
        {
            effect.OnDestroy(this);
        }
        gameObject.SetActive(false);
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