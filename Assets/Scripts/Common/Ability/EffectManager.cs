using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;




public interface IProjectileEffect
{
    void UpdateEffect(Projectile projectile);

    void OnHit(Projectile projectile, GameObject gameObject);

    void OnDestroy(Projectile projectile);
}


public interface ICommonEffect
{
    void UpdateEffect(Projectile projectile = null);

    void OnHit(GameObject gameObject, float dmg);

    void OnDestroy(Projectile projectile = null);
}

public interface IMeleeEffect
{
    void OnHit(GameObject gameObject);
}



public class EffectManager : MonoBehaviour
{
    public enum ProjectileEffectType
    {
        Homing,
        Explosive,
        PiercingEnemy,
        PiercingWall
    }

    public enum CommonEffectType
    {
        Poison,
        Fire,
        Ice,
        Lightning
    }

    public enum MeleeEffectType
    {
        Bleed,
        Stun,
        Knockback
    }
    public static EffectManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitEffects();
    }


    List<ProjectileEffectType> activePE = new List<ProjectileEffectType>();
    List<CommonEffectType> activeEE = new List<CommonEffectType>();
    List<MeleeEffectType> activeME = new List<MeleeEffectType>();
    
    List<IProjectileEffect> projectileEffects = new List<IProjectileEffect>();
    List<ICommonEffect> commonEffects = new List<ICommonEffect>();
    List<IMeleeEffect> meleeEffects = new List<IMeleeEffect>();

    //원거리
    HomingEffect homingEffect;
    ExplosiveEffect explosiveEffect;
    PiercingEnemyEffect piercingEnemyEffect;
    PiercingWallEffect piercingWallEffect;

    //공통
    PosionEffect posionEffect;
    LightningEffect lightningEffect;
    IceEffect iceEffect;

    // 근접
    StunEffect stunEffect;
    BleedEffect bleedEffect;
    KnockBackEffect knockbackEffect;

    void InitEffects()
    {
        homingEffect = new HomingEffect();
        explosiveEffect = new ExplosiveEffect();
        piercingEnemyEffect = new PiercingEnemyEffect();
        piercingWallEffect = new PiercingWallEffect();
        posionEffect = new PosionEffect();
        lightningEffect = new LightningEffect();
        iceEffect = new IceEffect();
        stunEffect = new StunEffect();
        bleedEffect = new BleedEffect();
        knockbackEffect = new KnockBackEffect();
    }

    public void AddPE(ProjectileEffectType effectType)
    {
        if (!activePE.Contains(effectType))
        {
            activePE.Add(effectType);
        }
        UpdateEffects();
    }

    public void RemovePE(ProjectileEffectType effectType)
    {
        if (activePE.Contains(effectType))
        {
            activePE.Remove(effectType);
        }
        UpdateEffects();
    }

    public void AddCE(CommonEffectType effectType)
    {
        if (!activeEE.Contains(effectType))
        {
            activeEE.Add(effectType);
        }
        UpdateEffects();
    }

    public void RemoveCE(CommonEffectType effectType)
    {
        if (activeEE.Contains(effectType))
        {
            activeEE.Remove(effectType);
        }
        UpdateEffects();
    }

    public void AddME(MeleeEffectType effectType)
    {
        if (!activeME.Contains(effectType))
        {
            activeME.Add(effectType);
        }
        UpdateEffects();
    }

    public void RemoveME(MeleeEffectType effectType)
    {
        if (activeME.Contains(effectType))
        {
            activeME.Remove(effectType);
        }
        UpdateEffects();
    }

    void UpdateEffects()
    {
        // 모든 효과 리스트 초기화
        projectileEffects.Clear();
        commonEffects.Clear();
        meleeEffects.Clear();

        // 활성화된 원거리 효과들을 리스트에 추가
        foreach (ProjectileEffectType effectType in activePE)
        {
            switch (effectType)
            {
                case ProjectileEffectType.Homing:
                    projectileEffects.Add(homingEffect);
                    break;
                case ProjectileEffectType.Explosive:
                    projectileEffects.Add(explosiveEffect);
                    break;
                case ProjectileEffectType.PiercingEnemy:
                    projectileEffects.Add(piercingEnemyEffect);
                    break;
                case ProjectileEffectType.PiercingWall:
                    projectileEffects.Add(piercingWallEffect);
                    break;
            }
        }

        // 활성화된 공통 효과들을 리스트에 추가
        foreach (CommonEffectType effectType in activeEE)
        {
            switch (effectType)
            {
                case CommonEffectType.Poison:
                    commonEffects.Add(posionEffect);
                    break;
                case CommonEffectType.Fire:
                    // fireEffect가 생성되면 추가
                    break;
                case CommonEffectType.Ice:
                    commonEffects.Add(iceEffect);
                    break;
                case CommonEffectType.Lightning:
                    commonEffects.Add(lightningEffect);
                    break;
            }
        }

        // 활성화된 근접 효과들을 리스트에 추가
        foreach (MeleeEffectType effectType in activeME)
        {
            switch (effectType)
            {
                case MeleeEffectType.Bleed:
                    meleeEffects.Add(bleedEffect);
                    break;
                case MeleeEffectType.Stun:
                    meleeEffects.Add(stunEffect);
                    break;
                case MeleeEffectType.Knockback:
                    meleeEffects.Add(knockbackEffect);
                    break;
            }
        }
    }

    public List<IProjectileEffect> GetActiveProjectileEffects()
    {
        return projectileEffects;
    }

    public List<ICommonEffect> GetActiveCommonEffects()
    {
        return commonEffects;
    }

    public List<IMeleeEffect> GetActiveMeleeEffects()
    {
        return meleeEffects;
    }

    public void ApplyItemEffect(ItemInfo.AttackEnhanceType attType)
    {
        switch(attType)
        {
            case ItemInfo.AttackEnhanceType.Homing:
                AddPE(ProjectileEffectType.Homing);
                break;
            case ItemInfo.AttackEnhanceType.Explosive:
                AddPE(ProjectileEffectType.Explosive);
                break;
            case ItemInfo.AttackEnhanceType.PiercingEnemy:
                AddPE(ProjectileEffectType.PiercingEnemy);
                break;
            case ItemInfo.AttackEnhanceType.PiercingWall:
                AddPE(ProjectileEffectType.PiercingWall);
                break;


            case ItemInfo.AttackEnhanceType.Poision:
                AddCE(CommonEffectType.Poison);
                break;
            case ItemInfo.AttackEnhanceType.Fire:
                //불추가
                break;
            case ItemInfo.AttackEnhanceType.Ice:
                AddCE(CommonEffectType.Ice);
                break;
            case ItemInfo.AttackEnhanceType.Lightning:
                AddCE(CommonEffectType.Lightning);
                break;


            case ItemInfo.AttackEnhanceType.Stun:
                AddME(MeleeEffectType.Stun);
                break;
            case ItemInfo.AttackEnhanceType.Bleed:
                AddME(MeleeEffectType.Bleed);
                break;
            case ItemInfo.AttackEnhanceType.Knockback:
                AddME(MeleeEffectType.Knockback);
                break;
        }
    }

}
