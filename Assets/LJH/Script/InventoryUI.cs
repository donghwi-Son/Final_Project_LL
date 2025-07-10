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

        var list = PlayerTest.Instance.GetAllAcquired();
        if (list == null)
        {
            Debug.LogError("InventoryUI: PlayerTest.Instance 또는 GetAllAcquired()가 null입니다!");
            return;
        }

        foreach (var def in list)
        {
            var go = Instantiate(entryPrefab, content);
            var entryUI = go.GetComponent<InventoryEntryUI>();
            entryUI.Setup(def);
        }
    }
}