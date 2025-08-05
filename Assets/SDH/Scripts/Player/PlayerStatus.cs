using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStatus : CharacterStats
{
    public static PlayerStatus Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ProjectileType projectileType;
    public Stat maxMana;
    public int currentMana;
    public Stat maxStamina;
    public int currentStamina;
    
    public float projectileLifeTime = 5f;
    public float shotSpeed = 5f;
    public bool canChargeAttack = false;
    public bool canHoldAttack;

    protected override void Start()
    {
        base.Start();

        currentMana = maxMana.GetValue();
        currentStamina = maxStamina.GetValue();
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
                IncreaseStatBy(maxHealth, (int)amount);
                break;
            case ItemInfo.StatType.Defense:
                IncreaseStatBy(defense, (int)amount);
                break;
            case ItemInfo.StatType.MoveSpeed:
                PlayerController player = GetComponent<PlayerController>();
                player.moveSpeed += amount;
                break;
            case ItemInfo.StatType.Power:
                IncreaseStatBy(damage, (int)amount);
                break;
            case ItemInfo.StatType.Critical:
                IncreaseStatBy(critChance, (int)amount);
                break;
            case ItemInfo.StatType.AttackSpeed:
                IncreaseStatBy(attackSpeed, (int)amount);
                break;
        }
    }

}
