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
        AttackRange,             //근접, 공격범위
        Stun,                    //근접, 스턴 효과(확률적으로 스턴)
        Penetrate,               //원거리, 관통 효과
        Split,                   //원거리, 화살 분할(1 -> 2)
        Poison,                  //공통, 독(지속 딜)
        Ice                      //공통, 슬로우
    }

    public enum SkillType
    {
        
    }

    [Header("Item Info")] 
    public int itemIndex;
    public string itemName;
    public ItemRarity itemRarity;
    public ItemUpgradeType itemUpgradeType;
    public string itemDescription;
    public Sprite itemIcon;
    
    [System.Serializable]
    public class EffectData
    {
        public ItemUpgradeType type;
    
        // 스탯 증가
        public float statAmount;                //증가량
        public StatType statType;               //스탯 종류
    
        // 공격 강화 부분은 스킬 보고 해야할 듯
    
        // 서브 어택
        public GameObject subAttackPrefab;          //보조 공격 수단 프리펩
        public float subAttackCooldown;             //클타임
    
        // 유틸도 이후에 진행
    }
}
