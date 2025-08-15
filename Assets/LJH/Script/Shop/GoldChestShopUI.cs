using System;
using UnityEngine;
using UnityEngine.UI;

public class GoldChestShopUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button smallChestButton;
    [SerializeField] private Button mediumChestButton;
    [SerializeField] private Button largeChestButton;

    [Header("Chest Prefabs")]
    [SerializeField] private GameObject smallChestPrefab;
    [SerializeField] private GameObject mediumChestPrefab;
    [SerializeField] private GameObject largeChestPrefab;

    [Header("Player Reference")]
    [SerializeField] private Transform player; // 플레이어 위치

    [SerializeField] private GameObject panel;
    private void Awake()
    {
        smallChestButton.onClick.AddListener(() => TryBuyChest(100, smallChestPrefab));
        mediumChestButton.onClick.AddListener(() => TryBuyChest(300, mediumChestPrefab));
        largeChestButton.onClick.AddListener(() => TryBuyChest(500, largeChestPrefab));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"[ShopUI] P pressed. inside={StoreMan.Instance.PlayerInside}");
            if (StoreMan.Instance.PlayerInside)
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
    }

    private void TryBuyChest(int cost, GameObject chestPrefab)
    {
        if (PlayerGold.Instance.gold < cost)
        {
            Debug.Log("골드 부족!");
            return;
        }

        PlayerGold.Instance.gold -= cost;

        Vector3 dropPosition = player.position + player.right * 1.5f + Vector3.up * 0.5f;
        Instantiate(chestPrefab, dropPosition, Quaternion.identity);
    }
}