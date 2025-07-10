using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using static UnityEditor.Progress;


public enum AttackMode
{
    Melee,
    Ranged
}

public class AttackManager : MonoBehaviour
{
    Animator anim;
    PlayerStatus stat;
    Vector2 attOffset;
    Vector2 attPos;
    bool isRight;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        stat = GetComponent<PlayerStatus>();
    }
   

    void FireProjectile()
    {
        Projectile projectile = ProjectilePool.Instance.GetProjectile();
        if (projectile == null) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        projectile.Fire(transform.position, dir, stat.damage, stat.projecTileLifeTime, stat.shotSpeed);

    }

    public void Attack(AttackMode attmode, bool isRight)
    {
        switch (attmode)
        {
            case AttackMode.Melee:
                anim.SetTrigger("Att");
                this.isRight = isRight;
                break;
            case AttackMode.Ranged:
                anim.SetTrigger("Att");
                FireProjectile();
                break;
        }
    }

    public void SpecialAttack(AttackMode attmode)
    {
        switch (attmode)
        {
            case AttackMode.Melee:
                anim.SetTrigger("SpecialAttack");
                break;
            case AttackMode.Ranged:
                anim.SetTrigger("SpecialAttack");
                break;
        }
    }
    public void Skill()
    {

    }

    public void AirAttack()
    {
        anim.SetTrigger("AirAttack");
    }

    void MeleeAttack()
    {
        attOffset = isRight ? new Vector2(0.5f, 0) : new Vector2(-0.5f, 0);
        attPos = (Vector2)transform.position + attOffset;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attPos, stat.attackRange, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            //적 공격 메소드
            Debug.Log($"Hit Enemy: {enemy.name}");
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(attPos, stat.attackRange);
    //}
}
