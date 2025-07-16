using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<Vector2Int, StageData> placedRooms = new();
    private Vector2Int currentGrid;
    private Vector2 roomSize = new Vector2(28.8456f, 16.1808f);
    private Vector3 origin = Vector3.zero;
    public Transform player;
    [SerializeField] private StageGenerator generator; // StageGenerator 연결
    public static StageManager Instance;

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
            Debug.Log($"[StageManager] Tilemap bounds.size = {bounds.size}");
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
        ActivateRandomFinishes(room);

        // 플레이어 이동 처리 (방 내 SpawnPoint가 있으면)
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
        Vector2Int nextGrid = currentGrid + dir;

        // 이미 있는 방이면 그걸로 이동
        if (placedRooms.ContainsKey(nextGrid))
        {
            MoveToRoom(placedRooms[nextGrid]);
            return;
        }

        // 새 방 생성
        StageType type = GetRandomStageType(); // Normal, Hard, Store, Event 중
        GameObject prefab = GetPrefabByType(type);
        Vector3 worldPos = generator.GridToWorld(nextGrid);
        GameObject room = Instantiate(prefab, worldPos, Quaternion.identity);

        ActivateRandomFinishes(room); // 아까 만든 랜덤 Finish 활성화

        StageData newStage = new StageData
        {
            type = type,
            indexX = nextGrid.x,
            indexY = nextGrid.y,
            prefab = prefab,
            instance = room
        };

        placedRooms[nextGrid] = newStage;

        // 연결
        placedRooms[currentGrid].Connect(newStage, dir);
        newStage.Connect(placedRooms[currentGrid], -dir);

        // 이동
        currentGrid = nextGrid;
        MoveToRoom(newStage);
    }

    // 기존 방 돌아가기
    void MoveToRoom(StageData roomData)
    {
        currentGrid = new Vector2Int(roomData.indexX, roomData.indexY);
        Transform spawn = roomData.instance.transform.Find("SpawnPoint");
        player.position = spawn != null ? spawn.position : roomData.instance.transform.position;
    }


    Vector2Int GetNextEmptyGrid(Vector2Int from)
    {
        // 간단한 랜덤 방향 예시
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
            if (!placedRooms.ContainsKey(next))
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
        StageType[] candidates = new StageType[] {
        StageType.Normal,
        StageType.Hard,
        StageType.Store,
        StageType.Event
    };
        return candidates[Random.Range(0, candidates.Length)];
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


}
