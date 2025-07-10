using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HomingEffect : IProjectileEffect
{
    private float homingStrength = 10f;
    private float detectionRange = 10f;

    public void UpdateEffect(Projectile projectile)
    {
        Debug.Log("나는 유도");
        // 매 프레임마다 가장 가까운 적을 찾아서 유도
        GameObject nearestEnemy = FindNearestEnemy(projectile);
        if (nearestEnemy != null)
        {
            Vector3 direction = (nearestEnemy.transform.position - projectile.transform.position).normalized;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            // 현재 속도 방향과 목표 방향 사이의 각도 계산
            float angle = Vector3.Angle(rb.linearVelocity.normalized, direction);

            // 각도가 클수록 유도 효과 감소
            float effectiveHoming = homingStrength * (1f - angle / 180f);
            if(angle > 90f)
            {
                effectiveHoming = 0f;
            }
            rb.AddForce(direction * effectiveHoming);
            projectile.transform.right = Vector3.Lerp(projectile.transform.right, direction, Time.deltaTime);
        }
    }

    public void OnHit(Projectile projectile, GameObject target) { }

    public void OnDestroy(Projectile projectile) { }


    private GameObject FindNearestEnemy(Projectile pro)
    {
        // 가장 가까운 적 찾기 로직
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = detectionRange;

        foreach (GameObject enemy in enemies)
        {
            if(pro.HasHitEnemy(enemy))
                continue;

            float distance = Vector3.Distance(pro.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

}
