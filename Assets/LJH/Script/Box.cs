using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Box : MonoBehaviour
{
    [Header("드롭 확률 설정")]
    public BoxConfig config;

    [Header("드랍 후 아이템 스폰 위치")]
    public Transform spawnPoint;
    
    private Box box;

    void Awake()
    {
        if(box == null) box = GetComponent<Box>();
    }

    public void Open()
    {
        // 1) 등급 뽑기
        ItemInfo.ItemRarity rarity = RollRarity();
        Debug.Log($"뽑힌 등급: {rarity}");

        // 2) 해당 등급아이템 확인 이미 획득한 아이템은 제외
        var pool = ItemDatabase.Instance
            .GetAllDefinitions()
            .Where(d => d.rarity == rarity && !PlayerTest.Instance.HasAcquired(d.index))
            .ToList();

        if (pool.Count == 0)
        {
            Debug.LogWarning($"{rarity} 등급 풀에 남은 아이템이 없습니다!");
            return;
        }

        // 3) 하나 랜덤 뽑기
        ItemDefinition chosen = pool[Random.Range(0, pool.Count)];
        Debug.Log($"획득 아이템: {chosen.name}");

        // 4) 획득
        PlayerTest.Instance.OnItemAcquired(chosen.index);

        // 5) 직접스폰
        // Instantiate(itemPickupPrefab, spawnPoint.position, Quaternion.identity)
        //     .GetComponent<ItemPickup>().itemIndex = chosen.index;

        Destroy(gameObject);
    }

    ItemInfo.ItemRarity RollRarity()
    {
        float r = Random.value * 100f;
        float cum = 0f;
        foreach (var rr in config.rates)
        {
            cum += rr.rate;
            if (r < cum)
                return rr.rarity;
        }
        // 혹시 합이 100이 안 되면 마지막 등급 리턴
        return config.rates.Last().rarity;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            box.Open();
            Debug.Log("플레이어 충돌");
        }
    }
}