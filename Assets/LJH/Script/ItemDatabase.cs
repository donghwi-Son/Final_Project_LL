using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    private Dictionary<int, ItemDefinition> _defs;
    
    public static ItemDatabase Instance { get; private set; }

    void Start()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadDefinitions();
    }

    void LoadDefinitions()
    {
        var rows = CSVReader.Read("DataTable/ItemCSV");
        _defs = new Dictionary<int, ItemDefinition>();

        foreach (var row in rows)
        {
            var def = new ItemDefinition
            {
                index       = Convert.ToInt32(row["Index"]),
                name        = row["ItemName"].ToString(),
                rarity      = (ItemInfo.ItemRarity)Enum.Parse(
                    typeof(ItemInfo.ItemRarity),
                    row["ItemGrade"].ToString(), true),
                upgradeType = (ItemInfo.ItemUpgradeType)Enum.Parse(
                    typeof(ItemInfo.ItemUpgradeType),
                    row["UpgradeType"].ToString(), true),
                description = row["Description"].ToString(),
                iconName    = row["IconName"].ToString(),
            };
            if (row.TryGetValue("ItemTag", out var tagsObj) && tagsObj is string tagsStr)
            {
                foreach (var s in tagsStr.Split(','))
                    if (Enum.TryParse<ItemInfo.ItemTag>(s.Trim(), true, out var tag))
                        def.tags.Add(tag);
            }
            def.icon = Resources.Load<Sprite>($"ItemIcons/{def.iconName}");
            if (def.icon == null)
                Debug.LogWarning($"Icon '{def.iconName}' not found for item {def.name}");

            _defs[def.index] = def;
        }

        Debug.Log($"[ItemDatabase] Loaded {_defs.Count} items.");
    }
    
    public ItemDefinition GetDefinition(int idx)                                    //아이템 정보 조회하는 
    {
        if (_defs != null && _defs.TryGetValue(idx, out var def))
            return def;
        Debug.LogWarning($"ItemDatabase: 정의되지 않은 index {idx}");
        return null;
    }
}
