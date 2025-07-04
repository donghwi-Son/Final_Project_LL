using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName="Configs/ItemEffectConfig")]
public class ItemEffectConfig : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public int      index;
        public ItemInfo.ItemUpgradeType upgradeType;

        // StatIncrease 증가량
        public float statAmount;
        
        // AttackEnhance 종류
        public ItemInfo.AttackEnhanceType attackEnhance;
        
        //skill 종류
        public ItemInfo.SkillType skillType;
        
        // SubAttack 종류랑 쿨다운
        public ItemInfo.SubAttackType subAttackType;
        public float subAttackCooldown;
        
        // Utility 활성화
        public bool enableDoubleJump;
        
        //public ItemInfo.ItemTag itemTag;
    }

    public Entry[] entries;
    
    Dictionary<int, Entry> _map;
    void OnEnable()
        => _map = entries.ToDictionary(e=>e.index);

    public bool TryGet(int idx, out Entry e)
        => _map.TryGetValue(idx, out e);
}
