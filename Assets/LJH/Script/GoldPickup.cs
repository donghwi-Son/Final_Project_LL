using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    private int value = 50;

    public void SetValue(int amount)
    {
        value = amount;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerGold.Instance.gold += value;
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Structure") && TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero; // 남은 속도 제거
        }
    }
}