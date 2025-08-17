using UnityEngine;

public class SubAttackAoE : SubAttackBase
{
    [Header("AoE")]
    public float radius = 10f;
    [SerializeField] private Transform muzzle;
    public int damage = 10;

    private readonly Collider2D[] hits = new Collider2D[48];
    
    private CharacterStats attacker;
    
    public PlayerStats Stats { get; private set; }
    
    protected override void OnEquipped()
    {
        
        var pc = owner ? owner.GetComponentInParent<PlayerController>() : null;
        attacker = pc ? pc.Stats : owner ? owner.GetComponentInParent<CharacterStats>() : null;

        if (!attacker)
            Debug.LogError($"{name}: 공격자(CharacterStats) 찾기 실패 - owner 경로 확인 필요", this);

        if (!muzzle) muzzle = transform;
    }

    protected override void Fire()
    {
        if (!attacker)
        {
            Debug.LogError($"{name}: attacker == null (OnEquipped에서 못 찾음)", this);
            return;
        }
        if (!muzzle)
        {
            Debug.LogError($"{name}: muzzle == null", this);
            return;
        }
        Vector3 pos = muzzle.position;
        var hits = Physics2D.OverlapCircleAll(pos, radius, enemyLayer);
        foreach (var col in hits)
        {
            if (!col) continue;
            
            if (col.TryGetComponent<CharacterStats>(out var enemyStats))
            {
                attacker.DoDamage(enemyStats);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        var center = Application.isPlaying && owner ? owner.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}