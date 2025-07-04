using System.Collections.Generic;
using UnityEngine;

public class ItemDefinition
{
    public int    index;
    public string name;
    public ItemInfo.ItemRarity    rarity;
    public ItemInfo.ItemUpgradeType upgradeType;
    public string description;
    public string iconName;
    public Sprite icon;
    public List<ItemInfo.ItemTag> tags = new List<ItemInfo.ItemTag>();
}
