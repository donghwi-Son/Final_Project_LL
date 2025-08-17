using System.Collections.Generic;
using UnityEngine;

public class SubAttackSlot : MonoBehaviour
{
    [Header("장착 위치")]
    public Transform mountPoint;                  
    [SerializeField] private List<Transform> extraMountPoints; 

    private readonly List<SubAttackBase> equipped = new List<SubAttackBase>();

    public static SubAttackSlot Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    
    public SubAttackBase Equip(GameObject prefab, float overrideCooldown = -1f, bool uniquePerType = true)
    {
        if (prefab == null) return null;

        var go = Instantiate(prefab);
        var sub = go.GetComponent<SubAttackBase>();
        if (sub == null)
        {
            Debug.LogError("장착한 프리팹에 SubAttackBase 파생 컴포넌트가 없습니다.");
            Destroy(go);
            return null;
        }

        if (uniquePerType)
        {
            System.Type t = sub.GetType();
            foreach (var s in equipped)
            {
                if (s != null && s.GetType() == t)
                {
                    Debug.LogWarning($"이미 같은 타입의 서브가 장착되어 있습니다: {t.Name}");
                    Destroy(go);
                    return null;
                }
            }
        }
        
        Transform mp = FindFreeMountPoint() ?? (mountPoint != null ? mountPoint : transform);
        sub.Initialize(mp, overrideCooldown);

        equipped.Add(sub);
        return sub;
    }
    
    public void Unequip(SubAttackBase sub)
    {
        if (sub == null) return;
        if (equipped.Remove(sub))
            Destroy(sub.gameObject);
    }
    
    public void UnequipByType<T>() where T : SubAttackBase
    {
        for (int i = equipped.Count - 1; i >= 0; --i)
        {
            var s = equipped[i];
            if (s is T)
            {
                equipped.RemoveAt(i);
                if (s != null) Destroy(s.gameObject);
            }
        }
    }
    
    public void UnequipAll()
    {
        foreach (var s in equipped)
        {
            if (s != null) Destroy(s.gameObject);
        }
        equipped.Clear();
    }

    private Transform FindFreeMountPoint()
    {
        if (extraMountPoints == null || extraMountPoints.Count == 0) return null;
        foreach (var p in extraMountPoints)
        {
            if (p != null && p.childCount == 0)
                return p;
        }
        return null;
    }
}
