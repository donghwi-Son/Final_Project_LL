using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 생성된 StageData를 기반으로 프리팹 생성 및 플레이어 이동 담당 클래스
public class StageManager : MonoBehaviour
{
    public static StageManager Instance; // 싱글톤 인스턴스

    [Header("프리팹 설정")] // 각 타입 별 방 연결
    public GameObject startRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject[] hardRoomPrefabs;
    public GameObject[] storeRoomPrefabs;
    public GameObject[] eventRoomPrefabs;
    public GameObject bossRoomPrefab;

    [Header("연결 참조")] // StageGenerator, MinimapManage, Player 연결
    public StageGenerator generator;
    public MiniMapManager miniMapManager;
    public Transform player;

    [Header("스테이지 진행 정보")]
    public int maxStageCount; // 방 생성 총 수량 (=초기화용), StageInitializer와 연결된 상태

    [Header("보스 진입 조건")]
    public int maxStageCounter; // 플레이어가 몇 방을 지나야 Boss Finish가 열리는지 정하는 영역

    [Header("플레이어 진행도")]
    public int stageCounter; // 플레이어가 방을 지날 때마다 가산되는 영역
    private StageData bossRoomData; // 클래스 필드로 보관하는 보스 방 데이터

    // 각 방의 타입, 방 크기 등의 선언 영역
    public Dictionary<Vector2Int, StageData> placedRooms = new();
    private Vector2 roomSize;
    private Vector3 origin;
    private Vector2Int currentGrid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 매 런타임 초기마다 정해진 수량의 방을 랜덤하게 생성하는 영역
    // 방 생성은 정중앙 Room에 startRoomPrefab 생성 - 좌우로 랜덤 타입의 방이 1개씩 생성 - 그 후에는 각각 최소 1개 이상 이어진 연결성을 가지고 랜덤 생성
    // 보스 방은 가장 멀리 생성됨
    public void InitializeStages(Dictionary<Vector2Int, StageData> stageMap, int maxStageCountFromInitializer, int startingStageCounter)
    {
        placedRooms = stageMap;
        roomSize = generator.roomSize;
        origin = generator.origin;
        maxStageCount = maxStageCountFromInitializer;
        stageCounter = startingStageCounter;
        Debug.Log($"[StageManager] InitializeStages: stageCounter={stageCounter}, maxStageCounter={maxStageCounter}");

        foreach (var pair in placedRooms)
        {
            Vector2Int grid = pair.Key;
            StageData data = pair.Value;
            GameObject prefab = GetPrefabByType(data.type);
            if (prefab == null)
            {
                Debug.LogWarning($"[StageManager] No prefab for {data.type} at {grid}");
                continue;
            }

            Vector3 worldPos = generator.GridToWorld(grid);
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
            instance.SetActive(false);
            data.instance = instance;
            data.prefab = prefab;

            if (data.isStartRoom)
            {
                SetStartRoomFinishTwoDirections(data.instance);
            }
            else if (data.type == StageType.Event || data.type == StageType.Store)
            {
                Transform finishGroup = data.instance.transform.Find("Finish Group");
                if (finishGroup != null)
                {
                    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                    foreach (var dir in directions)
                    {
                        if (data.HasNeighbor(dir))
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
                                Debug.Log($"[StageManager] Activated {finishName} for {data.type} room at {grid}");
                            }
                            else
                            {
                                Debug.LogWarning($"[StageManager] Finish {finishName} not found in {data.type} room at {grid}");
                            }
                        }
                        else
                        {
                            string finishName = GetFinishName(dir, false);
                            Transform finish = finishGroup?.Find(finishName);
                            if (finish != null)
                            {
                                finish.gameObject.SetActive(false);
                                Debug.Log($"[StageManager] Deactivated {finishName} for {data.type} room at {grid} (no neighbor)");
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[StageManager] Finish Group not found in {data.type} room at {grid}");
                }
            }
            else
            {
                Transform finishGroup = data.instance.transform.Find("Finish Group");
                if (finishGroup != null)
                {
                    foreach (Transform child in finishGroup)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            Transform bossFinishGroup = data.instance.transform.Find("Boss Finish Group");
            if (bossFinishGroup != null)
            {
                Transform bossFinish = bossFinishGroup.Find("Boss Finish 1");
                if (bossFinish != null)
                {
                    bossFinish.gameObject.SetActive(false);
                    var trigger = bossFinish.GetComponent<FinishTrigger>();
                    if (trigger != null)
                    {
                        trigger.direction = Vector2Int.zero;
                        trigger.isBoss = true;
                    }
                    Debug.Log($"[StageManager] Force-deactivated Boss Finish 1 for room at {grid}");
                }
                else
                {
                    Debug.LogWarning($"[StageManager] Boss Finish 1 not found in room at {grid}");
                }
            }
        }

        EnsureStartRoomSideConnections();
        RoomConnector.ProcessConnections(placedRooms, generator, maxStageCount, stageCounter);
        PreloadBossRoom();
    }

    // 시작 방으로 이동
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
    }

    // 플레이어가 이전 방에서 다음 방으로 이동하는 부분을 다루는 영역
    public void MovePlayerTo(Vector2Int nextGrid)
    {
        if (!placedRooms.ContainsKey(nextGrid))
        {
            Debug.Log($"[StageManager] Invalid next grid: {nextGrid}");
            return;
        }

        Vector2Int prevGrid = currentGrid;
        currentGrid = nextGrid;

        StageData nextRoomData = placedRooms[nextGrid];
        if (!nextRoomData.hasBeenVisited && nextRoomData.type != StageType.Start)
        {
            stageCounter++;
            Debug.Log($"[StageManager] stageCounter incremented to {stageCounter}");
        }
        else
        {
            Debug.Log($"[StageManager] stageCounter not incremented for {nextRoomData.type} room at {nextGrid}");
        }

        if (nextRoomData.type == StageType.Boss)
        {
            Debug.Log("[StageManager] Attempted to move to boss room via MovePlayerTo, blocking.");
            return;
        }

        if (player.parent != null)
        {
            player.SetParent(null);
        }

        foreach (var room in placedRooms.Values)
        {
            if (room.instance != null)
                room.instance.SetActive(false);
        }

        StageData roomData = placedRooms[nextGrid];
        if (roomData?.instance == null)
        {
            Debug.Log($"[StageManager] Room instance at {nextGrid} is null");
            return;
        }
        roomData.instance.SetActive(true);

        Vector2Int? backDir = null;
        if (prevGrid != Vector2Int.zero && prevGrid != nextGrid)
        {
            Vector2Int fromDir = nextGrid - prevGrid;
            if (Mathf.Abs(fromDir.x) + Mathf.Abs(fromDir.y) == 1)
            {
                backDir = new Vector2Int(-fromDir.x, -fromDir.y);
            }
        }

        Transform finishGroup = roomData.instance.transform.Find("Finish Group");

        Transform spawn = null;
        if (backDir.HasValue && finishGroup != null)
        {
            string entryName = GetFinishName(backDir.Value, false);
            Transform entryFinish = finishGroup.Find(entryName);
            if (entryFinish != null)
            {
                spawn = entryFinish;
            }
        }

        if (spawn == null)
        {
            spawn = roomData.instance.transform.Find("SpawnPoint");
        }

        Vector3 spawnOffset = Vector3.zero;
        if (backDir.HasValue)
        {
            Vector2Int dir = backDir.Value;
            if (dir == Vector2Int.up) spawnOffset = Vector3.down * 2f + Vector3.right * 1f;
            else if (dir == Vector2Int.down) spawnOffset = Vector3.up * 2f;
            else if (dir == Vector2Int.left) spawnOffset = Vector3.right * 2f;
            else if (dir == Vector2Int.right) spawnOffset = Vector3.left * 2f;
        }

        player.position = spawn != null ? spawn.position + spawnOffset : roomData.instance.transform.position;
        Debug.Log($"[StageManager] Player moved to {nextGrid} with spawn at {player.position}");

        if (roomData.type == StageType.Boss)
        {
            MiniMapManager.instance?.ShowOnlyBossRoom(roomData.GetGridPosition(), roomData.type);
            return;
        }

        MiniMapManager.instance?.RevealRoom(nextGrid, roomData.type);
        if (backDir.HasValue)
        {
            MiniMapManager.instance?.ShowOnlyLineInDirection(nextGrid, roomData, backDir.Value);
        }
        MiniMapManager.instance?.ShowLinesFromThisRoom(nextGrid, roomData);
        MiniMapManager.instance.HighlightIcon(currentGrid);

        roomData.hasBeenVisited = true;
        TrySetupRoomCondition(roomData.instance);

        if (roomData.type == StageType.Start) return;

        if (finishGroup != null)
        {
            if (roomData.type != StageType.Event && roomData.type != StageType.Store)
            {
                foreach (Transform child in finishGroup) child.gameObject.SetActive(false);
            }

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var dir in directions)
            {
                if (roomData.type == StageType.Start && (dir == Vector2Int.up || dir == Vector2Int.down))
                    continue;

                if (roomData.HasNeighbor(dir))
                {
                    Vector2Int neighborGrid = nextGrid + dir;
                    if (placedRooms.ContainsKey(neighborGrid))
                    {
                        string finishName = GetFinishName(dir, false);
                        Transform finish = finishGroup.Find(finishName);
                        if (finish != null)
                        {
                            bool activateFinish = roomData.type == StageType.Event || roomData.type == StageType.Store || true;
                            if (activateFinish)
                            {
                                finish.gameObject.SetActive(true);
                                var trigger = finish.GetComponent<FinishTrigger>();
                                if (trigger != null)
                                {
                                    trigger.direction = dir;
                                    trigger.isBoss = false;
                                    trigger.isReturning = backDir.HasValue && dir == backDir.Value;
                                }
                                Debug.Log($"[StageManager] Activated {finishName} for {roomData.type} room at {nextGrid}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[StageManager] Finish {finishName} not found in {roomData.type} room at {nextGrid}");
                        }
                    }
                }
                else
                {
                    string finishName = GetFinishName(dir, false);
                    Transform finish = finishGroup?.Find(finishName);
                    if (finish != null)
                    {
                        finish.gameObject.SetActive(false);
                        Debug.Log($"[StageManager] Deactivated {finishName} for {roomData.type} room at {nextGrid} (no neighbor)");
                    }
                }
            }
        }
    }


    // Finish 조건 업데이트
    private void TrySetupRoomCondition(GameObject roomInstance)
    {
        //IRoomCondition condition = roomInstance.GetComponentInChildren<IRoomCondition>();

        //if (condition != null)
        //{
        //    condition.Setup(roomInstance);
        //}
        //else
        //{
        //    // 조건 스크립트가 없으면 기본 Finish 활성
        //    foreach (var trigger in roomInstance.GetComponentsInChildren<FinishTrigger>())
        //    {
        //        trigger.gameObject.SetActive(true);
        //    }
        //}
    }


    // 최대 스테이지 리미트
    public void SetProgressionLimit(int limit)
    {
        maxStageCounter = limit;
    }

    // 초기 Start 방은 좌측과 우측 Finish만 활성화
    private void SetStartRoomFinishTwoDirections(GameObject roomInstance)
    {
        Transform finishGroup = roomInstance.transform.Find("Finish Group");
        if (finishGroup == null) return;

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
    }

    // 시작 방 연결 처리
    private void EnsureStartRoomSideConnections()
    {
        // 시작 방을 찾고
        var startRoomEntry = placedRooms.FirstOrDefault(p => p.Value.isStartRoom);
        // 시작 방이 없으면 메서드를 종료
        if (startRoomEntry.Value == null) return;

        Vector2Int startGrid = startRoomEntry.Key;
        Vector2Int[] dirs = { Vector2Int.left, Vector2Int.right };

        foreach (var dir in dirs)
        {
            Vector2Int neighborGrid = startGrid + dir;

            // 해당 위치에 방이 없으면 새 방을 생성
            if (!placedRooms.ContainsKey(neighborGrid))
            {
                // StageData 객체를 생성하고 딕셔너리에 추가
                StageData neighborData = new StageData(neighborGrid.x, neighborGrid.y, StageType.Normal);
                placedRooms[neighborGrid] = neighborData;

                // 새로 생성된 방의 프리팹을 인스턴스화하고 비활성화
                GameObject prefab = GetPrefabByType(neighborData.type);
                if (prefab != null)
                {
                    Vector3 worldPos = generator.GridToWorld(neighborGrid);
                    GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
                    instance.SetActive(false);
                    neighborData.instance = instance;
                    neighborData.prefab = prefab;
                }
            }

            // 시작 방과 이웃 방의 연결성을 강제로 설정
            StageData startRoom = startRoomEntry.Value;
            StageData neighborRoom = placedRooms[neighborGrid];

            startRoom.Connect(neighborRoom, dir);
            neighborRoom.Connect(startRoom, -dir);
        }
    }

    // 보스 방 생성
    public Vector2Int GetBossRoomGrid()
    {
        foreach (var pair in placedRooms)
        {
            if (pair.Value.type == StageType.Boss)
                return pair.Key;
        }
        return currentGrid; // fallback
    }

    // 전체 방 제거 & 보스룸 재생성
    public void MoveToBossRoomCleanTransition()
    {
        // 1. 기존 모든 방 제거
        foreach (var room in placedRooms.Values)
        {
            if (room.instance != null)
                Destroy(room.instance);
        }
        placedRooms.Clear();

        // 2. 미니맵 아이콘 전부 제거
        MiniMapManager.instance.ClearAllIcons();

        // 3. 보스 룸 재생성
        Vector2Int bossGrid = new Vector2Int(8, 4); // 중심 좌표
        bossRoomData = new StageData(bossGrid.x, bossGrid.y, StageType.Boss);
        bossRoomData.prefab = bossRoomPrefab;

        Vector3 bossWorldPos = generator.GridToWorld(bossGrid);
        GameObject bossInstance = Instantiate(bossRoomPrefab, bossWorldPos, Quaternion.identity);
        bossInstance.SetActive(true);

        bossRoomData.instance = bossInstance;
        placedRooms[bossGrid] = bossRoomData;
        currentGrid = bossGrid;

        // 4. 플레이어 이동
        Transform spawn = bossInstance.transform.Find("SpawnPoint");
        player.position = spawn != null ? spawn.position : bossWorldPos;

        // 5. 미니맵 중앙에 보스 룸만 표시
        MiniMapManager.instance.ShowOnlyBossRoom(bossGrid, StageType.Boss);
    }


    // 보스 Finish 활성화
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

    // 보스 방은 룸 외부에 생성되도록 하는 영역
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

    // 플레이어 보스 방 이동
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

    // 보스 방 미리 생성해놓고 비활성화 시키기
    public void PreloadBossRoom()
    {
        if (bossRoomData != null) return;

        Vector2Int bossGrid = new Vector2Int(-999, -999); // RoomConnector 대상 제외
        bossRoomData = new StageData(bossGrid.x, bossGrid.y, StageType.Boss);
        bossRoomData.prefab = bossRoomPrefab;

        Vector3 offscreenPos = new Vector3(0f, 0f, 0f);
        GameObject bossInstance = Instantiate(bossRoomPrefab, offscreenPos, Quaternion.identity);

        bossInstance.SetActive(false); // 처음엔 비활성화
        bossRoomData.instance = bossInstance;

        placedRooms[bossGrid] = bossRoomData;
    }

    // 보스 방 생성 조건

    public void ActivateBossRoomIfReady()
    {
        if (stageCounter >= maxStageCounter && bossRoomData != null)
            bossRoomData.instance?.SetActive(true);
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


    // 등록한 프리팹을 각 타입 별로 연결
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

    // 이 밑으로는 각 방을 랜덤하게 생성하는 배열 영역
    private GameObject GetRandomFromArray(GameObject[] array)
    {
        if (array == null || array.Length == 0) return null;
        return array[Random.Range(0, array.Length)];
    }

    public StageData GetRoomAt(Vector2Int grid)
    {
        return placedRooms.TryGetValue(grid, out var data) ? data : null;
    }

    public StageData GetStageDataByInstance(GameObject roomInstance)
    {
        foreach (var data in placedRooms.Values)
        {
            if (data.instance == roomInstance)
                return data;
        }
        return null;
    }

    public Vector2Int GetCurrentGrid()
    {
        return currentGrid;
    }
}