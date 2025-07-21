using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform       content;
    public GameObject          entryPrefab;
    public Button              rerollButton;
    public int                 slotCount = 5;

    [Header("Fixed Probabilities")]
    public RarityChance[]      rarityChances; // Inspector에서 ItemInfo.ItemRarity와 % 입력

    void OnEnable()
    {
        rerollButton.onClick.AddListener(GenerateShop);
        GenerateShop();
    }

    private void GenerateShop()
    {
        // 기존 슬롯 삭제
        foreach (Transform t in content) Destroy(t.gameObject);

        for (int i = 0; i < slotCount; i++)
        {
            var def = GetRandomItemByRarity();
            var go  = Instantiate(entryPrefab, content);
            go.GetComponent<ShopEntryUI>().Setup(def);
        }
    }

    private ItemDefinition GetRandomItemByRarity()
    {
        int rnd = Random.Range(0, 100);
        int cum = 0;
        ItemInfo.ItemRarity chosen = rarityChances[0].rarity;

        foreach (var rc in rarityChances)
        {
            cum += rc.percentage;
            if (rnd < cum)
            {
                chosen = rc.rarity;
                break;
            }
        }

        // 해당 등급 아이템 리스트 얻기
        var pool = ItemDatabase.Instance
            .GetAllDefinitions()
            .Where(d => d.rarity == chosen)
            .ToList();

        if (pool.Count == 0)
        {
            Debug.LogWarning($"{chosen} 등급 아이템 풀이 비어있습니다!");
            return ItemDatabase.Instance.GetAllDefinitions().First();
        }

        return pool[Random.Range(0, pool.Count)];
    }
}