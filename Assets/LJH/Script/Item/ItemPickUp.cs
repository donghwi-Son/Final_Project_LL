using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemIndex;
    private SpriteRenderer _sr;
    private Collider2D _col2D;
    private float bounceHeight   = 2.5f;
    private float bounceDuration = 0.4f;
    private Vector3 _startPos;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        _col2D   = GetComponent<Collider2D>();
        _startPos = transform.position;
    }

    void Start()
    {
        if (_col2D != null) _col2D.enabled = false;
        var def = ItemDatabase.Instance.GetDefinition(itemIndex);
        if (_sr != null && def.icon != null) _sr.sprite = def.icon;
        StartCoroutine(BounceY());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory.Instance.OnItemAcquired(itemIndex);
            InventoryUI.Instance.Refresh();
            Destroy(gameObject);
        }
    }
    
    private IEnumerator BounceY()   //아이템 스폰 후 움직임
    {
        float halfTime = bounceDuration * 0.5f;
        Vector3 peakPos = _startPos + Vector3.up * bounceHeight;
        
        for (float t = 0; t < halfTime; t += Time.deltaTime)
        {
            float p = t / halfTime;
            transform.position = Vector3.Lerp(_startPos, peakPos, p);
            yield return null;
        }
        
        for (float t = 0; t < halfTime; t += Time.deltaTime)
        {
            float p = t / halfTime;
            transform.position = Vector3.Lerp(peakPos, _startPos, p);
            yield return null;
        }
        
        transform.position = _startPos;
        
        if (_col2D != null) _col2D.enabled = true;
    }
}
