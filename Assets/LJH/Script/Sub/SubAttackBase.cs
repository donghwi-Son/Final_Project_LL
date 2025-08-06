using UnityEngine;
using System.Collections;

public abstract class SubAttackBase : MonoBehaviour
{
    [Header("Common")]
    public float cooldown = 1f;
    public Vector3 localOffset;
    public LayerMask enemyLayer;

    protected Transform owner;

    public virtual void Initialize(Transform _owner, float overrideCooldown = -1f)
    {
        owner = _owner;
        transform.SetParent(owner, false);
        transform.localPosition = localOffset;

        if (overrideCooldown > 0f) cooldown = overrideCooldown;

        StopAllCoroutines();
        StartCoroutine(AttackLoop());
        OnEquipped();
    }

    protected virtual void OnEquipped() { }
    protected virtual void OnUnequipped() { }

    private IEnumerator AttackLoop()
    {
        var wait = new WaitForSeconds(cooldown);
        while (true)
        {
            if (owner != null) Fire();
            yield return wait;
        }
    }

    /// <summary>각 서브어택 고유 발동 로직</summary>
    protected abstract void Fire();

    private void OnDestroy()
    {
        OnUnequipped();
    }
}