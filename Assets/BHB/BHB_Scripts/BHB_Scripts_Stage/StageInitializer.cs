using System.Collections.Generic;
using UnityEngine;

// 게임 시작 시 전체 맵 구조를 사전 생성하는 초기화 클래스
public class StageInitializer : MonoBehaviour
{
    public int maxStageCount; // 매 런타임마다 생성될 맵의 수

    [Header("연결 참조")]
    public StageGenerator generator;
    public StageManager stageManager;

    public static Vector2Int FirstRoomGrid { get; private set; }

    private void Start()
    {
        Dictionary<Vector2Int, StageData> roomMap = GenerateMap();
        stageManager.maxStageCount = maxStageCount; // StageManager에 전달
        int startingStageCounter = 0; // StartRoom 포함
        stageManager.InitializeStages(roomMap, maxStageCount, startingStageCounter);
    }

    // 연결 기반 연속 방 배치를 위한 전면 수정
    // 성장형 생성 방식 적용
    // growthFrontier에서 랜덤 선택
    // 인접한 4방향 중 비어 있는 좌표를 후보로 등록
    // 유효한 곳이면 새 방 생성, map에 등록
    // 새 방의 좌표도 growthFrontier에 추가
    private Dictionary<Vector2Int, StageData> GenerateMap()
    {
        Dictionary<Vector2Int, StageData> map = new();
        List<Vector2Int> growthFrontier = new(); // 다음 후보 좌표 후보군

        Vector2Int center = generator.GetCenterGrid(); // (8, 4)
        StageData startRoom = new StageData(center.x, center.y, StageType.Start);
        startRoom.isStartRoom = true;
        map[center] = startRoom;
        growthFrontier.Add(center);

        FirstRoomGrid = center;

        int createdCount = 1; // Start 포함
        int maxAttempts = 1000;
        int attempts = 0;

        while (createdCount < maxStageCount && attempts < maxAttempts)
        {
            attempts++;

            // 현재 확장 가능한 방 중 하나 선택
            Vector2Int baseRoom = growthFrontier[Random.Range(0, growthFrontier.Count)];

            // 상하좌우 방향 순회
            Vector2Int[] directions = new Vector2Int[]
            {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
            };

            // 방향 셔플
            directions = Shuffle(directions);

            foreach (var dir in directions)
            {
                Vector2Int newPos = baseRoom + dir;

                if (!generator.IsInsideGrid(newPos)) continue;
                if (map.ContainsKey(newPos)) continue;

                // 보스 열 제외하고 제한
                if (newPos.x == 0 || newPos.x >= generator.cols - 1) continue;

                // 방 생성
                StageType type = GetRandomStageType();
                StageData newRoom = new StageData(newPos.x, newPos.y, type);
                map[newPos] = newRoom;
                growthFrontier.Add(newPos);
                createdCount++;
                break; // 한 개 만들고 나가기
            }
        }

        return map;
    }

    // 맵 셔플 메서드
    private T[] Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(i, array.Length);
            (array[i], array[rand]) = (array[rand], array[i]);
        }
        return array;
    }

    // 각 맵을 런타임마다 생성되는 확률을 다르게 주는 영역

    private StageType GetRandomStageType()
    {
        int rand = Random.Range(0, 10); // 랜덤 확률 조정

        if (rand < 6) return StageType.Normal;   // 60%
        else if (rand < 8) return StageType.Hard;   // 20%
        else if (rand == 8) return StageType.Store; // 10%
        else return StageType.Event;               // 10%
    }
}
