using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifeTime = 5f;
    public LayerMask enemyLayer;

    [Header("Enhancement Effects")]
    public bool hasPenetration = false;
    public bool hasExplosion = false;
    public bool hasChain = false;
    public float explosionRadius = 2f;
    public int chainCount = 3;
    public int penetrationCount = 0;

    private Vector2 direction;
    private List<GameObject> hitEnemies = new List<GameObject>();
    private int currentPenetrations = 0;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 dir, float dmg, Dictionary<EnhancementType, AttackEnhancement> enhancements)
    {
        direction = dir.normalized;
        damage = dmg;

        // 강화 효과 적용
        ApplyEnhancements(enhancements);

        // 투사체 이동
        rb.linearVelocity = direction * speed;

        // 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    private void ApplyEnhancements(Dictionary<EnhancementType, AttackEnhancement> enhancements)
    {
        foreach (var enhancement in enhancements)
        {
            switch (enhancement.Key)
            {
                case EnhancementType.Speed:
                    speed += enhancement.Value.value * enhancement.Value.stacks;
                    break;
                case EnhancementType.Damage:
                    damage += enhancement.Value.value * enhancement.Value.stacks;
                    break;
                case EnhancementType.Penetration:
                    hasPenetration = true;
                    penetrationCount += Mathf.RoundToInt(enhancement.Value.value) * enhancement.Value.stacks;
                    break;
                case EnhancementType.Explosion:
                    hasExplosion = true;
                    explosionRadius += enhancement.Value.value * enhancement.Value.stacks;
                    break;
                case EnhancementType.Chain:
                    hasChain = true;
                    chainCount += Mathf.RoundToInt(enhancement.Value.value) * enhancement.Value.stacks;
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        
    }

    private void CreateExplosion(Vector2 center)
    {
        //Collider2D[] enemies = Physics2D.OverlapCircleAll(center, explosionRadius, enemyLayer);
        //foreach (var enemy in enemies)
        //{
        //    if (!hitEnemies.Contains(enemy.gameObject))
        //    {
        //        Enemy enemyScript = enemy.GetComponent<Enemy>();
        //        if (enemyScript != null)
        //        {
        //            enemyScript.TakeDamage(damage * 0.7f); // 폭발 데미지는 70%
        //        }
        //    }
        //}

        //// 폭발 이펙트 생성 (여기서는 간단한 원형 표시)
        //GameObject explosion = new GameObject("Explosion");
        //explosion.transform.position = center;
        //LineRenderer lr = explosion.AddComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Sprites/Default"));
        //lr.color = Color.red;
        //lr.startWidth = 0.1f;
        //lr.endWidth = 0.1f;
        //lr.positionCount = 50;

        //for (int i = 0; i < 50; i++)
        //{
        //    float angle = i * Mathf.PI * 2 / 50;
        //    Vector3 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * explosionRadius;
        //    lr.SetPosition(i, pos);
        //}

        //Destroy(explosion, 0.5f);
    }

    private void CreateChain(Vector2 startPos)
    {
        //Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(startPos, 5f, enemyLayer);
        //int chained = 0;

        //foreach (var enemy in nearbyEnemies)
        //{
        //    if (!hitEnemies.Contains(enemy.gameObject) && chained < chainCount)
        //    {
        //        Enemy enemyScript = enemy.GetComponent<Enemy>();
        //        if (enemyScript != null)
        //        {
        //            enemyScript.TakeDamage(damage * 0.5f); // 체인 데미지는 50%
        //            hitEnemies.Add(enemy.gameObject);
        //            chained++;

        //            // 체인 이펙트 라인 생성
        //            CreateChainEffect(startPos, enemy.transform.position);
        //        }
        //    }
        //}
    }

    private void CreateChainEffect(Vector2 start, Vector2 end)
    {
        //GameObject chain = new GameObject("Chain");
        //LineRenderer lr = chain.AddComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Sprites/Default"));
        //lr.color = Color.cyan;
        //lr.startWidth = 0.05f;
        //lr.endWidth = 0.05f;
        //lr.positionCount = 2;
        //lr.SetPosition(0, start);
        //lr.SetPosition(1, end);

        //Destroy(chain, 0.3f);
    }
}