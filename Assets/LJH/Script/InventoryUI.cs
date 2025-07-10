using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform content;        // Scroll View ▶ Content
    public GameObject     entryPrefab;   // Prefab/ItemEntry
    public GameObject     inventoryUI;
    private bool inventoryOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryOn();
        }
    }

    private void InventoryOn()
    {
        Refresh();
        inventoryUI.SetActive(!inventoryOpen);
        inventoryOpen = !inventoryOpen;
    }

    public void Refresh()
    {
        foreach (Transform t in content) Destroy(t.gameObject);

        // 획득 아이템 리스트 불러오기
        var list = PlayerTest.Instance.GetAllAcquired();
        if (list == null)
        {
            Debug.LogError("InventoryUI: PlayerTest.Instance 또는 GetAllAcquired()가 null입니다!");
            return;
        }
        foreach (var def in list)
        {
            var go = Instantiate(entryPrefab, content);
            // 아이콘 들고오기
            var img  = go.transform.Find("Icon").GetComponent<Image>();
            // 이름 들고오기
            var name = go.transform.Find("Name").GetComponent<TMP_Text>();
            // 설명 들고오기
            var desc = go.transform.Find("Desc").GetComponent<TMP_Text>();

            img.sprite    = def.icon;
            name.text     = def.name;
            desc.text     = def.description;
        }
    }
}