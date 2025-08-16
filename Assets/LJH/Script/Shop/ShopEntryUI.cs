using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopEntryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image    iconImage;
    public TMP_Text priceText;
    public Button   buyButton;
    
    [Header("Tooltip")]
    [SerializeField] private GameObject detailPanel;  
    [SerializeField] private TMP_Text   nameText;
    [SerializeField] private TMP_Text   descText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buySound;

    private ItemDefinition def;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Setup(ItemDefinition def, bool isBlackMarket)
    {
        this.def = def;
        iconImage.sprite = def.icon;
        
        int price = def.price;
        if (isBlackMarket)
            price = Mathf.CeilToInt(def.price * 0.7f);
        
        priceText.text = price.ToString();
        
        nameText.text    = def.name;
        descText.text    = def.description;
        detailPanel.SetActive(false);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuy(price));
    }

    private void OnBuy(int price)
    {
        // 획득
        if (PlayerGold.Instance.gold >= price)
        {
            PlayerGold.Instance.gold -= price;
            PlayerInventory.Instance.OnItemAcquired(def.index);
            ItemEffectFactory.ApplyEffect(def);
            audioSource.PlayOneShot(buySound);
            
            // 인벤토리 UI 새로고침
            if (InventoryUI.Instance != null)
                InventoryUI.Instance.Refresh();
        
            var c = iconImage.color;
            c.a = 0.5f;                  
            iconImage.color = c;

            //이중 구매 방지 비튼 비활성화
            buyButton.interactable = false;
        

            Debug.Log($"구매 완료: {def.name} (가격: {price})");
        }
        else
        {
            Debug.Log("보유 골드 부족");
        }
        
    }
        
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        detailPanel.SetActive(true);
    }

    // 마우스 벗어나면
    public void OnPointerExit(PointerEventData eventData)
    {
        detailPanel.SetActive(false);
    }
}