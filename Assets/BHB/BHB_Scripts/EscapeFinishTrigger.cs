using UnityEngine;

public class EscapeFinishTrigger : MonoBehaviour
{
    public Vector2Int originGrid;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var stageManager = StageManager.Instance;
        Vector2Int fallback = originGrid;

        // 미방문 방 중 무작위 하나 선택
        foreach (var pair in stageManager.placedRooms)
        {
            if (!pair.Value.hasBeenVisited && pair.Value.type != StageType.Boss)
            {
                stageManager.MovePlayerTo(pair.Key);
                Debug.Log("[EscapeFinishTrigger] 탈출 성공 → 이동: " + pair.Key);
                return;
            }
        }

        Debug.LogWarning("[EscapeFinishTrigger] 미탐색 방이 없음. 원위치 유지");
        stageManager.MovePlayerTo(fallback);
    }
}
