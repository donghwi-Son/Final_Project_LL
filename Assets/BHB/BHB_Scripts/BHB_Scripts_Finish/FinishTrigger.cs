using UnityEngine;

// 방 끝 포탈. 플레이어가 닿으면 다음 방으로 이동 클래스
public class FinishTrigger : MonoBehaviour
{
    public Vector2Int direction;  // ex: (0, 1), (1, 0)
    public bool isBoss = false;
    public bool isReturning = false;  // 새로 추가

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 보스 이동 조건
        if (isBoss && StageManager.Instance.stageCounter >= StageManager.Instance.maxStageCounter)
        {
            StageManager.Instance.ActivateBossRoomIfReady();
            StageManager.Instance.MovePlayerToBossRoom();
            StageManager.Instance.MoveToBossRoomCleanTransition();
            return;
        }

        Vector2Int current = StageManager.Instance.GetCurrentGrid();
        Vector2Int next = FindNextRoomInDirection(current, direction);
        StageData nextRoom = StageManager.Instance.GetRoomAt(next);

        if (next == current) return;

        StageData targetRoom = StageManager.Instance.GetRoomAt(next);
        if (targetRoom == null) return;

        // 최초 방문만 stageCounter++
        if (!isReturning && targetRoom.type != StageType.Boss && !targetRoom.hasBeenVisited)
        {
            StageManager.Instance.stageCounter++;
        }

        if (nextRoom != null && nextRoom.type == StageType.Start)
        {
            return;
        }

        StageManager.Instance.MovePlayerTo(next);
    }

    // 직선 방향 방 찾기
    private Vector2Int FindNextRoomInDirection(Vector2Int from, Vector2Int dir)
    {
        Vector2Int check = from + dir;

        while (true)
        {
            StageData room = StageManager.Instance.GetRoomAt(check);
            if (room != null) return check;

            check += dir;

            if (check.x < 0 || check.x >= 100 || check.y < 0 || check.y >= 100)
                break;
        }

        return from;
    }
}