using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName="Configs/TagSynergyConfig")]
public class TagSynergyConfig : ScriptableObject
{
    public enum SynergyType { None, Stats, Attack, Skill, Utility }

    public TagSynergy[] entries;
    
    Dictionary<ItemInfo.ItemTag, List<TagSynergy>> _map;

    void OnEnable()
    {
        _map = new Dictionary<ItemInfo.ItemTag, List<TagSynergy>>();
        if (entries == null) return;

        // 태그별 그룹화 + threshold 오름차순 정렬
        foreach (var g in entries.GroupBy(e => e.tag))
        {
            var list = g.OrderBy(e => e.threshold).ToList();
            _map[g.Key] = list;
        }
    }
    
    public bool TryGetSynergies(ItemInfo.ItemTag t, out List<TagSynergy> list)
    {
        list = null;

        if (_map == null || _map.Count == 0)
            OnEnable();

        if (_map != null && _map.TryGetValue(t, out var foundList) && foundList != null && foundList.Count > 0)
        {
            list = foundList;
            return true;
        }

        return false;
    }
}