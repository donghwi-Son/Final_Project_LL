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
        Health,             //체력
        Defense,            //방어력
        MoveSpeed,          //이속
        Power,              //힘
        Critical,           //크리티컬 확률
        AttackSpeed         //공속
    }
    
    public enum AttackEnhanceType
    {
        No,
        AttackRange,             //근접, 공격범위
        Stun,                    //근접, 스턴 효과(확률적으로 스턴)
        Penetrate,               //원거리, 관통 효과
        Split,                   //원거리, 화살 분할(1 -> 2)
        Poison,                  //공통, 독(지속 딜)
        Ice                      //공통, 슬로우
    }
    
    public enum SkillType
    {
        No,
        Swing
    }

    public enum SubAttackType
    {
        No,
        Boom
    }
    
    public enum ItemTag
    {
        Tag01,
        Tag02
    }
}
