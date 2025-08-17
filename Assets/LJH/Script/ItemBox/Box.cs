using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Box : MonoBehaviour
{
    [Header("드롭 확률 설정")]
    public BoxConfig config;

    
    private Transform spawnPoint;
    
    [Header("아이템 프리펩")]
    [SerializeField] private GameObject itemPickupPrefab;
    
    [Header("골드 프리펩")]
    [SerializeField] private GameObject goldPrefab;
    
    private Box box;
    public static event Action<Box, int> OnBoxOpened;
    
    private bool isOpened = false;
    
    void Awake()
    {
        if(box == null) box = GetComponent<Box>();
    }

    public void Open()
    {
        if (isOpened) return;
        isOpened = true;
        
        // 1) 등급 뽑기
        ItemInfo.ItemRarity rarity = RollRarity();
        Debug.Log($"뽑힌 등급: {rarity}");
    
        var entry = config.rates.First(r => r.rarity == rarity);
        int reward = entry.goldReward;
        Debug.Log($"골드 드랍: {reward}");
        
        AudioManager.Instance.PlaySFX(SFX.BoxOpen);

        var boxAni = GetComponent<BoxAni>();
        if (boxAni != null) boxAni.SetOpened();

        // 골드 드랍 추가
        SpawnGold(reward);

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

        Vector3 spawnPos = (spawnPoint != null) ? spawnPoint.position : transform.position;

        var pickupObj = Instantiate(itemPickupPrefab, spawnPos, Quaternion.identity);
        var itemPickup = pickupObj.GetComponent<ItemPickup>();
        itemPickup.itemIndex = chosen.index;

        StartCoroutine(DestroyAfterDelay(3f));
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
    
    void SpawnGold(int totalAmount)
    {
        int unit = 50;
        int roundedAmount = totalAmount / unit * unit;
        int count = Mathf.Max(1, roundedAmount / unit);
        Vector3 dropPos = (spawnPoint != null) ? spawnPoint.position : transform.position;

        for (int i = 0; i < count; i++)
        {
            GameObject gold = Instantiate(goldPrefab, dropPos, Quaternion.identity);
            var goldPickup = gold.GetComponent<GoldPickup>();
            goldPickup.SetValue(unit);

            if (gold.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.AddForce(new Vector2(Random.Range(-1f, 1f), 2f), ForceMode2D.Impulse);
            }
        }
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}