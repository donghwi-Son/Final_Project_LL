using System;
using UnityEngine;

[Serializable]
public class TagSynergy
{
    public ItemInfo.ItemTag tag;
    public int threshold = 3;

    public TagSynergyConfig.SynergyType type = TagSynergyConfig.SynergyType.None;

    // 스탯
    public ItemInfo.StatType statType = ItemInfo.StatType.No;
    public float bonus = 1f;

    // 공격 타입
    public ItemInfo.AttackEnhanceType attackEnhance = ItemInfo.AttackEnhanceType.No;

    // 스킬
    public ItemInfo.SkillType skillType = ItemInfo.SkillType.No;

    // 유틸
    public ItemInfo.UtilityType utilityType = ItemInfo.UtilityType.No;
}