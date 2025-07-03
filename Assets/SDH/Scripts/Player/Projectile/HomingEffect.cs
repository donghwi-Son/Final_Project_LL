using UnityEngine;

public class HomingEffect : IProjectileEffect
{
    private float homingStrength = 5f;
    private float detectionRange = 10f;

    public void UpdateEffect(Projectile projectile)
    {
        // 매 프레임마다 가장 가까운 적을 찾아서 유도
        GameObject nearestEnemy = FindNearestEnemy(projectile.transform.position);
        if (nearestEnemy != null)
        {
            Vector3 direction = (nearestEnemy.transform.position - projectile.transform.position).normalized;
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * homingStrength);
        }
    }

    public void OnHit(Projectile projectile, GameObject target) { }

    public void OnDestroy(Projectile projectile) { }


    private GameObject FindNearestEnemy(Vector3 position)
    {
        // 가장 가까운 적 찾기 로직
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = detectionRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

}
