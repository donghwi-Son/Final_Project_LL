using UnityEngine;

public class GoldChest : MonoBehaviour
{
    public enum ChestSize { Small, Medium, Large }
    [SerializeField] private ChestSize size;

    [Header("Gold Drop Settings")]
    public GameObject goldPrefab;
    public Transform dropPoint;

    private void Open()
    {
        int amount = GetRandomGold();
        SpawnGold(amount);
        Destroy(gameObject); // 상자 파괴
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
        int roundedAmount = totalAmount / unit * unit;
        int count = Mathf.Max(1, roundedAmount / unit);

        for (int i = 0; i < count; i++)
        {
            GameObject gold = Instantiate(goldPrefab, dropPoint.position, Quaternion.identity);
            gold.GetComponent<GoldPickup>().SetValue(unit);
            Rigidbody2D rb = gold.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(new Vector2(Random.Range(-1f, 1f), 2f), ForceMode2D.Impulse);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Open();
        }
    }
}