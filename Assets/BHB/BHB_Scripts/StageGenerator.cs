using UnityEngine;
using System.Collections.Generic;

public class StageGenerator : MonoBehaviour
{
    [Header("공통 그리드 정보")]
    public int rows = 16; // 맵의 세로 칸 수
    public int cols = 9; // 맵의 가로 칸 수
    public Vector2 roomSize = new Vector2(28.8456f, 16.1808f); // 각 방의 크기, 직사각형으로 1칸씩
    public Vector3 origin = Vector3.zero; // 맵 시작 좌표

    [Header("마지막 방")]
    [SerializeField] private GameObject finishRoomObject_Normal; // 일반 방 Finish
    [SerializeField] private GameObject finishRoomObject_Hard; // 어려운 방 Finish
    private GameObject activeFinishRoomObject; // 현재 스테이지 타입에 맞는 Finish
    private Vector2Int finishRoomGridPos; // Finish 방의 위치는 가로는 고정하되 세로는 랜덤하게

    [Header("프리팹 생성 최대치 설정")]
    public int maxNormalSpawnCount; // 일반 방 프리팹 최대 생성 개수
    public int maxHardSpawnCount; // 어려운 방 프리팹 최대 생성 개수

    private List<(GameObject obj, string tag)> spawnedPrefabs = new(); // 생성된 프리팹 목록
    private List<Vector2Int> gridPool = new List<Vector2Int>(); // 타일을 배치할 수 있는 후보 위치 목록

    // 타일맵 생성, 방 배치, Finish 타일맵 생성 영역
    public void Spawn(StageType stageType)
    {
        ClearPrevious(); // 기존에 생성된 프리팹들 반환 

        // Finish 방 위치 랜덤 결정 (Col 고정 = 8, Row 랜덤)
        finishRoomGridPos = new Vector2Int(cols - 1, Random.Range(0, rows));

        // 선택된 타입에 따라 Finish 방 선택
        switch (stageType)
        {
            case StageType.Normal:
                activeFinishRoomObject = finishRoomObject_Normal;
                break;
            case StageType.Hard:
                activeFinishRoomObject = finishRoomObject_Hard;
                break;
            default:
                activeFinishRoomObject = null; // Store, Event, Start 등은 없음
                break;
        }

        // gridPool 생성, 생성 가능한 그리드 위치 초기화
        gridPool.Clear();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector2Int cell = new Vector2Int(col, row);

                if (cell == new Vector2Int(0, 0)) continue; // 고정 방 위치, (0,0)은 빼고
                if (cell == finishRoomGridPos) continue;  // Finish 방 위치
                gridPool.Add(cell);
            }
        }

        Shuffle(gridPool); // 위치 섞기

        // Noraml과 Hard를 공용 태그로 변경
        string poolTag = GetSharedTag(stageType);

        string GetSharedTag(StageType type)
        {
            switch (type)
            {
                case StageType.Normal:
                case StageType.Hard:
                    return "SharedRoom";
                default:
                    return null;
            }
        }

        int spawnCount = 0;

        switch (stageType)
        {
            case StageType.Normal:
                spawnCount = Mathf.Min(maxNormalSpawnCount, gridPool.Count);
                break;
            case StageType.Hard:
                spawnCount = Mathf.Min(maxHardSpawnCount, gridPool.Count);
                break;
            default:
                return; // Start, Boss, Store, Event 등 제외
        }

        // 일반 방을 랜덤하게 생성
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2Int gridPos = gridPool[i];
            float xOffsetFix = -4.5f;
            float zFix = Camera.main.transform.position.z;
            Vector3 spawnPos = origin
                + new Vector3(gridPos.x * roomSize.x, gridPos.y * roomSize.y, 0f) // Z축은 0으로 고정
                + new Vector3(roomSize.x * 0.5f + xOffsetFix, roomSize.y * 0.5f, 0);

            spawnPos.z = 0f; 


            GameObject obj = StagePoolManager.Instance.GetFromPool(poolTag);
            if (obj == null)
            {
                Debug.LogWarning($"[StageGenerator] Failed to get object from pool for tag: {poolTag}");
                continue;
            }

            obj.transform.position = spawnPos;
            obj.transform.rotation = Quaternion.identity;
            spawnedPrefabs.Add((obj, poolTag));

            // Finish 방 위치 계산해서 생성
            if (activeFinishRoomObject != null)
            {
                Vector3 finishWorldPos = origin
                    + new Vector3(finishRoomGridPos.x * roomSize.x, finishRoomGridPos.y * roomSize.y, 0)
                    + new Vector3(roomSize.x * 0.5f + xOffsetFix, roomSize.y * 0.5f, 0);
                finishWorldPos.z = 10f;
                activeFinishRoomObject.transform.position = finishWorldPos;
                activeFinishRoomObject.SetActive(true);
            }
        }
    }

    // 프리팹 초기화 후 반환 영역
    public void ClearPrevious()
    {
        foreach (var (obj, tag) in spawnedPrefabs)
        {
            if (obj != null)
            {
                StagePoolManager.Instance.ReturnToPool(tag, obj);
            }
        }
        spawnedPrefabs.Clear();
    }

    // 프리팹 좌표 섞기 영역
    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // 직사각형 모양의 방 칸 생성
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

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
    }
}
