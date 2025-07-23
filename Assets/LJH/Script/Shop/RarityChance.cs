using System;
using UnityEngine;

[Serializable]
public class RarityChance
{
    public ItemInfo.ItemRarity rarity;
    [Range(0,100)]
    [Tooltip("이 등급이 뽑힐 확률(%) — 합이 100이어야 합니다.")]
    public int percentage;
}