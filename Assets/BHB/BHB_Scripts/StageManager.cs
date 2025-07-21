using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// 스테이지-버튼을 인덱스 번호가 아닌 각기 연결
// 이전까지는 스테이지-버튼이 동일해야 했지만, 이를 통해 인덱스 번호만 맞추도록 수정
[System.Serializable]
public class StageButtonBinding
{
    public Button button; // 스테이지 이동 버튼 연결
    public int stageIndex; // 버튼이 이동시킬 스테이지 인덱스 (Stages 배열의 인덱스)
    public StageType stagetype; // 스테이지 별 타입 구분(시작/일반/어려움/상점/이벤트/보스)
}

// 스테이지 타입 관리 영역
public enum StageType
{
    Start,
    Normal,
    Hard,
    Store,
    Event,
    Boss
}

public class StageData
{
    public StageType type;

    public StageData RightMap = null;
    public StageData LeftMap = null;
    public StageData UpMap = null;
    public StageData DownMap = null;

    public int Num;
    public int indexX;
    public int indexY;

    public GameObject prefab;
    public GameObject instance;

    public void InitSttting(int num, int x, int y, StageType type)
    {
        this.Num = num;
        this.indexX = x;
        this.indexY = y;
        this.type = type;
    }

    public void InitSttting(int num, int x, int y)
    {
        this.Num = num;
        this.indexX = x;
        this.indexY = y;
        this.type = StageType.Normal;
    }

    public void Connect(StageData neighbor, Vector2Int direction)
    {
        if (direction == Vector2Int.right) RightMap = neighbor;
        else if (direction == Vector2Int.left) LeftMap = neighbor;
        else if (direction == Vector2Int.up) UpMap = neighbor;
        else if (direction == Vector2Int.down) DownMap = neighbor;
    }

    public Vector2Int GetGridPosition() => new Vector2Int(indexX, indexY);
}



