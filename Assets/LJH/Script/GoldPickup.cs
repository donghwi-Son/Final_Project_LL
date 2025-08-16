using System;
using System.Collections;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    private int value = 50;
    private bool canPickedUp = false;

    public void SetValue(int amount)
    {
        value = amount;
    }

    private void Start()
    {
        StartCoroutine(EnablePickupDelay());
    }

    private IEnumerator EnablePickupDelay()
    {
        yield return new WaitForSeconds(0.3f); // 드랍 후 0.3초 뒤부터 먹을 수 있음
        canPickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canPickedUp) return;

        if (other.CompareTag("Player"))
        {
            PlayerGold.Instance.gold += value;
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.CompareTag("Ground") || other.CompareTag("Wall")) && TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
    }
}