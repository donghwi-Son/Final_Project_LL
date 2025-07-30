using UnityEngine;

// 플레이어가 처음 시작 Room에서 진입할 때 한 번만 트리거됨
public class StartStageFinishTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        Invoke(nameof(EnterFirstRoom), 0.1f);
    }

    private void EnterFirstRoom()
    {
        // 최초 룸 좌표 획득
        Vector2Int firstRoomGrid = StageInitializer.FirstRoomGrid;

        // StageManager가 해당 위치로 플레이어 이동 처리
        StageManager.Instance.MovePlayerTo(firstRoomGrid);
    }
}
