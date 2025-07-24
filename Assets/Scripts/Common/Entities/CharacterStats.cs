using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;              // default value 150%
    public Stat attackSpeed;
    public float attackRange;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat defense;

    public float attackInterval => 100f / attackSpeed.GetValue();
    public int currentHealth;

    public System.Action onHealthChanged;

    public bool isDead { get; private set; }

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
    }

    public virtual void IncreaseStatBy(Stat _statToModify, int _modifier)
    {
        _statToModify.AddModifier(_modifier);
    }

    public virtual void IncreaseStatBy(Stat _statToModify, int _modifier, float _duration)
    {
        StartCoroutine(StatModCoroutine(_statToModify, _modifier, _duration));
    }

    private IEnumerator StatModCoroutine(Stat _statToModify, int _modifier, float _duration)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        int totalDamage = damage.GetValue() * damage.GetValue() / (damage.GetValue() + _targetStats.defense.GetValue());

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        _targetStats.TakeDamage(totalDamage);
    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();

        if (currentHealth < 0 && !isDead)
            Die();
    }


    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > maxHealth.GetValue())
            currentHealth = maxHealth.GetValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region Stat calculations
    private bool CanCrit()
    {
        if (Random.Range(0, 100) <= critChance.GetValue())
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float critDamage = _damage * critPower.GetValue();

        return Mathf.RoundToInt(critDamage);
    }
    #endregion
}
