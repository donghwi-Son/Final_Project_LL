using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public enum AttackMode
{
    Melee,
    Ranged
}

public class AttackManager : MonoBehaviour
{
    Animator anim;
    PlayerStatus stat;

    //투사체 풀링용
    public List<ProjectilePoolData> poolList;
    public GameObject specialProjectile;
    Dictionary<ProjectileType, Queue<GameObject>> projectilePoolDic;

    HomingEffect homingEffect;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        stat = GetComponent<PlayerStatus>();
        InitializePool();
        InitializeEffect();
    }

    private void Update()
    {

    }

    void InitializeEffect()
    {
        homingEffect = new HomingEffect();
    }

void InitializePool()
    {
        projectilePoolDic = new Dictionary<ProjectileType, Queue<GameObject>>();

        foreach (var poolData in poolList)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < poolData.initialPoolSize; i++)
            {
                var obj = Instantiate(poolData.projectilePrefab);
                Projectile projectile = obj.GetComponent<Projectile>();
                projectile.OnProjectiledestroyed += ReturnProjectile;
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            projectilePoolDic[poolData.projectileType] = queue;
        }
    }

    public Projectile GetProjectile(ProjectileType type)
    {
        if(projectilePoolDic.ContainsKey(type))
        {
            if (projectilePoolDic[type].Count > 0)
            {
                var projectile = projectilePoolDic[type].Dequeue();
                projectile.SetActive(true);
                return projectile.GetComponent<Projectile>();
            }
            else
            {
                Debug.LogWarning($"No available projectile of type {type} in the pool. Consider increasing the pool size.");
                // Optionally, instantiate a new projectile if needed
                var newProjectileObj = Instantiate(poolList.Find(p => p.projectileType == type).projectilePrefab);
                Projectile newProjectile = newProjectileObj.GetComponent<Projectile>();
                newProjectile.OnProjectiledestroyed += ReturnProjectile;
                return newProjectile;
            }
        }
        else
        {
            Debug.LogError($"Projectile type {type} not found in the pool dictionary.");
            return null;
        }
    }

    public void ReturnProjectile(Projectile projectile)
    {
        if (projectile == null) return;
        ProjectileType type = projectile.projectileData.projectileType;
        if (projectilePoolDic.ContainsKey(type))
        {
            projectile.gameObject.SetActive(false);
            projectilePoolDic[type].Enqueue(projectile.gameObject);
        }
    }

    void FireProjectile()
    {
        Projectile projectile = GetProjectile(stat.projectileType);
        if (projectile == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        projectile.Fire(transform.position, dir, stat.damage, stat.projecTileLifeTime , projectile.projectileData.speed);

    }

    public void ApplyEffect(IProjectileEffect effect)
    {
        foreach (var poolData in poolList)
        {
            foreach (var projectile in projectilePoolDic[poolData.projectileType])
            {
                projectile.GetComponent<Projectile>().AddEffect(effect);
            }
        }
    }

    public void HomingON()
    {
        ApplyEffect(homingEffect);
    }


    public void Attack(AttackMode attmode)
    {
        switch(attmode)
        {
            case AttackMode.Melee:
                anim.SetTrigger("Att");
                break;
            case AttackMode.Ranged:
                anim.SetTrigger("Att");
                FireProjectile();
                break;
        }
    }

    public void SpecialAttack(AttackMode attmode)
    {
        switch(attmode)
        {
            case AttackMode.Melee:
                anim.SetTrigger("SpecialAttack");
                break;
            case AttackMode.Ranged:
                anim.SetTrigger("SpecialAttack");
                break;
        }
    }
    public void Skill()
    {

    }

    public void AirAttack()
    {
        anim.SetTrigger("AirAttack");
    }
}
