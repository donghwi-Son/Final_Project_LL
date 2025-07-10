using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemIndex;
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        var def = ItemDatabase.Instance.GetDefinition(itemIndex);
        if (_sr != null && def.icon != null)
            _sr.sprite = def.icon;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerController.Instance.OnItemAcquired(itemIndex);
            Destroy(gameObject);
        }
    }
}