// 스테이지 관리 매니저
// 나중에 GameManager와 합칠 수 있도록 조정 중...
public class StageManager : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject[] hardRoomPrefabs;
    public GameObject[] storeRoomPrefabs;
    public GameObject[] eventRoomPrefabs;
    public GameObject bossRoomPrefab;

    public Dictionary<Vector2Int, StageData> placedRooms = new();
    private Vector2Int currentGrid;
    private Vector2 roomSize = new Vector2(28.8456f, 16.1808f);
    private Vector3 origin = Vector3.zero;
    public Transform player;
    [SerializeField] private StageGenerator generator; // StageGenerator 연결
    public static StageManager Instance;
    public MiniMapManager miniMapManager;

    public int maxStageCounter = 10;
    public int StageCounter = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }

    void Start()
    {
        // 중심 좌표 계산
        int centerX = generator.cols / 2;
        int centerY = generator.rows / 2;
        currentGrid = new Vector2Int(centerX, centerY);

        // 기존에 있는 StartRoom을 찾아 사용
        GameObject start = GameObject.Find("Start Stage"); // 계층에 존재하는 오브젝트 이름
        if (start == null)
        {
            Debug.Log("[StageManager] StartRoom이 Hierarchy에 존재하지 않습니다.");
            return;
        }

        placedRooms[currentGrid] = new StageData
        {
            type = StageType.Start,
            indexX = currentGrid.x,
            indexY = currentGrid.y,
            instance = start
        };

        // 플레이어를 SpawnPoint로 이동
        Transform spawn = start.transform.Find("SpawnPoint");
        if (spawn != null)
        {
            player.position = spawn.position;
        }
        else
        {
            Debug.Log("[StageManager] StartRoom에 SpawnPoint가 없습니다.");
        }

        // roomSize 정확히 측정
        var example = normalRoomPrefabs[0];
        var tilemap = example.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            var bounds = tilemap.localBounds;
            roomSize = new Vector2(bounds.size.x, bounds.size.y);
        }

        // StageManager.cs > Start() 내부에서
        roomSize = generator.roomSize;
        origin = generator.origin;
    }

    public void OnPlayerFinish()
    {
        Vector2Int nextGrid = GetNextEmptyGrid(currentGrid);
        if (nextGrid == Vector2Int.zero) return;

        StageType nextType = GetRandomType(); // Normal / Hard / Store 등
        GameObject prefab = GetPrefabByType(nextType);
        Vector3 worldPos = generator.GridToWorld(nextGrid);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);
        ActivateRandomFinishes(room);

        StageData newStage = new StageData
        {
            type = nextType,
            indexX = nextGrid.x,
            indexY = nextGrid.y,
            prefab = prefab,
            instance = room
        };
        placedRooms[nextGrid] = newStage;

        Vector2Int direction = nextGrid - currentGrid;
        placedRooms[currentGrid].Connect(newStage, direction);
        newStage.Connect(placedRooms[currentGrid], -direction);

        currentGrid = nextGrid;

        Transform spawn = room.transform.Find("SpawnPoint");
        if (spawn != null)
            player.position = spawn.position;
    }

    public void GenerateFirstRoom()
    {
        Vector2Int gridPos = generator.GetCenterGrid();
        StageType type = GetRandomStageType();
        GameObject prefab = GetPrefabByType(type);

        if (prefab == null)
        {
            Debug.Log($"[StageManager] '{type}' 타입의 프리팹이 null입니다! 프리팹 연결 확인 요망.");
            return;
        }

        // 프리팹 중심 좌표 보정 적용
        Vector3 worldPos = generator.GridToWorld(gridPos);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);

        var stageData = new StageData
        {
            type = type,
            indexX = gridPos.x,
            indexY = gridPos.y,
            instance = room
        };

        placedRooms[gridPos] = stageData;
        currentGrid = gridPos;

        Transform bossFinishGroup = room.transform.Find("Boss Finish Group");
        if (bossFinishGroup != null)
        {
            foreach (Transform boss in bossFinishGroup)
            {
                boss.gameObject.SetActive(false);
            }
        }

        // 일반 Finish는 랜덤 2개 활성화
        ActivateRandomFinishes(room);

        // 스폰 포인트로 플레이어 이동
        var spawn = room.transform.Find("SpawnPoint");
        player.position = spawn != null ? spawn.position : worldPos;
    }

    void ActivateRandomFinishes(GameObject room)
    {
        Transform finishGroup = room.transform.Find("Finish Group");
        if (finishGroup == null)
        {
            Debug.LogWarning("[StageManager] Finish Group이 프리팹 안에 없습니다.");
            return;
        }

        // 방향 오브젝트 수집
        List<Transform> finishes = new List<Transform>();
        string[] directions = { "Top Finish", "Down Finish", "Left Finish", "Right Finish" };

        foreach (string dir in directions)
        {
            Transform finish = finishGroup.Find(dir);
            if (finish != null)
            {
                finish.gameObject.SetActive(false); // 일단 전체 비활성화
                finishes.Add(finish);
            }
            else
            {
                Debug.LogWarning($"[StageManager] {dir} 오브젝트가 Finish Group 아래에 없습니다.");
            }
        }

        // 무작위 2개 활성화
        if (finishes.Count >= 2)
        {
            var selected = finishes.OrderBy(x => Random.value).Take(2);
            foreach (var finish in selected)
                finish.gameObject.SetActive(true);
        }
    }

    // 새로운 방 생성
    public void TryMoveToDirection(Vector2Int dir)
    {
        Vector2Int nextGrid = currentGrid + dir * 2;

        // 이미 있는 방이면 그걸로 이동
        if (placedRooms.ContainsKey(nextGrid))
        {
            currentGrid = nextGrid;
            var existingRoom = placedRooms[nextGrid].instance;
            player.position = GetEntryPosition(existingRoom, -dir);
            MiniMapManager.Instance.HighlightCurrent(currentGrid); // 미니맵 
            return;
        }

        // 최대 방 수 제한
        if (StageCounter >= maxStageCounter)
        {
            Debug.Log("[StageManager] 최대 방 수에 도달하여 더 이상 생성되지 않습니다.");
            return;
        }

        // 새 방 생성
        StageType type = GetRandomStageType(); // Normal, Hard, Store, Event 중
        GameObject prefab = GetPrefabByType(type);
        Vector3 worldPos = generator.GridToWorld(nextGrid);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);

        //ActivateRandomFinishes(room); // 아까 만든 랜덤 Finish 활성화

        StageData newStage = new StageData
        {
            type = type,
            indexX = nextGrid.x,
            indexY = nextGrid.y,
            prefab = prefab,
            instance = room
        };

        placedRooms[nextGrid] = newStage;
        StageCounter++;

        // 연결
        placedRooms[currentGrid].Connect(newStage, dir);
        newStage.Connect(placedRooms[currentGrid], -dir);

        // 이동
        currentGrid = nextGrid;
        player.position = GetEntryPosition(room, -dir);

        ActivateFinishExits(room, dir);

        MiniMapManager.Instance.SpawnIcon(currentGrid, type);
        MiniMapManager.Instance.HighlightCurrent(currentGrid);
    }

    // 기존 방 돌아가기
    void MoveToRoom(StageData roomData)
    {
        currentGrid = new Vector2Int(roomData.indexX, roomData.indexY);
        Transform spawn = roomData.instance.transform.Find("SpawnPoint");
        player.position = spawn != null ? spawn.position : roomData.instance.transform.position;
    }

    // 방 생성 시, 방향 반대편에 포탈 생성
    void ActivateFinishExits(GameObject room, Vector2Int requiredDirection)
    {
        // "Finish Group" 자식에서 모든 Finish를 가져옴
        Transform finishGroup = room.transform.Find("Finish Group");
        Transform bossFinishGroup = room.transform.Find("Boss Finish Group");
        if (finishGroup == null || bossFinishGroup == null)
        {
            return;
        }

        // 일반 Finish 비활성화
        foreach (Transform child in finishGroup)
            child.gameObject.SetActive(false);

        // 보스 Finish 비활성화
        foreach (Transform boss in bossFinishGroup)
            boss.gameObject.SetActive(false);

        // 보스 전용 Finish 활성화
        if (StageCounter>= maxStageCounter)
        {
            // 4개 방향 중 1개 선택
            List<string> bossFinishNames = new List<string> {
            "Top Boss Finish", "Down Boss Finish", "Left Boss Finish", "Right Boss Finish"
        };

            string selected = bossFinishNames[Random.Range(0, bossFinishNames.Count)];
            Transform selectedBoss = bossFinishGroup.Find(selected);

            if (selectedBoss != null)
            {
                selectedBoss.gameObject.SetActive(true);

                var trigger = selectedBoss.GetComponent<FinishTrigger>();
                if (trigger != null)
                {
                    trigger.direction = GetDirectionFromName(selected); // 방향 매칭
                    trigger.isBoss = true;
                }
            }
            return;
        }

        // 일반 Finish 2개 중 1개는 반드시 이전 방향으로
        List<Transform> candidates = new();
        var required = finishGroup.Find(GetFinishNameFromDirection(requiredDirection));
        if (required != null && generator.IsInsideGrid(currentGrid + GetDirectionFromName(required.name)))
        {
            candidates.Add(required);
        }

        List<string> pool = new List<string> { "Top Finish", "Down Finish", "Left Finish", "Right Finish" };
        pool.Remove(GetFinishNameFromDirection(requiredDirection));

        while (candidates.Count < 2 && pool.Count > 0)
        {
            int rand = Random.Range(0, pool.Count);
            string name = pool[rand];
            Vector2Int dir = GetDirectionFromName(name);
            Vector2Int nextGrid = currentGrid + dir;

            if (generator.IsInsideGrid(nextGrid) && !placedRooms.ContainsKey(nextGrid))
            {
                var f = finishGroup.Find(name);
                if (f != null) candidates.Add(f);
            }

            pool.RemoveAt(rand);
        }

        foreach (var f in candidates)
        {
            f.gameObject.SetActive(true);
            var trigger = f.GetComponent<FinishTrigger>();
            if (trigger != null)
                trigger.direction = GetDirectionFromName(f.name);
        }
    }

    // 어떤 방향에서 오든 방의 고정된 SpawnPoint 위치에 플레이어가 배치
    private Vector3 GetEntryPosition(GameObject room, Vector2Int entryDirection)
    {
        string finishName = entryDirection switch
        {
            { x: 0, y: 1 } => "Top Finish",
            { x: 0, y: -1 } => "Down Finish",
            { x: -1, y: 0 } => "Left Finish",
            { x: 1, y: 0 } => "Right Finish",
            _ => null
        };

        if (finishName == null) return room.transform.position;

        Transform finish = room.transform.Find("Finish Group")?.Find(finishName);
        if (finish == null) return room.transform.position;

        // 오프셋: 해당 방향에서 약간 앞쪽으로
        Vector3 offset = entryDirection switch
        {
            { x: 0, y: 1 } => Vector3.down * 2f,
            { x: 0, y: -1 } => Vector3.up * 2f,
            { x: -1, y: 0 } => Vector3.right * 2f,
            { x: 1, y: 0 } => Vector3.left * 2f,
            _ => Vector3.zero
        };

        return finish.position + offset;
    }

    // 보스 방 생성
    public void SpawnBossRoom(Vector2Int dir)
    {
        Vector2Int targetGrid = currentGrid + dir;
        if (placedRooms.ContainsKey(targetGrid)) return;

        Vector3 worldPos = generator.GridToWorld(targetGrid);
        GameObject room = Instantiate(bossRoomPrefab, worldPos, Quaternion.identity);

        StageData newStage = new StageData
        {
            type = StageType.Boss,
            indexX = targetGrid.x,
            indexY = targetGrid.y,
            instance = room
        };

        placedRooms[targetGrid] = newStage;

        Vector2Int direction = targetGrid - currentGrid;
        placedRooms[currentGrid].Connect(newStage, direction);
        newStage.Connect(placedRooms[currentGrid], -direction);

        currentGrid = targetGrid;

        var spawn = room.transform.Find("SpawnPoint");
        player.position = spawn != null ? spawn.position : worldPos;

        Debug.Log("[StageManager] 보스 방 생성 완료");
    }

    Vector2Int GetNextEmptyGrid(Vector2Int from)
    {
        List<Vector2Int> directions = new()
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        foreach (var dir in directions.OrderBy(x => Random.value))
        {
            Vector2Int next = from + dir;
            if (!placedRooms.ContainsKey(next) && generator.IsInsideGrid(next))
                return next;
        }

        return Vector2Int.zero;
    }

    StageType GetRandomType()
    {
        // 나중엔 확률 조절 가능
        return StageType.Normal;
    }

    GameObject GetPrefabByType(StageType type)
    {
        switch (type)
        {
            case StageType.Normal:
                return normalRoomPrefabs.Length > 0 ? normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)] : null;
            case StageType.Hard:
                return hardRoomPrefabs.Length > 0 ? hardRoomPrefabs[Random.Range(0, hardRoomPrefabs.Length)] : null;
            case StageType.Store:
                return storeRoomPrefabs.Length > 0 ? storeRoomPrefabs[Random.Range(0, storeRoomPrefabs.Length)] : null;
            case StageType.Event:
                return eventRoomPrefabs.Length > 0 ? eventRoomPrefabs[Random.Range(0, eventRoomPrefabs.Length)] : null;
            case StageType.Boss:
                return bossRoomPrefab;
        }
        return null;
    }


    StageType GetRandomStageType()
    {
        int rand = Random.Range(0, 10); // 0~9

        if (rand < 6)
            return StageType.Normal;  // 0~5
        else if (rand < 8)
            return StageType.Hard;    // 6~7
        else if (rand == 8)
            return StageType.Store;   // 8
        else
            return StageType.Event;   // 9
    }


    Vector3 GetRoomCenter(GameObject prefab)
    {
        var tilemap = prefab.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            tilemap.CompressBounds();
            return tilemap.localBounds.center;
        }
        return Vector3.zero;
    }

    // Finish 방향과 이름 표기
    string GetFinishNameFromDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return "Down Finish";
        if (dir == Vector2Int.down) return "Top Finish";
        if (dir == Vector2Int.left) return "Right Finish";
        if (dir == Vector2Int.right) return "Left Finish";
        return null;
    }

    Vector2Int GetDirectionFromName(string name)
    {
        if (name.Contains("Top")) return Vector2Int.up;
        if (name.Contains("Down")) return Vector2Int.down;
        if (name.Contains("Left")) return Vector2Int.left;
        if (name.Contains("Right")) return Vector2Int.right;
        return Vector2Int.zero;
    }
}
