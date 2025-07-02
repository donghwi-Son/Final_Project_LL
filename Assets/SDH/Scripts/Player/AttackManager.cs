using System.Collections;
using UnityEngine;


public enum AttackMode
{
    Melee,
    Ranged
}


public class AttackManager : MonoBehaviour
{
    Animator anim;
    public GameObject basicProjectile;
    public GameObject specialProjectile;



    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Attack(AttackMode attmode)
    {
        switch(attmode)
        {
            case AttackMode.Melee:
                anim.SetTrigger("Att");
                break;
            case AttackMode.Ranged:
                anim.SetTrigger("Att");
                break;
        }
    }

    public void SpecialAttack(AttackMode attmode)
    {
        switch(attmode)
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
}
