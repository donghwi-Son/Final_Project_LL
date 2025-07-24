using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Box : MonoBehaviour
{
    [Header("드롭 확률 설정")]
    public BoxConfig config;

    
    private Transform spawnPoint;
    
    [SerializeField] private GameObject itemPickupPrefab;
    
    private Box box;
    public static event Action<Box, int> OnBoxOpened;
    
    void Awake()
    {
        if(box == null) box = GetComponent<Box>();
    }

    public void Open()
    {
        // 1) 등급 뽑기
        ItemInfo.ItemRarity rarity = RollRarity();
        Debug.Log($"뽑힌 등급: {rarity}");
        
        var entry = config.rates.First(r => r.rarity == rarity);
        int reward = entry.goldReward;
        Debug.Log($"지급 보상: {reward} 골드");
        
        OnBoxOpened?.Invoke(this, reward);

        // 2) 해당 등급아이템 확인 이미 획득한 아이템은 제외
        var pool = ItemDatabase.Instance
            .GetAllDefinitions()
            .Where(d => d.rarity == rarity && !PlayerInventory.Instance.HasAcquired(d.index))
            .ToList();

        if (pool.Count == 0)
        {
            Debug.LogWarning($"{rarity} 등급 풀에 남은 아이템이 없습니다!");
            Destroy(gameObject);
            return;
        }

        // 3) 하나 랜덤 뽑기
        ItemDefinition chosen = pool[Random.Range(0, pool.Count)];
        Debug.Log($"획득 아이템: {chosen.name}");
        
        Vector3 spawnPos = (spawnPoint != null)
            ? spawnPoint.position
            : transform.position;

        var pickupObj = Instantiate(itemPickupPrefab, spawnPos, Quaternion.identity);
        var itemPickup = pickupObj.GetComponent<ItemPickup>();
        itemPickup.itemIndex = chosen.index;

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
            Debug.Log("플레이어 충돌");
            box.Open();
        }
    }
}