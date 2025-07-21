using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;




public enum ProjectileEffectType
{
    None,
    Homing,
    Explosive,
    Piercing
}

public enum CommonEffectType
{
    None,
    Fire,
    Ice,
    Lightning
}

public enum MeleeEffectType
{
    None,
    Bleed,
    Stun,
    Knockback
}

public class EffectManager : MonoBehaviour
{
    List<ProjectileEffectType> activePE = new List<ProjectileEffectType>();
    List<CommonEffectType> activeEE = new List<CommonEffectType>();
    List<MeleeEffectType> activeME = new List<MeleeEffectType>();
    Dictionary<ProjectileEffectType, IProjectileEffect> PEDic = new Dictionary<ProjectileEffectType, IProjectileEffect>();



    HomingEffect homingEffect;
    ExplosiveEffect explosiveEffect;
    PiercingEffect piercingEffect;

    private void Awake()
    {
        InitEffects();
    }

    void InitEffects()
    {
        homingEffect = new HomingEffect();
        explosiveEffect = new ExplosiveEffect();
        piercingEffect = new PiercingEffect();
    }

    public void AddPE(ProjectileEffectType effectType)
    {
        if (!activePE.Contains(effectType))
        {
            activePE.Add(effectType);
        }
    }

    public void RemovePE(ProjectileEffectType effectType)
    {
        if (activePE.Contains(effectType))
        {
            activePE.Remove(effectType);
        }
    }

    public void AddEE(CommonEffectType effectType)
    {
        if (!activeEE.Contains(effectType))
        {
            activeEE.Add(effectType);
        }
    }

    public void RemoveEE(CommonEffectType effectType)
    {
        if (activeEE.Contains(effectType))
        {
            activeEE.Remove(effectType);
        }
    }

    public void AddME(MeleeEffectType effectType)
    {
        if (!activeME.Contains(effectType))
        {
            activeME.Add(effectType);
        }
    }

    public void RemoveME(MeleeEffectType effectType)
    {
        if (activeME.Contains(effectType))
        {
            activeME.Remove(effectType);
        }
    }
}
