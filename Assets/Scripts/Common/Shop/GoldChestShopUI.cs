using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GoldChestShopUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button smallChestButton;
    [SerializeField] private Button mediumChestButton;
    [SerializeField] private Button largeChestButton;
    [SerializeField] private Image smallChestImage;
    [SerializeField] private Image mediumChestImage;
    [SerializeField] private Image largeChestImage;

    [Header("상자 프리펩")]
    [SerializeField] private GameObject smallChestPrefab;
    [SerializeField] private GameObject mediumChestPrefab;
    [SerializeField] private GameObject largeChestPrefab;
    
    private Transform player => PlayerManager.Instance.player.transform;

    [SerializeField] private GameObject panel;
    
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
                panel.SetActive(!panel.activeSelf);
                AudioManager.Instance.PlaySFX(SFX.unhun);
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

        smallChestButton.interactable = false;
        var color = smallChestImage.color;
        color.a = 0.5f;
        smallChestImage.color = color;
        mediumChestButton.interactable = false;
        mediumChestImage.color = color;
        largeChestButton.interactable = false;
        largeChestImage.color = color;
        
        StartCoroutine(Reset());
        AudioManager.Instance.PlaySFX(SFX.SpendMoney);
    }
    
    private float GetDropYOffset(GameObject prefab)
    {
        if (prefab == smallChestPrefab)  return smallYOffset;
        if (prefab == mediumChestPrefab) return mediumYOffset;
        if (prefab == largeChestPrefab)  return largeYOffset;
        
        return smallYOffset;
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(30f);
        smallChestButton.interactable = true;
        mediumChestButton.interactable = true;
        largeChestButton.interactable = true;
        var color = smallChestImage.color;
        color.a = 1f;
        smallChestImage.color = color;
        mediumChestImage.color = color;
        largeChestImage.color = color;
    }
}