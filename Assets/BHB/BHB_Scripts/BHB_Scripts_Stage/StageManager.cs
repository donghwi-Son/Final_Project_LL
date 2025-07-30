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

    [Header("연결 참조")]
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

        foreach (var pair in placedRooms)
        {
            Vector2Int grid = pair.Key;
            StageData data = pair.Value;

            GameObject prefab = GetPrefabByType(data.type);
            if (prefab == null) continue;

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
        EnsureStartRoomSideConnections();
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
    }

    // 플레이어가 이전 방에서 다음 방으로 이동하는 부분을 다루는 영역
    public void MovePlayerTo(Vector2Int nextGrid)
    {
        if (!placedRooms.ContainsKey(nextGrid)) return;

        Vector2Int prevGrid = currentGrid;
        currentGrid = nextGrid;

        // 런타임마다 생성되는 모든 방은 최초 비활성화 상태
        foreach (var room in placedRooms.Values)
        {
            if (room.instance != null)
                room.instance.SetActive(false);
        }

        // 방에 플레이어가 들어갈 때마다 활성화
        StageData roomData = placedRooms[nextGrid];
        if (roomData.instance != null)
            roomData.instance.SetActive(true);

        // backDir(이전 방) 가장 먼저 계산 
        Vector2Int? backDir = null;
        if (prevGrid != Vector2Int.zero && prevGrid != nextGrid)
        {
            Vector2Int fromDir = nextGrid - prevGrid;
            if (Mathf.Abs(fromDir.x) + Mathf.Abs(fromDir.y) == 1)
            {
                backDir = new Vector2Int(-fromDir.x, -fromDir.y);
            }
        }

        // FinishGroup 참조하여 자식인 상하좌우 Finish 활성화
        Transform finishGroup = roomData.instance.transform.Find("Finish Group");

        // SpawnPoint 위치: backDir 기반 Entry Finish 사용
        // 이전 방의 Finish와 반대되는 곳에 SpawnPoint가 놓여짐
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

        player.position = spawn != null ? spawn.position : roomData.instance.transform.position;

        // 미니맵 처리
        MiniMapManager.instance?.HighlightIcon(nextGrid);
        MiniMapManager.instance?.RevealRoom(nextGrid, roomData.type);
        // 라인 처리 — MiniMapManager 쪽에서 진행
        if (backDir.HasValue)
        {
            MiniMapManager.instance?.ShowOnlyLineInDirection(nextGrid, roomData, backDir.Value);
        }
        MiniMapManager.instance?.ShowLinesFromThisRoom(nextGrid, roomData);

        roomData.hasBeenVisited = true;

        // Start Room이면 포탈 처리 생략
        // Start Room은 매 게임마다 최초 입장 1번만 허용되게 막는 역할
        if (roomData.type == StageType.Start) return;

        // Boss 조건 처리 먼저
        if (stageCounter >= maxStageCounter)
        {
            if (finishGroup != null)
                foreach (Transform child in finishGroup) child.gameObject.SetActive(false);

            Transform bossGroup = roomData.instance.transform.Find("Boss Finish Group");
            if (bossGroup != null)
            {
                foreach (Transform child in bossGroup)
                    child.gameObject.SetActive(false);

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
                }
            }
            return; // Boss 조건이면 나머지 처리 생략
        }

        // 일반 Finish 활성화
        // 기본적으로 각 방마다 상하좌우로 연결된 방 && 아직 거치지 않은 방을 확인하고 Finish 생성
        // 만약, 상하좌우 연결된 새로운 방이 없고 이전 건너온 방들만 존재한다면 되돌아갈 수 있게 Finish 생성
        if (finishGroup != null)
        {
            foreach (Transform child in finishGroup) child.gameObject.SetActive(false);

            List<Vector2Int> candidateDirs = new();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                if (backDir.HasValue && dir == backDir.Value) continue;

                if (roomData.HasNeighbor(dir))
                {
                    Vector2Int neighborGrid = nextGrid + dir;

                    if (placedRooms.TryGetValue(neighborGrid, out var neighbor))
                    {
                        // 아직 방문하지 않은 방이면 무조건 Finish 열기
                        if (!neighbor.hasBeenVisited)
                        {
                            candidateDirs.Add(dir);
                        }
                    }
                }
            }

            // Finish 생성 (개수 제한 없이 전부 활성화)
            foreach (var dir in candidateDirs)
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

            // 보충
            if (candidateDirs.Count < 2)
            {
                foreach (var dir in directions)
                {
                    if (backDir.HasValue && dir == backDir.Value) continue;
                    if (candidateDirs.Contains(dir)) continue;

                    Vector2Int neighborGrid = nextGrid + dir;
                    if (roomData.HasNeighbor(dir) && placedRooms.ContainsKey(neighborGrid))
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

            // 되돌아가는 방향 확실히 비활성화
            if (backDir.HasValue)
            {
                string backName = GetFinishName(backDir.Value, false);
                Transform backFinish = finishGroup.Find(backName);
                if (backFinish != null)
                    backFinish.gameObject.SetActive(false);
            }

            // 되돌아가기만 가능한 경우의 처리
            if (candidateDirs.Count == 0 && backDir.HasValue)
            {
                Vector2Int backGrid = nextGrid + backDir.Value;
                if (placedRooms.TryGetValue(backGrid, out var backRoom))
                {
                    // Start Room이면 Finish 생성 금지
                    if (backRoom.type != StageType.Start) //  핵심 조건 추가됨
                    {
                        string backFinishName = GetFinishName(backDir.Value, false);
                        Transform backFinish = finishGroup.Find(backFinishName);
                        if (backFinish != null)
                        {
                            backFinish.gameObject.SetActive(true);
                            var trigger = backFinish.GetComponent<FinishTrigger>();
                            if (trigger != null)
                            {
                                trigger.direction = backDir.Value;
                                trigger.isBoss = false;
                                trigger.isReturning = true; // 되돌이 표시
                            }
                        }
                    }
                }
            }

        }
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
        foreach (var pair in placedRooms)
        {
            if (pair.Value.isStartRoom)
            {
                Vector2Int startGrid = pair.Key;
                Vector2Int[] dirs = { Vector2Int.left, Vector2Int.right };

                foreach (var dir in dirs)
                {
                    Vector2Int neighborGrid = startGrid + dir;

                    if (!generator.IsInsideGrid(neighborGrid)) continue; // 그리드 초과 방지
                    if (!placedRooms.ContainsKey(neighborGrid))
                    {
                        StageData neighbor = new StageData(neighborGrid.x, neighborGrid.y, StageType.Normal);
                        placedRooms[neighborGrid] = neighbor;
                    }

                    var current = placedRooms[startGrid];
                    var neighborRoom = placedRooms[neighborGrid];

                    // 여기서 강제 연결
                    current.Connect(neighborRoom, dir);
                    neighborRoom.Connect(current, -dir);
                }

                break; // StartRoom은 하나뿐이므로 break
            }
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

        bossInstance.SetActive(false); // 처음엔 비활성화
        bossRoomData.instance = bossInstance;

        placedRooms[bossGrid] = bossRoomData;
    }

    public void ActivateBossRoomIfReady()
    {
        if (stageCounter >= maxStageCounter && bossRoomData != null)
            bossRoomData.instance?.SetActive(true);
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
}
