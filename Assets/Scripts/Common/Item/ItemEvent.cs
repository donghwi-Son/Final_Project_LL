using UnityEngine;

public class ItemEvent : MonoBehaviour
{
    [SerializeField] private ItemPickupToast toast;
    
    void Reset()
    {
        if (toast == null) toast = GetComponentInChildren<ItemPickupToast>(true);
    }
    
    void OnEnable()
    {
        ItemEffectFactory.OnEffectApplied += HandleEffectApplied;
    }

    void OnDisable()
    {
        ItemEffectFactory.OnEffectApplied -= HandleEffectApplied;
    }

    private void HandleEffectApplied(int itemIndex)
    {
        var def = ItemDatabase.Instance.GetDefinition(itemIndex);
        if (def == null)
        {
            Debug.LogWarning($"토스트: ItemDefinition 없음 (index={itemIndex})");
            return;
        }

        if (toast != null) toast.Enqueue(itemIndex);
        else Debug.LogWarning("ItemEvent: ItemPickupToast 참조가 비어있음.");
    }
}
