using System;
using System.Linq;
using TMPro;
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
    
    [Header("리롤")]
    [SerializeField] private TMP_Text rerollCostText; 
    private int rerollCount = 0;
    private const int baseRerollCost = 50;
    
    private bool shopOpen = false;
    private int slotCount = 5; 
    private int firstRowCount = 3;

    [Header("Fixed Probabilities")]
    public RarityChance[] rarityChances; //등급와 % 입력

    void Awake()
    {
        rerollButton.onClick.AddListener(OnRerollButtonClicked);
    }

    void Start()
    {
        UpdateRerollUI();
        GenerateShop();
    }

    void OnDestroy()
    {
        rerollButton.onClick.RemoveListener(OnRerollButtonClicked);
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
    
    int GetRerollCost()
    {
        return baseRerollCost * (1 << rerollCount);
    }

    void UpdateRerollUI()
    {
        if (rerollCostText != null)
            rerollCostText.text = $"{GetRerollCost()}";
    }
    
    private void OnRerollButtonClicked()
    {
        int cost = GetRerollCost();
        
        if (PlayerGold.Instance.gold < cost)
        {
            Debug.Log("리롤 골드 부족");
            return; 
        }
        
        PlayerGold.Instance.gold -= cost;
        rerollCount++;
        
        UpdateRerollUI();
        GenerateShop();
    }
    
}