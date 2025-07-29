using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public Vector2Int direction; // ex: (0, 1), (1, 0) 등
    public bool isBoss = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Boss Finish만 Boss 방으로 이동
        if (isBoss)
        {
            if (StageManager.Instance.stageCounter >= StageManager.Instance.maxStageCounter)
            {
                StageManager.Instance.ActivateBossRoomIfReady();
                StageManager.Instance.MovePlayerToBossRoom();
            }
            return;
        }

        // 일반 Finish 처리
        Vector2Int current = StageManager.Instance.GetCurrentGrid();
        Vector2Int next = FindNextRoomInDirection(current, direction);

        if (next == current) return;

        StageData targetRoom = StageManager.Instance.GetRoomAt(next);
        if (targetRoom == null) return;

        if (targetRoom.type != StageType.Boss)
            StageManager.Instance.stageCounter++;

        StageManager.Instance.MovePlayerTo(next);
    }





    // 1칸 앞만이 아닌 직선 방향으로 방을 탐색하는 영역
    private Vector2Int FindNextRoomInDirection(Vector2Int from, Vector2Int dir)
    {
        Vector2Int check = from + dir;

        while (true)
        {
            StageData room = StageManager.Instance.GetRoomAt(check);
            if (room != null) return check;

            // 다음 칸으로 직진
            check += dir;

            // 그리드 벗어나는 경우 대비 (선택적 한계)
            if (check.x < 0 || check.x >= 100 || check.y < 0 || check.y >= 100)
                break;
        }

        return from; // 못 찾으면 원래 위치로 되돌림
    }

}
