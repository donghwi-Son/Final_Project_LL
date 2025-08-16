using UnityEngine;

public class AttackHitBoxBase : MonoBehaviour
{
    private CharacterStats parentsStats;

    void Start()
    {
        parentsStats = GetComponentInParent<CharacterStats>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어 데미지 로직
            parentsStats.DoDamage(collision.GetComponent<PlayerStatus>());
        }
    }
}
