using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemEffectFactory : MonoBehaviour
{
    static ItemEffectConfig _cfg;
    [RuntimeInitializeOnLoadMethod]
    static void Init()
        => _cfg = Resources.Load<ItemEffectConfig>("ItemSO/Item Effect Config");
    
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
                ApplyStatIncrease(e.statType, e.statAmount);
                break;
            
            case ItemInfo.ItemUpgradeType.AttackEnhance:
                Debug.Log("기본 공격 강화 아이템 획득");
                EffectManager.Instance.ApplyItemEffect(e.attackEnhance);
                break;
            
            case ItemInfo.ItemUpgradeType.SkillUpgrade:
                Debug.Log("스킬 강화 아이템 획득");
                //PlayerTest.Instance.AddSkill(e.skillType);            //스킬 획득 넣어야 하는 부분
                break;
                

            case ItemInfo.ItemUpgradeType.SubAttack:
                Debug.Log("보조 공격 아이템 획득");
                //var subCfg = SubAttackRegistry.Instance.Get(e.subAttackType);                     //보조 공격 아이템 프리펩 추가
                //SubAttackSystem.Instance.SubAttackRegister(subCfg.prefab, e.subAttackCooldown);   //보조 공격 쿨타임 설정
                break;
                
            case ItemInfo.ItemUpgradeType.Utility:
                Debug.Log("유틸 아이템 획득");
                //PlayerController.ApplyUtility(e.utilityType, e.utilityAmount); //유틸 아이템 적용
                break;
        }
    }
    
    static void ApplyStatIncrease(ItemInfo.StatType statType, float amount)
    {
        PlayerStatus.Instance.ModifyStat(statType, amount);
    }
}
