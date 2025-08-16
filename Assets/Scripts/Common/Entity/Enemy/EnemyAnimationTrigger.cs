using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.AttackCheck.position, enemy.AttackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<PlayerController>() != null)
            {
                PlayerStats _target = hit.GetComponent<PlayerStats>();

                enemy.stats.DoDamage(_target);
            }
        }
    }
}
