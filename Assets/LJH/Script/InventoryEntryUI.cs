using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryEntryUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image      iconImage;    // Icon 자식
    [SerializeField] private GameObject detailPanel;  // Name+Desc 묶음
    [SerializeField] private TMP_Text   nameText;     // Name
    [SerializeField] private TMP_Text   descText;     // Desc

    /// <summary>
    /// 외부에서 아이템 데이터를 세팅할 때 호출
    /// </summary>
    public void Setup(ItemDefinition def)
    {
        iconImage.sprite   = def.icon;
        nameText.text      = def.name;
        descText.text      = def.description;
        detailPanel.SetActive(false);  // 초기엔 감춤
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        detailPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        detailPanel.SetActive(false);
    }
}