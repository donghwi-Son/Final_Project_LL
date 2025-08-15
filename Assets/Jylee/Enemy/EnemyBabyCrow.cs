using UnityEngine;

public class EnemyBabyCrow : Enemy
{
    [Header("이동")]
    public Transform playerTrans;            // 타겟
    public float rotationSpeed = 200f;  // 회전 속도 (deg/sec)

    protected override void Start()
    {
        if (playerTrans == null)
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();

        if (playerTrans == null) return;

        // 1. 플레이어 방향 계산
        Vector2 direction = (playerTrans.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // 2. 회전
        anim.transform.rotation = Quaternion.Euler(0, 0, EnemyPlayerGaze()-90);

        // 3. 현재 방향으로 이동
        rb.linearVelocity = direction * MoveSpeed;
    }

    public float EnemyPlayerGaze()
    {
        Vector2 direction = (new Vector3(playerTrans.position.x, playerTrans.position.y) - transform.position).normalized; // 플레이어 방향 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 각도로 변환

        return angle;
    }

    public override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }
}
