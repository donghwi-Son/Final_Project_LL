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

    private ItemDefinition def;

    public void Setup(ItemDefinition def)
    {
        this.def = def;
        iconImage.sprite = def.icon;
        priceText.text = def.price.ToString();
        
        nameText.text    = def.name;
        descText.text    = def.description;
        detailPanel.SetActive(false);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuy);
    }

    private void OnBuy()
    {
        //아이템 획득
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