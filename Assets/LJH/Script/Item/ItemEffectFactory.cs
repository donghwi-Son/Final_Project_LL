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
                if (e.statType == ItemInfo.StatType.AllStats)
                {
                    PlayerStatus.Instance.IncreaseAllStats(1);
                }
                else
                {
                    ApplyStatIncrease(e.statType, e.statAmount);
                }
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
            {
                string prefabName = e.subAttackType.ToString();
                GameObject prefab = Resources.Load<GameObject>($"SubAttacks/{prefabName}");
                if (prefab == null)
                {
                    Debug.LogError($"서브어택 프리팹을 찾을 수 없음: Resources/SubAttacks/{prefabName}.prefab");
                    break;
                }
                
                // Config 쿨다운을 우선 적용하고 싶으면 overrideCooldown 전달
                SubAttackSlot.Instance.Equip(prefab, e.subAttackCooldown > 0 ? e.subAttackCooldown : -1f);
                break;
            }
                
            case ItemInfo.ItemUpgradeType.Utility:
                Debug.Log("유틸 아이템 획득");
                PlayerManager.Instance.player.ApplyUtility(e.utilityType, e.utilityAmount); //유틸 아이템 적용
                break;
        }
    }
    
    static void ApplyStatIncrease(ItemInfo.StatType statType, float amount)
    {
        PlayerManager.Instance.player.GetComponent<PlayerStatus>().ModifyStat(statType, amount);
    }
}
