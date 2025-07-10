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
    
    public void Setup(ItemDefinition def)
    {
        iconImage.sprite   = def.icon;
        nameText.text      = def.name;
        descText.text      = def.description;
        detailPanel.SetActive(false);
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