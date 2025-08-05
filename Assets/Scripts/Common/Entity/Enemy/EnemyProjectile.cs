using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;      //미사일 속도
    [SerializeField] private float lifeTime = 3f;   //미사일 생존 시간
    [SerializeField] private int damage = 10;       //미사일 대미지
    private Vector2 direction;                      //미사일 이동 방향

    private void Start()
    {
        Destroy(gameObject, lifeTime); //일정 시간 후 미사일 제거
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //여기에 플레이어 대미지 로직 추가
            
            Destroy(gameObject);
        }
    }
}
