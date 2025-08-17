using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemEffectFactory : MonoBehaviour
{
    public static event Action<int> OnEffectApplied;
    
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
        
        bool success = false;

        switch (def.upgradeType)
        {
            case ItemInfo.ItemUpgradeType.StatIncrease:
                Debug.Log("스탯 증가 아이템 획득");
                if (e.statType == ItemInfo.StatType.AllStats)
                    PlayerManager.Instance.player.Stats.IncreaseAllStats(1);
                else
                    ApplyStatIncrease(e.statType, e.statAmount);
                success = true;
                break;

            case ItemInfo.ItemUpgradeType.AttackEnhance:
                Debug.Log("기본 공격 강화 아이템 획득");
                EffectManager.Instance.ApplyItemEffect(e.attackEnhance);
                success = true;
                break;

            case ItemInfo.ItemUpgradeType.SkillUpgrade:
                Debug.Log("스킬 강화 아이템 획득");
                success = true;
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

                var cd = e.subAttackCooldown > 0 ? e.subAttackCooldown : -1f;
                var sub = SubAttackSlot.Instance.Equip(prefab, cd);
                success = sub != null;
                break;
            }

            case ItemInfo.ItemUpgradeType.Utility:
                Debug.Log("유틸 아이템 획득");
                PlayerManager.Instance.player.ApplyUtility(e.utilityType, e.utilityAmount);
                success = true;
                break;
        }
        
        if (success) OnEffectApplied?.Invoke(def.index);
    }
    
    static void ApplyStatIncrease(ItemInfo.StatType statType, float amount)
    {
        PlayerManager.Instance.player.GetComponent<PlayerStats>().ModifyStat(statType, amount);
    }
}
