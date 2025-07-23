using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStatus : MonoBehaviour
{
    public ProjectileType projectileType;
    public float health = 100f;
    public float maxHealth = 100f;
    public float mana = 100f;
    public float maxMana = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float damage = 10f;
    public float defense = 2f;
    public float speed = 5f;
    public float attackSpeed = 1f;
    public float attackInterval => 1f / attackSpeed;
    public float attackRange = 1f;
    public float projecTileLifeTime = 5f;
    public float shotSpeed = 5f;
    public bool canChargeAttack = false;
    public bool canHoldAttack;
    public float criticalChance = 0.1f;
    
    public static PlayerStatus Instance { get; private set; }

    public void TakeDamage(float dmg)
    {
        float takenDamage = dmg * dmg / (dmg + defense);
        health -= takenDamage;
        if (health < 0)
        {
            health = 0;
            Die();
        }
    }

    public void Die()
    {

    }

    public void ChangeProjectile(ProjectileType type)
    {
        projectileType = type;
        ProjectilePool.Instance.ChangeProjectile(projectileType);
    }







    
    
    //스탯 증가
    public void ModifyStat(ItemInfo.StatType statType, float amount)
    {
        switch (statType)
        {
            case ItemInfo.StatType.Health:
                maxHealth += amount;
                break;
            case ItemInfo.StatType.Defense:
                defense += amount;
                break;
            case ItemInfo.StatType.MoveSpeed:
                speed += amount;
                break;
            case ItemInfo.StatType.Power:
                damage += amount;
                break;
            case ItemInfo.StatType.Critical:
                criticalChance = Mathf.Clamp01(criticalChance + amount);
                break;
            case ItemInfo.StatType.AttackSpeed:
                attackSpeed += amount;
                break;
        }
    }
}
