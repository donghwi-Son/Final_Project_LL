using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("투사체 옵션")]
    public float speed = 5f;
    public float lifeTime = 3f;

    [Header("투사체 속성")]
    public bool toPlayer;

    private Vector2 direction;
    private Rigidbody2D rb;
    private CharacterStats parentsStats;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void SetParentsStats(CharacterStats stats)
    {
        parentsStats = stats;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어 데미지 로직
            parentsStats.DoDamage(collision.GetComponent<PlayerStats>());

            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (toPlayer)
        {
            rb.linearVelocity = direction * speed;
        }
    }
}
