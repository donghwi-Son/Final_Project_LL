using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private HashSet<int> acquired = new HashSet<int>();

    private List<ItemDefinition> acquiredDefs = new List<ItemDefinition>();

    public static PlayerInventory Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnItemAcquired(int idx)      //나중에 플레이어에 넣어야 하는 부분 아이템 획득 적용하는 부분
    {
        if (!acquired.Add(idx))
            return;

        // 정의 꺼내서 효과 적용
        var def = ItemDatabase.Instance.GetDefinition(idx);
        acquiredDefs.Add(def);
        ItemEffectFactory.ApplyEffect(def);
        SynergyManager.Instance.OnItemAcquired(def);

        // 획득 목록 저장
        SaveAcquiredSet();
    }


    private void SaveAcquiredSet()
    {
        // PlayerPrefs 방식으로 acquired 저장, 저장 필요 없으면 안할듯
    }

    public IReadOnlyList<ItemDefinition> GetAllAcquired()   //모든 아이템
    {
        return acquiredDefs;
    }

    public bool HasAcquired(int idx)                        //획득한 아이템
    {
        return acquired.Contains(idx);
    }
}
