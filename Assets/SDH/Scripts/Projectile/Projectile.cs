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
    float piercingCount = 0;
    float sizeFactor = 0.7f;
    bool isPiercing = false;


    [Header("컴포넌트")]
    private Rigidbody2D rb;
    private List<IProjectileEffect> effects = new List<IProjectileEffect>();
    private List<GameObject> hitEnemies = new List<GameObject>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        finalLifeTime -= Time.deltaTime;
        if(rb.linearVelocity.magnitude > finalSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * finalSpeed;
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


    void CalculateFinalStat(float statdmg, float statlf, float statshotspd)
    {
        finalDamage = projectileData.damageMultiplier * statdmg;
        finalLifeTime = statlf;
        finalSpeed = projectileData.speedMultiplier * statshotspd;
        piercingCount = projectileData.piercingCount;
    }

    public void Fire(Vector2 pos, Vector2 dir, float statdmg, float statlf, float statshotspd)
    {
        CalculateFinalStat(statdmg, statlf, statshotspd);
        sizeFactor = 0.7f + statdmg * 0.05f;
        sizeFactor = Mathf.Clamp(sizeFactor, 0.7f, 2.0f);
        transform.localScale = new Vector3(sizeFactor, sizeFactor, 1f);
        transform.position = pos;
        transform.right = dir;
        gameObject.SetActive(true);
        rb.linearVelocity = dir.normalized * finalSpeed;
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
        Debug.Log($"Remaining Piercing Count: {piercingCount}");
    }

    public void DestroyProjectile()
    {
        ClearHitEnemies();
        OnProjectiledestroyed?.Invoke(this);
    }

    public void AddHitEnemy(GameObject enemy)
    {
        if (!hitEnemies.Contains(enemy))
        {
            hitEnemies.Add(enemy);
        }
    }

    public bool HasHitEnemy(GameObject enemy)
    {
        return hitEnemies.Contains(enemy);
    }

    public void ClearHitEnemies()
    {
        hitEnemies.Clear();
    }

    // 충돌 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(!HasHitEnemy(other.gameObject))
            {
                //공격추가
                AddHitEnemy(other.gameObject);
            }

            // 모든 효과의 충돌 처리 실행
            foreach (var effect in effects)
            {
                effect.OnHit(this, other.gameObject);
            }
            DecreasePiercingCount();

            // 관통이 아니라면 파괴
            if (piercingCount <= 0)
            {
                DestroyProjectile();
            }
        }
    }
}