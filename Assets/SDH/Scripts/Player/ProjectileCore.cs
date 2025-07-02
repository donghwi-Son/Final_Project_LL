// ProjectileCore.cs
using UnityEngine;

public class ProjectileCore : MonoBehaviour
{
    [Header("Base Stats")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifeTime = 5f;

    public Rigidbody2D rb;          // 필수
    private float lifeTimer;

    private void Awake() { rb = GetComponent<Rigidbody2D>(); }

    public void Init(Vector2 dir, float dmgOverride = -1)
    {
        if (dmgOverride > 0) damage = dmgOverride;
        rb.linearVelocity = dir.normalized * speed;
        lifeTimer = 0f;
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime) Despawn();
    }

    public void Despawn()
    {
        // 풀링 시스템이라 가정
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var hit = new ProjectileHitData(this, other);
        SendMessage("OnProjectileHit", hit, SendMessageOptions.DontRequireReceiver);
    }
}

public readonly struct ProjectileHitData
{
    public readonly ProjectileCore core;
    public readonly Collider2D target;
    public ProjectileHitData(ProjectileCore c, Collider2D t) { core = c; target = t; }
}