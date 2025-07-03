using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemEffectFactory : MonoBehaviour
{
    static ItemEffectConfig _cfg;
    [RuntimeInitializeOnLoadMethod]
    static void Init()
        => _cfg = Resources.Load<ItemEffectConfig>("Item/Item Effect Config");
    
    public static void ApplyEffect(ItemDefinition def)
    {
        if (def == null) return;
        if (!_cfg.TryGet(def.index, out var e))
        {
            Debug.LogWarning($"Config 없음: index={def.index}");
            return;
        }
        
        switch (def.upgradeType)
        {
            case ItemInfo.ItemUpgradeType.StatIncrease:
                Debug.Log("스탯 증가 아이템 획득");
                PlayerTest.Instance.AddHealth(e.statAmount);
                break;
            
            case ItemInfo.ItemUpgradeType.AttackEnhance:
                Debug.Log("기본 공격 강화 아이템 획득");
                //PlayerTest.Instance.AttackEnhance(e.attackEnhance);
                break;
            
            case ItemInfo.ItemUpgradeType.SkillUpgrade:
                Debug.Log("스킬 강화 아이템 획득");
                //PlayerTest.Instance.AddSkill(e.skillType);
                break;
                

            case ItemInfo.ItemUpgradeType.SubAttack:
                Debug.Log("보조 공격 아이템 획득");
                //var subCfg = SubAttackRegistry.Instance.Get(e.subAttackType);
                //SubAttackSystem.Instance.SubAttackRegister(subCfg.prefab, e.subAttackCooldown);
                break;
                
            case ItemInfo.ItemUpgradeType.Utility:
                Debug.Log("유틸 아이템 획득");
                //PlayerController.Instance.EnableDoubleJump();
                break;
        }
    }
}
