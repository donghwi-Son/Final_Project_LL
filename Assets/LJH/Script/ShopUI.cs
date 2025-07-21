using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform row1; 
    [SerializeField] Transform row2; 
    [SerializeField]private GameObject entryPrefab;
    [SerializeField]private Button rerollButton;
    [SerializeField]private GameObject ShopPanel;
    
    private bool shopOpen = false;
    private int slotCount = 5; 
    private int firstRowCount = 3;

    [Header("Fixed Probabilities")]
    public RarityChance[] rarityChances; // Inspector에서 ItemInfo.ItemRarity와 % 입력

    void Awake()
    {
        rerollButton.onClick.AddListener(GenerateShop);
    }

    void Start()
    {
        GenerateShop();
    }

    void OnDestroy()
    {
        rerollButton.onClick.RemoveListener(GenerateShop);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShopOpen();
            Debug.Log("상점오픈");
        }
    }

    private void ShopOpen()
    {
        ShopPanel.SetActive(!shopOpen);
        shopOpen = !shopOpen;
        Debug.Log(shopOpen+"오픈 함수");
    }
    

    void GenerateShop()
    {
        // 기존 삭제
        foreach (Transform t in row1) Destroy(t.gameObject);
        foreach (Transform t in row2) Destroy(t.gameObject);

        // 랜덤 아이템 뽑기
        var list = Enumerable.Range(0, slotCount)
            .Select(_ => GetRandomItemByRarity())
            .ToList();

        // 1행에 3개
        for (int i = 0; i < firstRowCount && i < list.Count; i++)
        {
            var go = Instantiate(entryPrefab, row1);
            go.GetComponent<ShopEntryUI>().Setup(list[i]);
        }

        // 2행 나머지
        for (int i = firstRowCount; i < list.Count; i++)
        {
            var go = Instantiate(entryPrefab, row2);
            go.GetComponent<ShopEntryUI>().Setup(list[i]);
        }
    }

    ItemDefinition GetRandomItemByRarity()
    {
        //퍼센트 기반 뽑기 로직
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

        var pool = ItemDatabase.Instance
            .GetDefinitionsByRarity(chosen)
            .ToList();
        if (pool.Count == 0) return ItemDatabase.Instance.GetAllDefinitions().First();
        return pool[Random.Range(0, pool.Count)];
    }
    
}