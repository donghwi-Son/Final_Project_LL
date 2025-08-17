using UnityEngine;

// 각 Room의 메타 정보 및 연결 정보 저장 클래스
public class StageData
{
    public StageType type;

    // 방향 연결 (Up/Down/Left/Right)
    public StageData RightMap = null;
    public StageData LeftMap = null;
    public StageData UpMap = null;
    public StageData DownMap = null;

    // 그리드 좌표
    public int indexX;
    public int indexY;

    // 프리팹/인스턴스 (생성 후 할당됨)
    public GameObject prefab;
    public GameObject instance;

    // 시작 방 여부
    public bool isStartRoom = false;

    // 방문 여부 기록 추가
    public bool hasBeenVisited = false;

    public StageData(int x, int y, StageType type)
    {
        this.indexX = x;
        this.indexY = y;
        this.type = type;
    }

    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(indexX, indexY);
    }

    public void Connect(StageData neighbor, Vector2Int direction)
    {
        if (direction == Vector2Int.right) RightMap = neighbor;
        else if (direction == Vector2Int.left) LeftMap = neighbor;
        else if (direction == Vector2Int.up) UpMap = neighbor;
        else if (direction == Vector2Int.down) DownMap = neighbor;
    }

    public bool HasNeighbor(Vector2Int dir)
    {
        return dir switch
        {
            { x: 1, y: 0 } => RightMap != null,
            { x: -1, y: 0 } => LeftMap != null,
            { x: 0, y: 1 } => UpMap != null,
            { x: 0, y: -1 } => DownMap != null,
            _ => false
        };
    }
}