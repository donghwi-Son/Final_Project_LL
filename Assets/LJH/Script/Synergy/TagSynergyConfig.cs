using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName="Configs/TagSynergyConfig")]
public class TagSynergyConfig : ScriptableObject
{
    public enum SynergyType
    {
        None,
        Stats,
        Attack,
        Skill,
        Utility
    }
    public TagSynergy[] entries;
    Dictionary<ItemInfo.ItemTag, TagSynergy> _map;
    void OnEnable()
        => _map = entries.ToDictionary(e=>e.tag);
    public bool TryGetSynergy(ItemInfo.ItemTag t, out TagSynergy s)
    {
        if (_map == null || _map.Count == 0)
        {
            Debug.LogWarning("TagSynergyConfig map not initialized! Rebuilding now.");
            _map = entries.ToDictionary(e => e.tag);
        }

        return _map.TryGetValue(t, out s);
    }
}
