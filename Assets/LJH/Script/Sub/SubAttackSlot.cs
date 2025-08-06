using UnityEngine;

public class SubAttackSlot : MonoBehaviour
{
    public Transform mountPoint;
    private SubAttackBase equipped;
    
    public static SubAttackSlot Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Equip(GameObject prefab, float overrideCooldown = -1f)
    {
        Unequip();

        var go = Instantiate(prefab);
        equipped = go.GetComponent<SubAttackBase>();
        if (equipped == null)
        {
            Debug.LogError("장착한 프리팹에 SubAttackBase 파생 컴포넌트가 없습니다.");
            Destroy(go);
            return;
        }

        equipped.Initialize(mountPoint ? mountPoint : transform, overrideCooldown);
    }

    public void Unequip()
    {
        if (equipped != null)
        {
            Destroy(equipped.gameObject);
            equipped = null;
        }
    }
}