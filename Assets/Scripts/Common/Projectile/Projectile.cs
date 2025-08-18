using System;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Normal,
    AAA,
    BBB,
    CCC
}


public class Projectile : MonoBehaviour
{
    public ProjectileData projectileData;
    public event Action<Projectile> OnProjectiledestroyed;
    float statdmg;
    float finalDamage;
    float finalSpeed;
    float finalLifeTime;
    float sizeFactor = 0.7f;
    bool isPiercingEnemy = false;
    bool isPiercingWall = false;

    [Header("컴포넌트")]
    private Rigidbody2D rb;
    private List<IProjectileEffect> PEs = new List<IProjectileEffect>();
    private List<ICommonEffect> CEs = new List<ICommonEffect>();

    private List<GameObject> hitEnemies = new List<GameObject>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        finalLifeTime -= Time.deltaTime;
        if(rb.linearVelocity.magnitude > finalSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * finalSpeed;
        }
        // 모든 효과의 업데이트 처리 실행
        foreach (var effect in PEs)
        {
            effect.UpdateEffect(this);
        }
        foreach (var effect in CEs)
        {
            effect.UpdateEffect(this);
        }

        if (finalLifeTime <= 0f)
        {
            DestroyProjectile();
        }
    }

    private void CalculateFinalStat(float statdmg, float statlf, float statshotspd)
    {
        finalDamage = projectileData.damageMultiplier * statdmg;
        finalLifeTime = statlf;
        finalSpeed = projectileData.speedMultiplier * statshotspd;
    }

    public void Fire(Vector2 pos, Vector2 dir, float statdmg, float statlf, float statshotspd)
    {
        this.statdmg = statdmg / 2f;
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
    public void ApplyEffects(List<IProjectileEffect> PE, List<ICommonEffect> IE)
    {
        PEs.AddRange(PE);
        CEs.AddRange(IE);
    }

    public void ClearEffects()
    {
        PEs.Clear();
        CEs.Clear();
    }

    public void EnablePiercingEnemy()
    {
        isPiercingEnemy = true;
    }

    public void EnablePiercingWall()
    {
        isPiercingWall = true;
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
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if(enemyScript != null)
            {
                enemyScript.Stats.TakeDamage((int)statdmg);
            }
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            if (other.gameObject.GetComponent<Enemy>().Stats.IsDead) return;

            if(!HasHitEnemy(other.gameObject))
            {
                //공격추가
                AddHitEnemy(other.gameObject);
            }

            // 모든 효과의 충돌 처리 실행
            foreach (var effect in PEs)
            {
                effect.OnHit(this, other.gameObject);
            }
            foreach (var effect in CEs)
            {
                effect.OnHit(other.gameObject, statdmg);
            }

            // 관통이 아니라면 파괴
            if (!isPiercingEnemy)
            {
                DestroyProjectile();
            }
        }
        else if (other.CompareTag("Wall"))
        {
            foreach (var effect in PEs)
            {
                effect.OnHit(this, other.gameObject);
            }
            if (!isPiercingWall)
            {
                // 벽에 충돌했을 때 파괴
                DestroyProjectile();
            }
        }
        else if (other.CompareTag("Structure"))
        {
            DestroyProjectile();
        }
    }
}