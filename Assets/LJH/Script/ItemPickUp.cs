using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemIndex;
    private SpriteRenderer _sr;
    private float bounceHeight   = 2.5f;
    private float bounceDuration = 0.6f;
    private Vector3 _startPos;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        _startPos = transform.position;
    }

    void Start()
    {
        var def = ItemDatabase.Instance.GetDefinition(itemIndex);
        if (_sr != null && def.icon != null)
            _sr.sprite = def.icon;
        StartCoroutine(BounceY());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTest.Instance.OnItemAcquired(itemIndex);
            Destroy(gameObject);
        }
    }
    
    private IEnumerator BounceY()
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
    }
}
