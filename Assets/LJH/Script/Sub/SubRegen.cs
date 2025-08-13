using UnityEngine;

public class SubRegen : SubAttackBase
{
    [Header("회복 설정")]
    [SerializeField] private int healAmount = 1; // 틱당 회복량

    private CharacterStats stats;

    protected override void OnEquipped()
    {
        if (PlayerStatus.Instance != null)
        {
            stats = PlayerStatus.Instance;
            return;
        }
        
        var root = owner != null ? owner : transform.parent;
        if (root != null)
            stats = root.GetComponentInParent<CharacterStats>();

        if (stats == null)
            Debug.LogWarning("[Sub_RegenHP] CharacterStats를 찾지 못해 회복이 적용되지 않습니다.");
    }

    protected override void Fire()
    {
        if (stats == null) return;
        
        int max = stats.maxHealth.GetValue();
        if (stats.currentHealth >= max) return;
        stats.currentHealth = Mathf.Min(stats.currentHealth + healAmount, max);
    }

    protected override void OnUnequipped() { }
}
