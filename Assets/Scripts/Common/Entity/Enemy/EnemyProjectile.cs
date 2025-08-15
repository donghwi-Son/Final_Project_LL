using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;      //미사일 속도
    [SerializeField] private float lifeTime = 3f;   //미사일 생존 시간
    private int damage;                             //미사일 대미지
    private Vector2 direction;                      //미사일 이동 방향
    private Rigidbody2D rb;                         //미사일 Rigidbody2D 컴포넌트

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime); //일정 시간 후 미사일 제거
    }

    public void Initialize(Vector2 _dir, int _damage)
    {
        direction = _dir.normalized;
        damage = _damage;

        if(direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, direction.y * Mathf.Rad2Deg); //왼쪽으로 발사 시 회전
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, direction.y * Mathf.Rad2Deg); //오른쪽으로 발사 시 회전
        }

        rb.linearVelocity = direction * speed; //미사일 속도 설정
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //여기에 플레이어 대미지 로직 추가

            PlayerStatus playerStatus = collision.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.TakeDamage(damage); //플레이어에게 대미지 적용
            }

            Destroy(gameObject);
        }
    }
}
