using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TagSynergy
{
    public ItemInfo.ItemTag tag;       
    public int threshold;
    public TagSynergyConfig.SynergyType type;
    public float bonus;
}

