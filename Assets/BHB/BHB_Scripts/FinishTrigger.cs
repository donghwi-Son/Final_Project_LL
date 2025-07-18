using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private bool triggered = false;
    public Vector2Int direction; // ex) (0, 1) 위 / (-1, 0) 왼쪽 등 // 새로운 방 생성을 위한 부분
    // Top Finish	(0, 1)
    // Down Finish	(0, -1)
    // Left Finish	(-1, 0)
    // Right Finish	(1, 0)
    public bool isBoss = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        triggered = true;

        if (isBoss)
        {
            StageManager.Instance.SpawnBossRoom(); //보스방 생성
        }
        else
        {
            StageManager.Instance.TryMoveToDirection(direction);
        }
    }
}
