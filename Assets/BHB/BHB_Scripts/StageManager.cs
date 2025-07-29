using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("프리팹 설정")]
    public GameObject startRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject[] hardRoomPrefabs;
    public GameObject[] storeRoomPrefabs;
    public GameObject[] eventRoomPrefabs;
    public GameObject bossRoomPrefab;

    [Header("연결 참조")]
    public StageGenerator generator;
    public MiniMapManager miniMapManager;
    public Transform player;

    [Header("스테이지 진행 정보")]
    public int maxStageCount;         // 방 생성 총 수량 (초기화용)

    [Header("보스 진입 조건")]
    public int maxStageCounter = 10; // 플레이어가 몇 방을 지나야 Boss Finish가 열리는가

    [Header("플레이어 진행도")]
    public int stageCounter = 0;
    private StageData bossRoomData; // 클래스 필드로 보관하는 보스 방 데이터

    public Dictionary<Vector2Int, StageData> placedRooms = new();
    private Vector2 roomSize;
    private Vector3 origin;

    private Vector2Int currentGrid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeStages(Dictionary<Vector2Int, StageData> stageMap, int maxStageCountFromInitializer, int startingStageCounter)
    {
        placedRooms = stageMap;
        roomSize = generator.roomSize;
        origin = generator.origin;
        maxStageCount = maxStageCountFromInitializer;
        stageCounter = startingStageCounter;

        foreach (var pair in placedRooms)
        {
            Vector2Int grid = pair.Key;
            StageData data = pair.Value;

            GameObject prefab = GetPrefabByType(data.type);
            if (prefab == null)
            {
                Debug.LogError($"[{data.type}] 프리팹이 설정되지 않았습니다.");
                continue;
            }

            Vector3 worldPos = generator.GridToWorld(grid);

            // 1. Instantiate Room
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
            instance.SetActive(false); // 모두 비활성화
            data.instance = instance;
            data.prefab = prefab;

            // 2. StartRoom만 예외적으로 Finish 2개 활성화
            if (data.isStartRoom)
            {
                SetStartRoomFinishTwoDirections(data.instance);
            }
            else
            {
                // 3. 나머지 방은 Finish 전부 비활성화
                Transform finishGroup = data.instance.transform.Find("Finish Group");
                if (finishGroup != null)
                {
                    foreach (Transform child in finishGroup)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }

        // 연결 처리
        RoomConnector.ProcessConnections(placedRooms, generator, maxStageCount, stageCounter);

        // 보스 방 미리 생성
        PreloadBossRoom();
    }

    public void MoveToStartRoom()
    {
        foreach (var pair in placedRooms)
        {
            if (pair.Value.isStartRoom)
            {
                MovePlayerTo(pair.Key);
                return;
            }
        }

        Debug.LogError("[StageManager] 시작 Room이 지정되지 않았습니다.");
    }

    public void MovePlayerTo(Vector2Int nextGrid)
    {
        if (!placedRooms.ContainsKey(nextGrid))
        {
            Debug.LogWarning("[StageManager] 존재하지 않는 Room으로 이동 시도됨");
            return;
        }

        Vector2Int prevGrid = currentGrid;

        foreach (var room in placedRooms.Values)
        {
            if (room.instance != null)
                room.instance.SetActive(false);
        }

        currentGrid = nextGrid;

        StageData roomData = placedRooms[nextGrid];
        if (roomData.instance != null)
            roomData.instance.SetActive(true);

        // === backDir을 먼저 계산 ===
        Vector2Int? backDir = null;
        if (prevGrid != Vector2Int.zero && prevGrid != nextGrid)
        {
            Vector2Int fromDir = nextGrid - prevGrid;
            if (Mathf.Abs(fromDir.x) + Mathf.Abs(fromDir.y) == 1)
            {
                backDir = new Vector2Int(-fromDir.x, -fromDir.y);
            }
        }

        // === Finish Group 먼저 선언 ===
        Transform finishGroup = roomData.instance.transform.Find("Finish Group");

        // === SpawnPoint 설정 ===
        Transform spawn = null;

        // 먼저 finishGroup 정의는 위에서 했다고 가정
        if (backDir.HasValue && finishGroup != null)
        {
            string entryFinishName = GetFinishName(backDir.Value, false);
            Transform entryFinish = finishGroup.Find(entryFinishName);

            if (entryFinish != null)
            {
                // 방향 기반 정확한 포인트 사용
                spawn = entryFinish;
            }
        }

        // fallback: SpawnPoint 오브젝트 또는 방 중심
        if (spawn == null)
        {
            spawn = roomData.instance.transform.Find("SpawnPoint");
        }

        player.position = spawn != null ? spawn.position : roomData.instance.transform.position;


        // === 미니맵 처리 ===
        MiniMapManager.instance?.HighlightIcon(nextGrid);
        MiniMapManager.instance?.RevealRoom(nextGrid, roomData.type);
        MiniMapManager.instance?.HighlightIcon(nextGrid);

        roomData.hasBeenVisited = true;

        // === Start Room이면 포탈 비활성 처리 안함 ===
        if (roomData.type == StageType.Start)
            return;

        // === Boss 조건 우선 판단 ===
        if (stageCounter >= maxStageCounter)
        {
            Debug.Log($"[StageManager] Boss 조건 도달! 현재 {stageCounter}, 최대 {maxStageCounter}");

            if (finishGroup != null)
                foreach (Transform child in finishGroup) child.gameObject.SetActive(false);

            Transform bossGroup = roomData.instance.transform.Find("Boss Finish Group");
            if (bossGroup != null)
            {
                foreach (Transform child in bossGroup) child.gameObject.SetActive(false);
                Transform bossFinish = bossGroup.Find("Boss Finish 1");
                if (bossFinish != null)
                {
                    bossFinish.gameObject.SetActive(true);
                    var trigger = bossFinish.GetComponent<FinishTrigger>();
                    if (trigger != null)
                    {
                        trigger.direction = Vector2Int.zero;
                        trigger.isBoss = true;
                    }

                    Debug.Log("[StageManager] Boss Finish 1 활성화 완료 at " + nextGrid);
                }
            }

            var escapeGroup = roomData.instance.transform.Find("Escape Finish Group");
            if (escapeGroup != null)
                foreach (Transform child in escapeGroup) child.gameObject.SetActive(false);

            return;
        }

        // === 일반 포탈 활성화 ===
        if (finishGroup != null)
        {
            foreach (Transform child in finishGroup) child.gameObject.SetActive(false);

            List<Vector2Int> candidateDirs = new();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                if (backDir.HasValue && dir == backDir.Value) continue;

                Vector2Int neighborGrid = nextGrid + dir;
                if (placedRooms.TryGetValue(neighborGrid, out var neighbor) && !neighbor.hasBeenVisited)
                {
                    candidateDirs.Add(dir);
                }
            }

            // 보충
            if (candidateDirs.Count < 2)
            {
                foreach (var dir in directions)
                {
                    if (backDir.HasValue && dir == backDir.Value) continue;
                    if (candidateDirs.Contains(dir)) continue;

                    Vector2Int neighborGrid = nextGrid + dir;
                    if (placedRooms.ContainsKey(neighborGrid))
                    {
                        candidateDirs.Add(dir);
                        if (candidateDirs.Count == 2) break;
                    }
                }
            }

            foreach (var dir in candidateDirs.OrderBy(_ => Random.value).Take(2))
            {
                string finishName = GetFinishName(dir, false);
                Transform finish = finishGroup.Find(finishName);
                if (finish != null)
                {
                    finish.gameObject.SetActive(true);
                    var trigger = finish.GetComponent<FinishTrigger>();
                    if (trigger != null)
                    {
                        trigger.direction = dir;
                        trigger.isBoss = false;
                    }
                }
            }

            if (backDir.HasValue)
            {
                string backFinishName = GetFinishName(backDir.Value, false);
                Transform backFinish = finishGroup.Find(backFinishName);
                if (backFinish != null)
                    backFinish.gameObject.SetActive(false);
            }
        }

        // === Escape 처리 ===
        bool noUnvisited = true;
        Vector2Int[] dirsToCheck = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in dirsToCheck)
        {
            Vector2Int neighborGrid = nextGrid + dir;
            if (placedRooms.TryGetValue(neighborGrid, out var neighborRoom) && !neighborRoom.hasBeenVisited)
            {
                noUnvisited = false;
                break;
            }
        }

        if (noUnvisited)
        {
            Transform escapeGroup = roomData.instance.transform.Find("Escape Finish Group");
            if (escapeGroup != null)
            {
                foreach (Transform child in escapeGroup) child.gameObject.SetActive(false);

                Transform escapeFinish = escapeGroup.Find("Escape Finish");
                if (escapeFinish != null)
                {
                    escapeFinish.gameObject.SetActive(true);
                    var trigger = escapeFinish.GetComponent<EscapeFinishTrigger>();
                    if (trigger != null)
                    {
                        trigger.originGrid = nextGrid;
                    }

                    Debug.Log("[StageManager] Escape Finish 활성화 완료 at " + nextGrid);
                }
            }
        }
    }


    // 최대 스테이지 리미트
    public void SetProgressionLimit(int limit)
    {
        maxStageCounter = limit;
    }

    // 초기 Start 방은 1개의 좌측과 우측 중 Finish만 활성화
    private void SetStartRoomFinishTwoDirections(GameObject roomInstance)
    {
        Transform finishGroup = roomInstance.transform.Find("Finish Group");
        if (finishGroup == null)
        {
            Debug.LogWarning("[StageManager] StartRoom에 Finish Group 없음");
            return;
        }

        // 모든 Finish 비활성화
        foreach (Transform child in finishGroup)
        {
            child.gameObject.SetActive(false);
        }

        // Left와 Right Finish 둘 다 활성화
        string[] targets = { "Left Finish", "Right Finish" };

        foreach (string finishName in targets)
        {
            Transform finish = finishGroup.Find(finishName);
            if (finish != null)
            {
                finish.gameObject.SetActive(true);
                var trigger = finish.GetComponent<FinishTrigger>();
                if (trigger != null)
                {
                    trigger.direction = finishName == "Left Finish" ? Vector2Int.left : Vector2Int.right;
                    trigger.isBoss = false;
                }
            }
        }

        Debug.Log("[StageManager] StartRoom에서 Left, Right Finish 모두 활성화됨");
    }



    //// Boss Finish 활성화 메서드
    //public void EnableBossFinishIfReady()
    //{
    //    if (stageCounter < maxStageCounter || bossRoomData == null)
    //    {
    //        Debug.Log("[StageManager] 아직 보스 방 조건 미달");
    //        return;
    //    }

    //    Debug.Log("[StageManager] Boss Finish 활성화 시작");

    //    // 일반 방들만 무작위 순회
    //    var candidateGrids = placedRooms
    //        .Where(pair => pair.Value.type != StageType.Boss)
    //        .Select(pair => pair.Key)
    //        .OrderBy(_ => Random.value)
    //        .ToList();

    //    foreach (var grid in candidateGrids)
    //    {
    //        var data = placedRooms[grid];

    //        Transform instance = data.instance?.transform;
    //        if (instance == null) continue;

    //        // Boss Finish Group 확인
    //        var bossGroup = instance.Find("Boss Finish Group");
    //        if (bossGroup == null) continue;

    //        // 일반 Finish Group 비활성화
    //        var finishGroup = instance.Find("Finish Group");
    //        if (finishGroup != null)
    //        {
    //            foreach (Transform finish in finishGroup)
    //                finish.gameObject.SetActive(false);
    //        }

    //        // Boss Finish Group 내 모든 Finish 비활성화
    //        foreach (Transform finish in bossGroup)
    //            finish.gameObject.SetActive(false);

    //        // "Boss Finish 1"만 활성화
    //        var targetBossFinish = bossGroup.Find("Boss Finish 1");
    //        if (targetBossFinish != null)
    //        {
    //            targetBossFinish.gameObject.SetActive(true);

    //            var trigger = targetBossFinish.GetComponent<FinishTrigger>();
    //            if (trigger != null)
    //            {
    //                // Boss Finish에 대한 방향 설정은 불필요하거나 고정값 사용 가능
    //                trigger.direction = Vector2Int.zero; // 또는 필요한 방향으로
    //                trigger.isBoss = true;
    //            }

    //            Debug.Log($"[StageManager] Boss Finish 1 활성화 완료 at {grid}");
    //            return; // 1개만 처리하고 종료
    //        }
    //    }
    //}

    //
    public Vector2Int GetBossRoomGrid()
    {
        foreach (var pair in placedRooms)
        {
            if (pair.Value.type == StageType.Boss)
                return pair.Key;
        }

        Debug.LogError("[StageManager] 보스 방을 찾지 못했습니다.");
        return currentGrid; // fallback
    }


    private void ActivateBossFinish(Transform group, string name, Vector2Int dir)
    {
        Transform finish = group.Find(name);
        if (finish != null)
        {
            finish.gameObject.SetActive(true);
            var trigger = finish.GetComponent<FinishTrigger>();
            if (trigger != null)
            {
                trigger.direction = dir;
                trigger.isBoss = true;
            }
        }
    }

    // 보스 방은 룸 외부에 생성되도록
    public void SpawnBossRoom()
    {
        if (bossRoomData != null) return; // 이미 생성되었으면 무시

        // 실제로 그리드 밖이지만 위치는 아무 곳이나 써도 됨
        Vector2Int bossGrid = new Vector2Int(-999, -999);

        bossRoomData = new StageData(bossGrid.x, bossGrid.y, StageType.Boss);
        bossRoomData.prefab = bossRoomPrefab;

        Vector3 spawnPos = new Vector3(9999f, 9999f, 0f); // 보스룸을 씬 외부에 두어 카메라에 보이지 않게
        GameObject bossInstance = Instantiate(bossRoomPrefab, spawnPos, Quaternion.identity);

        bossRoomData.instance = bossInstance;
        placedRooms[bossGrid] = bossRoomData;
    }

    public void MovePlayerToBossRoom()
    {
        if (bossRoomData == null)
        {
            SpawnBossRoom();
        }

        currentGrid = bossRoomData.GetGridPosition();

        Transform spawn = bossRoomData.instance.transform.Find("SpawnPoint");
        player.position = spawn != null ? spawn.position : bossRoomData.instance.transform.position;
    }

    // 보스 피니시 활성화 처리 영역
    public void EnableBossFinishOnly()
    {
        foreach (var pair in placedRooms)
        {
            var data = pair.Value;

            // Finish Group 전부 비활성화
            var finishGroup = data.instance.transform.Find("Finish Group");
            if (finishGroup != null)
                finishGroup.gameObject.SetActive(false);

            // Boss Finish Group에서 하나만 활성화
            var bossGroup = data.instance.transform.Find("Boss Finish Group");
            if (bossGroup == null) continue;

            foreach (Transform t in bossGroup)
                t.gameObject.SetActive(false);

            // 하나만 찾고 활성화
            string[] finishes = { "Top Boss Finish", "Down Boss Finish", "Left Boss Finish", "Right Boss Finish" };
            foreach (var name in finishes)
            {
                var finish = bossGroup.Find(name);
                if (finish != null)
                {
                    finish.gameObject.SetActive(true);
                    var trigger = finish.GetComponent<FinishTrigger>();
                    if (trigger != null) trigger.isBoss = true;
                    break;
                }
            }
        }
    }

    // 보스 방 미리 생성
    public void PreloadBossRoom()
    {
        if (bossRoomData != null) return;

        Vector2Int bossGrid = new Vector2Int(-999, -999); // RoomConnector 대상 제외
        bossRoomData = new StageData(bossGrid.x, bossGrid.y, StageType.Boss);
        bossRoomData.prefab = bossRoomPrefab;

        Vector3 offscreenPos = new Vector3(9999f, 9999f, 0f);
        GameObject bossInstance = Instantiate(bossRoomPrefab, offscreenPos, Quaternion.identity);

        bossInstance.SetActive(false); // 처음엔 비활성화!
        bossRoomData.instance = bossInstance;

        placedRooms[bossGrid] = bossRoomData;
    }

    public void ActivateBossRoomIfReady()
    {
        if (stageCounter >= maxStageCounter && bossRoomData != null)
        {
            bossRoomData.instance?.SetActive(true);
            Debug.Log("[StageManager] 보스 방 활성화 완료");
        }
    }

    // 이전 방으로 되돌아가는 방향 뒤집기 유틸 비활성화
    private static Vector2Int GetOppositeDirection(Vector2Int dir)
    {
        return new Vector2Int(-dir.x, -dir.y);
    }

    // 이전 방 되돌아가기 방지 보조 함수
    private string GetFinishName(Vector2Int dir, bool isBoss)
    {
        string prefix = isBoss ? "Boss " : "";
        if (dir == Vector2Int.up) return prefix + "Top Finish";
        if (dir == Vector2Int.down) return prefix + "Down Finish";
        if (dir == Vector2Int.left) return prefix + "Left Finish";
        if (dir == Vector2Int.right) return prefix + "Right Finish";
        return null;
    }


    private GameObject GetPrefabByType(StageType type)
    {
        return type switch
        {
            StageType.Start => startRoomPrefab,
            StageType.Normal => GetRandomFromArray(normalRoomPrefabs),
            StageType.Hard => GetRandomFromArray(hardRoomPrefabs),
            StageType.Store => GetRandomFromArray(storeRoomPrefabs),
            StageType.Event => GetRandomFromArray(eventRoomPrefabs),
            StageType.Boss => bossRoomPrefab,
            _ => null
        };
    }

    private GameObject GetRandomFromArray(GameObject[] array)
    {
        if (array == null || array.Length == 0) return null;
        return array[Random.Range(0, array.Length)];
    }

    private Dictionary<Vector2Int, StageType> GetRoomTypeMap()
    {
        Dictionary<Vector2Int, StageType> map = new();
        foreach (var pair in placedRooms)
        {
            map[pair.Key] = pair.Value.type;
        }
        return map;
    }

    public StageData GetRoomAt(Vector2Int grid)
    {
        return placedRooms.TryGetValue(grid, out var data) ? data : null;
    }

    public Vector2Int GetCurrentGrid()
    {
        return currentGrid;
    }

    private string GetBossFinishName(Vector2Int dir)
    {
        if (dir == Vector2Int.right) return "Left Boss Finish";  // 플레이어는 왼쪽에서 보스로 감
        if (dir == Vector2Int.left) return "Right Boss Finish";
        if (dir == Vector2Int.up) return "Down Boss Finish";
        if (dir == Vector2Int.down) return "Top Boss Finish";
        return "Unknown";
    }
}
