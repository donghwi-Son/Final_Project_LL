using System;
using UnityEngine;
using UnityEngine.UI;

public class GoldChestShopUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button smallChestButton;
    [SerializeField] private Button mediumChestButton;
    [SerializeField] private Button largeChestButton;

    [Header("상자 프리펩")]
    [SerializeField] private GameObject smallChestPrefab;
    [SerializeField] private GameObject mediumChestPrefab;
    [SerializeField] private GameObject largeChestPrefab;

    [Header("플레이어 위치")]
    [SerializeField] private Transform player;

    [SerializeField] private GameObject panel;
    
    [SerializeField]private AudioSource audioSource;
    [SerializeField]private AudioClip clip;
    
    private float smallYOffset  = 0.40f;
    private float mediumYOffset = 0.05f;
    private float largeYOffset = 0.0001f;
        
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
            
            if (StoreMan.PlayerInside)
            {
                audioSource.PlayOneShot(clip);
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

        float yOff = GetDropYOffset(chestPrefab);
        Vector3 dropPosition = player.position + player.right * 5f + Vector3.down * yOff;
        Instantiate(chestPrefab, dropPosition, Quaternion.identity);
    }
    
    private float GetDropYOffset(GameObject prefab)
    {
        if (prefab == smallChestPrefab)  return smallYOffset;
        if (prefab == mediumChestPrefab) return mediumYOffset;
        if (prefab == largeChestPrefab)  return largeYOffset;
        
        return smallYOffset;
    }
}