using System.Collections;
using UnityEngine;

public class GoldChest : MonoBehaviour
{
    public enum ChestSize { Small, Medium, Large }
    [SerializeField] private ChestSize size;

    [Header("세팅")]
    public GameObject goldPrefab;
    public Transform dropPoint;
    
    [Header("오디오")]
    [SerializeField] private AudioClip openSound;
    private AudioSource audioSource;
    
    [Header("이미지")]
    [SerializeField] private Sprite openedSprite;
    private SpriteRenderer sr;
    
    private bool isOpened = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetOpened()
    {
        if (sr != null && openedSprite != null)
            sr.sprite = openedSprite;
    }

    private void Open()
    {
        if (isOpened) return;
        isOpened = true;
        int amount = GetRandomGold();
        SpawnGold(amount);
        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);
        SetOpened();
        StartCoroutine(DestroyAfterDelay(3f));
    }

    int GetRandomGold()
    {
        int raw = size switch
        {
            ChestSize.Small  => Random.Range(50, 301),
            ChestSize.Medium => Random.Range(200, 1001),
            ChestSize.Large  => Random.Range(300, 3001),
            _ => 0
        };
        return raw / 50 * 50;
    }

    void SpawnGold(int totalAmount)
    {
        int unit = 50;
        int count = Mathf.Max(1, totalAmount / unit);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnOffset = (Vector3)Random.insideUnitCircle.normalized * Random.Range(0.5f, 1.5f);
            Vector3 spawnPos = dropPoint.position + spawnOffset;

            GameObject gold = Instantiate(goldPrefab, spawnPos, Quaternion.identity);
            gold.GetComponent<GoldPickup>().SetValue(unit);
            
            if (gold.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 forceDir = new Vector2(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                rb.AddForce(forceDir, ForceMode2D.Impulse);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Open();
        }
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}