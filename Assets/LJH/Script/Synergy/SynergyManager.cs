using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    public static SynergyManager Instance { get; private set; }
    
    private Dictionary<ItemInfo.ItemTag,int> tagCounts = new();
    
    private readonly Dictionary<ItemInfo.ItemTag, HashSet<int>> activatedTierIndices = new();

    private TagSynergyConfig  tagConfig;

    void Awake()
    {
        Instance = this;
        tagConfig = Resources.Load<TagSynergyConfig>("ItemSO/TagSynergyConfig");
    }
    
    public void OnItemAcquired(ItemDefinition def)
    {
        foreach (var tag in def.tags)
        {
            tagCounts[tag] = tagCounts.GetValueOrDefault(tag) + 1;
            TryApplySynergies(tag);
        }
    }

    private void TryApplySynergies(ItemInfo.ItemTag tag)
    {
        if (tagConfig == null) return;
        if (!tagConfig.TryGetSynergies(tag, out var list)) return;

        int count = tagCounts.GetValueOrDefault(tag);
        if (!activatedTierIndices.TryGetValue(tag, out var activated))
        {
            activated = new HashSet<int>();
            activatedTierIndices[tag] = activated;
        }
        
        for (int i = 0; i < list.Count; i++)
        {
            var entry = list[i];
            if (activated.Contains(i)) continue;
            if (entry.threshold > count) break;

            ActivateSynergy(entry);
            activated.Add(i);
        }
    }

    private void ActivateSynergy(TagSynergy entry)
    {
        Debug.Log($"[Synergy] 발동! tag={entry.tag}, type={entry.type}, threshold={entry.threshold}, bonus={entry.bonus}");

        switch (entry.type)
        {
            case TagSynergyConfig.SynergyType.Stats:
                if (entry.statType == ItemInfo.StatType.AllStats)
                {
                    PlayerStatus.Instance.IncreaseAllStats(1);
                }
                else
                {
                    PlayerStatus.Instance.ModifyStat(entry.statType, entry.bonus);
                }
                break;

            case TagSynergyConfig.SynergyType.Attack:
                EffectManager.Instance.ApplyItemEffect(entry.attackEnhance);
                break;

            case TagSynergyConfig.SynergyType.Skill:
                // SkillSystem.Instance.AddSkill(entry.skillType);
                Debug.Log($"[Synergy] 스킬 적용: {entry.skillType}");
                break;

            case TagSynergyConfig.SynergyType.Utility:
                // 아이템 로직과 동일 경로
                PlayerManager.Instance.player.ApplyUtility(entry.utilityType, entry.bonus);
                break;

            default:
                Debug.LogWarning($"[Synergy] 알 수 없는 SynergyType: {entry.type}");
                break;
        }
    }
}
