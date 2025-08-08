using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackMode
{
    Melee,
    Ranged
}

public class AttackManager : MonoBehaviour
{
    PlayerController player;
    Vector2 attOffset;
    bool isRight;
    float lastFireTime = -999f;
    bool canFire = true;
    public Transform firePoint;
    public Transform attPos;
    public Transform airAttPos;
    Vector3 dashPos;
    List<ICommonEffect> CEs = new List<ICommonEffect>();
    List<IMeleeEffect> MEs = new List<IMeleeEffect>();

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    bool CanFireProjectile()
    {
        return canFire && Time.time >= lastFireTime + player.attackInterval;
    }
    void FireProjectile()
    {
        if(!CanFireProjectile()) return;

        player.FlipByMouse();
        Projectile projectile = ProjectilePool.Instance.GetProjectile();
        if (projectile == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)firePoint.position).normalized;
        var PEs = EffectManager.Instance.GetActiveProjectileEffects();
        var CEs = EffectManager.Instance.GetActiveCommonEffects();
        projectile.ApplyEffects(PEs, CEs);
        projectile.Fire(firePoint.position, dir, player.stats.damage.GetValue(), player.stats.projectileLifeTime, player.stats.shotSpeed);

        lastFireTime = Time.time;
    }

    void FireChargeProjectile()
    {
        Projectile projectile = ProjectilePool.Instance.GetProjectile();
        if (projectile == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)firePoint.position).normalized;
        projectile.Fire(firePoint.position, dir, player.stats.damage.GetValue()*3f, player.stats.projectileLifeTime, player.stats.shotSpeed);

    }

    public void Attack(AttackMode attmode, bool isRight)
    {
        switch (attmode)
        {
            case AttackMode.Melee:
                this.isRight = isRight;
                break;
            case AttackMode.Ranged:
                if (!CanFireProjectile()) return;
                FireProjectile();
                break;
        }
    }

    public void ApplyMeleeEffect(GameObject enemy)
    {
        CEs = EffectManager.Instance.GetActiveCommonEffects();
        MEs = EffectManager.Instance.GetActiveMeleeEffects();
        foreach (IMeleeEffect effect in MEs)
        {
            effect.OnHit(enemy);
        }
        foreach (ICommonEffect effect in CEs)
        {
            effect.OnHit(enemy, player.stats.damage.GetValue());
        }
    }

    public void ChargeAttack()
    {
        
    }

    public void Skill()
    {

    }

    public void AirAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(airAttPos.position, player.attackRange*1.5f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            //적 공격 메소드
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            player.stats.DoDamage(enemyStats);
            ApplyMeleeEffect(enemy.gameObject);
            Debug.Log($"Hit Enemy: {enemy.name}");
        }
    }

    public void SpecialMeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(airAttPos.position, player.attackRange * 1.6f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            //적 공격 메소드
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            player.stats.DoDamage(enemyStats);
            ApplyMeleeEffect(enemy.gameObject);
            Debug.Log($"Hit Enemy: {enemy.name}");
        }
    }

    public void SpecialRangedAttack()
    {
        FireChargeProjectile();
    }

    public void DashAttack(bool isright)
    {
        dashPos = transform.position + (isright ? Vector3.right : Vector3.left) * 3.3f + new Vector3 (0,0.75f);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(dashPos, new Vector2(6.6f, 1.4f), 0f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            //적 공격 메소드
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            player.stats.DoDamage(enemyStats);
            ApplyMeleeEffect(enemy.gameObject);
            Debug.Log($"Hit Enemy: {enemy.name}");
        }
    }

    public void MeleeAttack(float _rangeMod)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attPos.position, player.attackRange * _rangeMod, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            //적 공격 메소드
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            player.stats.DoDamage(enemyStats);
            ApplyMeleeEffect(enemy.gameObject);
            Debug.Log($"Hit Enemy: {enemy.name}");
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(attPos.position, stat.attackRange);
    //    Gizmos.DrawWireSphere(airAttPos.position, stat.attackRange * 1.5f);
    //    Gizmos.DrawWireSphere(attPos.position, stat.attackRange * 1.3f);
    //    Gizmos.DrawWireCube(dashPos, new Vector2(6.6f, 1.4f));
    //}
}
