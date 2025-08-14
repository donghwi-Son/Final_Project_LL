using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/ItemData")]
public class ItemInfo : ScriptableObject
{
    public enum ItemRarity
    {
        Common,             //5등급
        Uncommon,           //4등급
        Rare,               //3등급
        Epic,               //2등급
        Legendary,          //1등급
        Boss                //보스 유물
    }
    
    public enum ItemUpgradeType
    {
        StatIncrease,    // 스탯 증가 (공격력, 방어력, 속도 등등)
        AttackEnhance,   // 공격 강화 (스킬 강화)
        SkillUpgrade,    // 스킬 업그레이드
        SubAttack,       // 서브 어택 (기본 공격, 스킬 이외의 공격)
        Utility          // 유틸 (유틸 관련)
    }
    
    public enum StatType    //이후 플레이어 스탯을 상속 받아서 진행
    {
        No,
        Health,             //체력
        Defense,            //방어력
        MoveSpeed,          //이속
        Power,              //힘
        Critical,           //크리티컬 확률
        AttackSpeed,         //공속
        AllStats
    }
    
    public enum AttackEnhanceType
    {
        No,
        Homing,
        Explosive,
        PiercingEnemy,
        PiercingWall,

        Poision,
        Fire,
        Ice,
        Lightning,

        Bleed,
        Stun,
        Knockback
    }
    
    public enum SkillType
    {
        No,
        Heal,
        SwardSlash,
        Adrenaline
    }

    public enum SubAttackType
    {
        No,
        Zone,
        Healer
    }

    public enum UtilityType
    {
        No,
        DoubleJump,         //더블 점프
        DashCoolDown,
        AddDashCount       //대쉬 횟수 증가
    }
    
    public enum ItemTag
    {
        CombatSet,
        WindSet,
        GoblinSet,
        WeaponMaster,
        Amulet
    }
}
