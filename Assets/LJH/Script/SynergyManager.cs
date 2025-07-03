using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    public static SynergyManager Instance { get; private set; }
    
    private Dictionary<ItemInfo.ItemTag,int> tagCounts = new();
    
    private HashSet<ItemInfo.ItemTag> activeTagSynergy = new();

    TagSynergyConfig  tagConfig;

    void Awake()
    {
        Instance = this;
        tagConfig = Resources.Load<TagSynergyConfig>("Item/TagSynergyConfig");
    }
    
    public void OnItemAcquired(ItemDefinition def)
    {
        Debug.Log("시너지 아이템 획득 성공");
        foreach (var tag in def.tags)
        {
            tagCounts[tag] = tagCounts.GetValueOrDefault(tag) + 1;
            TryApplyTagSynergy(tag);
        }
    }

    void TryApplyTagSynergy(ItemInfo.ItemTag tag)
    {
        Debug.Log("시너지 적용 시도");
        if (activeTagSynergy.Contains(tag)) return;
        if (!tagConfig.TryGetSynergy(tag, out var entry)) return;

        int count = tagCounts.GetValueOrDefault(tag);
        Debug.Log($"[Synergy] count={count}, threshold={entry.threshold}");
    
        if (count < entry.threshold) 
            return;

        // 한 번만 실행하게
        activeTagSynergy.Add(tag);
        Debug.Log($"[Synergy] 발동! tag={tag}, type={entry.type}, bonus={entry.bonus}");
        
        switch (entry.type)
        {
            case TagSynergyConfig.SynergyType.Stats:
                //스탯을 증가 일단 체력으로 테스트
                PlayerTest.Instance.AddHealth(entry.bonus);
                break;

            case TagSynergyConfig.SynergyType.Attack:
                // 공격강화
                // PlayerTest.Instance.AttackEnhance(entry.bonus);
                Debug.Log("공격 강화");
                break;

            case TagSynergyConfig.SynergyType.Skill:
                // 스킬 강화
                //SkillSystem.Instance.AddSkill();
                Debug.Log("스킬 획득");
                break;

            case TagSynergyConfig.SynergyType.Utility:
                // 유틸 효과 EX)더블 점프 활성화
                    PlayerTest.Instance.EnableDoubleJump();
                    Debug.Log("유틸 획득");
                break;

            default:
                Debug.LogWarning($"[Synergy] 알 수 없는 SynergyType: {entry.type}");
                break;
        }
    }
    
}
