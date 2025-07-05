using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public ProjectileType projectileType;
    public float health = 100f;
    public float maxHealth = 100f;
    public float mana = 100f;
    public float maxMana = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float damage = 10f;
    public float defense = 2f;
    public float speed = 5f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
    public float projecTileLifeTime = 5f;


    public void TakeDamage(float dmg)
    {
        float takenDamage = dmg * dmg / (dmg + defense);
        health -= takenDamage;
        if (health < 0)
        {
            health = 0;
            Die();
        }
    }

    public void Die()
    {

    }
}
