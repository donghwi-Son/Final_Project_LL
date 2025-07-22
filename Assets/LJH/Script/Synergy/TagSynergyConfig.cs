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
        => _map.TryGetValue(t, out s);
}
