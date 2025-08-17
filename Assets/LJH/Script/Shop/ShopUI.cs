using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
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
    
    [Header("배경")]
    [SerializeField] private Image background;      
    [SerializeField] private Sprite normalBg;        
    [SerializeField] private Sprite blackMarketBg;   
    
    //일반 상점
    private bool shopOpen = false;
    private int slotCount = 5; 
    private int firstRowCount = 3;
    
    //암시장
    private bool isBlackMarket = false;
    private const float blackMarketChance = 0.1f;
    private const float blackMarketDiscountRate = 0.3f;

    [Header("확률 설정")]
    public RarityChance[] rarityChances; //등급와 % 입력
    
    [SerializeField]private AudioSource audioSource;
    [SerializeField]private AudioClip clip;
    
    private bool stockGenerated = false;

    void Awake()
    {
        rerollButton.onClick.AddListener(OnRerollButtonClicked);
    }

    void Start()
    {
        UpdateRerollUI();
        var data = StageManager.Instance.GetStageDataByInstance(gameObject);
    }

    void OnDestroy()
    {
        rerollButton.onClick.RemoveListener(OnRerollButtonClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (StoreMan.PlayerInside)
            {
                Debug.Log("상점오픈");
                ShopOpen();
            }
        }
    }

    private void ShopOpen()
    {
        ShopPanel.SetActive(!shopOpen);
        shopOpen = !shopOpen;
        audioSource.PlayOneShot(clip);
        Debug.Log(shopOpen+"오픈 함수");
        if (shopOpen)
        {
            if (!stockGenerated)
            {
                isBlackMarket = Random.value < blackMarketChance;
                ApplyBackgroundTheme();

                rerollCount = 0;
                UpdateRerollUI();

                GenerateShop();
                stockGenerated = true;
            }
            else
            {
                ApplyBackgroundTheme();
                UpdateRerollUI();
            }
        }
    }
    

    void GenerateShop()
    {
        foreach (Transform t in row1) Destroy(t.gameObject);
        foreach (Transform t in row2) Destroy(t.gameObject);

        var list = new List<ItemDefinition>();
        var attempted = new HashSet<int>();

        while (list.Count < slotCount)
        {
            var item = GetRandomItemByRarity();
            if (item == null) break;

            if (attempted.Contains(item.index)) continue; // 같은 아이템 두 번 추가 방지
            attempted.Add(item.index);

            list.Add(item);
        }

        for (int i = 0; i < firstRowCount && i < list.Count; i++)
        {
            var go = Instantiate(entryPrefab, row1);
            go.GetComponent<ShopEntryUI>().Setup(list[i], isBlackMarket);
        }

        for (int i = firstRowCount; i < list.Count; i++)
        {
            var go = Instantiate(entryPrefab, row2);
            go.GetComponent<ShopEntryUI>().Setup(list[i], isBlackMarket);
        }
    }


    ItemDefinition GetRandomItemByRarity()
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

        var pool = ItemDatabase.Instance
            .GetDefinitionsByRarity(chosen)
            .Where(def => !PlayerInventory.Instance.HasAcquired(def.index)) // 보유 아이템 제외
            .ToList();

        if (pool.Count == 0)
            return null;

        return pool[Random.Range(0, pool.Count)];
    }
    
    int GetRerollCost()
    {
        int rawCost = baseRerollCost * (1 << rerollCount);
        return Mathf.Min(rawCost, 800);
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
        stockGenerated = true;
    }
    
    void ApplyBackgroundTheme()
    {
        if (background == null) return;

        if (isBlackMarket)
        {
            background.sprite = blackMarketBg;
        }
        else
        {
            background.sprite = normalBg;
        }
    }
}