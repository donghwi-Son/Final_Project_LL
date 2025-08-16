using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerController player;

    public ProjectileType projectileType;
    public Stat maxMana;
    public int currentMana;
    public Stat maxStamina;
    public int currentStamina;
    
    public float projectileLifeTime = 5f;
    public float shotSpeed = 5f;
    public bool canChargeAttack = false;
    public bool canHoldAttack;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<PlayerController>();
        currentMana = maxMana.GetValue();
        currentStamina = maxStamina.GetValue();
    }

    public void ChangeProjectile(ProjectileType type)
    {
        projectileType = type;
        ProjectilePool.Instance.ChangeProjectile(projectileType);
    }
    
    //스탯 증가
    public void ModifyStat(ItemInfo.StatType statType, float amount)
    {
        switch (statType)
        {
            case ItemInfo.StatType.Health:
                IncreaseStatBy(maxHealth, (int)amount);
                break;
            case ItemInfo.StatType.Defense:
                IncreaseStatBy(defense, (int)amount);
                break;
            case ItemInfo.StatType.MoveSpeed:
                player.MoveSpeed += amount;
                break;
            case ItemInfo.StatType.Power:
                IncreaseStatBy(damage, (int)amount);
                break;
            case ItemInfo.StatType.Critical:
                IncreaseStatBy(critChance, (int)amount);
                break;
            case ItemInfo.StatType.AttackSpeed:
                player.AttackSpeed += amount;
                break;
        }
    }
    
    public void IncreaseAllStats(float amount)
    {
        IncreaseStatBy(maxHealth, (int)amount * 10);
        IncreaseStatBy(defense, (int)amount);
        IncreaseStatBy(damage, (int)amount);
        IncreaseStatBy(critChance, (int)amount);
        
        player.MoveSpeed += amount;
        player.AttackSpeed += amount;
        
        IncreaseStatBy(maxMana, (int)amount);
        IncreaseStatBy(maxStamina, (int)amount);

        Debug.Log($"모든 스탯이 {amount}만큼 증가했습니다.");
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        player.DamageImpact();
    }

    protected override void Die()
    {
        base.Die();

        player.Die();
    }
}
