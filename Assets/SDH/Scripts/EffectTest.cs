using UnityEngine;

public class EffectTest : MonoBehaviour
{
    public void Homing()
    {
        EffectManager.Instance.AddPE(EffectManager.ProjectileEffectType.Homing);
    }
    public void Explosive()
    {
        EffectManager.Instance.AddPE(EffectManager.ProjectileEffectType.Explosive);
    }
    public void PiercingEnemy()
    {
        EffectManager.Instance.AddPE(EffectManager.ProjectileEffectType.PiercingEnemy);
    }
    public void PiercingWall()
    {
        EffectManager.Instance.AddPE(EffectManager.ProjectileEffectType.PiercingWall);
    }
    public void Poison()
    {
        EffectManager.Instance.AddCE(EffectManager.CommonEffectType.Poison);
    }
    public void Stun()
    {
        EffectManager.Instance.AddME(EffectManager.MeleeEffectType.Stun);
    }
    public void Bleed()
    {
        EffectManager.Instance.AddME(EffectManager.MeleeEffectType.Bleed);
    }
    public void Knockback()
    {
        EffectManager.Instance.AddME(EffectManager.MeleeEffectType.Knockback);
    }
    public void Fire()
    {
        EffectManager.Instance.AddCE(EffectManager.CommonEffectType.Fire);
    }
    public void Ice()
    {
        EffectManager.Instance.AddCE(EffectManager.CommonEffectType.Ice);
    }
    public void Lightning()
    {
        EffectManager.Instance.AddCE(EffectManager.CommonEffectType.Lightning);
    }
}
