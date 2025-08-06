using UnityEngine;

// 그리드 기반 좌표 계산, Room 배치 좌표 유효성 확인 클래스
public class StageGenerator : MonoBehaviour
{
    [Header("그리드 설정")]
    public int rows = 16; // 세로 칸 수 (y)
    public int cols = 9;  // 가로 칸 수 (x)
    public Vector2 roomSize = new Vector2(0f, 0f); // 각 룸의 크기, 변경 가능
    public Vector3 origin = Vector3.zero; // 기준 시작 위치
    public float ySpawnOffsetFix = -10f; // 컴포넌트에서 보정값 조정 가능하게 공개 필드로 설정

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        // 그리드 전체 그리기
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 bottomLeft = origin + new Vector3(col * roomSize.x, row * roomSize.y, 0);
                Vector3 bottomRight = bottomLeft + new Vector3(roomSize.x, 0, 0);
                Vector3 topLeft = bottomLeft + new Vector3(0, roomSize.y, 0);
                Vector3 topRight = bottomLeft + new Vector3(roomSize.x, roomSize.y, 0);

                Gizmos.DrawLine(bottomLeft, bottomRight);
                Gizmos.DrawLine(bottomRight, topRight);
                Gizmos.DrawLine(topRight, topLeft);
                Gizmos.DrawLine(topLeft, bottomLeft);
            }
        }

        // 중심 좌표 시각화
        int centerCol = cols / 2;
        int centerRow = rows / 2;

        Vector3 centerPos = origin + new Vector3(centerCol * roomSize.x + roomSize.x / 2f,
                                                 centerRow * roomSize.y + roomSize.y / 2f,
                                                 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPos, Mathf.Min(roomSize.x, roomSize.y) * 0.2f);

    }

    // Random 좌표 전송 영역
    public Vector2Int GetRandomGridPosition()
    {
        int x = Random.Range(0, cols);
        int y = Random.Range(0, rows);
        return new Vector2Int(x, y);
    }

    // 아래 2개는 중심 좌표를 사용하기 위한 공개 메서드
    public Vector2Int GetCenterGrid()
    {
        return new Vector2Int(Mathf.FloorToInt(cols / 2f), Mathf.FloorToInt(rows / 2f));
    }

    public Vector3 GridToWorld(Vector2Int grid)
    {
        //Vector3 offset = new Vector3(roomSize.x / 2f, roomSize.y / 2f, 0f);
        //return origin + new Vector3(grid.x * roomSize.x, grid.y * roomSize.y, 0f) + offset;
        Vector3 basePos = origin + new Vector3(grid.x * roomSize.x, grid.y * roomSize.y, 0f);
        return basePos + new Vector3(0f, ySpawnOffsetFix, 0f); // Y축 보정 적용
    }

    // 그리드 내부에 방 제한
    public bool IsInsideGrid(Vector2Int grid)
    {
        return grid.x >= 0 && grid.x < cols && grid.y >= 0 && grid.y < rows;
    }
}
