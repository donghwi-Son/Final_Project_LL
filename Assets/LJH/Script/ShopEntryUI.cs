using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public Image    iconImage;
    public TMP_Text priceText;
    public Button   buyButton;

    private ItemDefinition def;

    public void Setup(ItemDefinition def)
    {
        this.def = def;
        iconImage.sprite = def.icon;
        priceText.text    = def.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        //buyButton.onClick.AddListener(OnBuy);
    }

    // private void OnBuy()
    // {
    //     if (CurrencyManager.Instance.CanAfford(def.price))
    //     {
    //         CurrencyManager.Instance.Spend(def.price);
    //         PlayerTest.Instance.OnItemAcquired(def.index);
    //         buyButton.interactable = false;
    //     }
    //     else
    //     {
    //         Debug.Log("골드가 부족합니다!");
    //     }
    // }
}