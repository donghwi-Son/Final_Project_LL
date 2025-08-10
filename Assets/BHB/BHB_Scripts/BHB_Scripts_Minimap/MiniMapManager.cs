using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

// M 키 전체 미니맵 패널 UI 전담 클래스
public class MiniMapManager : MonoBehaviour
{
    [Header("패널 및 부모")] // 각각 Canvas 아래 만든 MinimapPanel과 그 자식인 iconparent(프리팹 자식으로 저장 영역) 등록
    [SerializeField] private GameObject fullMapPanel;
    [SerializeField] private Transform iconParent;

    [Header("타입별 아이콘 프리팹")] // UI 캔버스에 표시되는 각 방 타입 별 미니맵 프리팹 등록
    public GameObject startPrefab;
    public GameObject normalPrefab;
    public GameObject hardPrefab;
    public GameObject storePrefab;
    public GameObject eventPrefab;
    public GameObject bossPrefab;

    [Header("설정")]
    public float iconSpacing = 40f; // 미니맵을 나타내는 프리팹이 캔버스에서 서로 유지하는 거리 설정(라인 포함)
    public Vector2 miniMapOffset = new Vector2(0f, 0f); // 위치 보정 역할로 미니맵의 위치 설정

    private Dictionary<Vector2Int, GameObject> spawnedIcons = new();
    private bool isMapOpen = false;
    public static MiniMapManager instance; // 싱글톤

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // M 키로 미니맵 UI 캔버스 활성화 및 활성화 시 플레이어를 포함한 모든 이동 일시정지
        {
            isMapOpen = !isMapOpen;
            fullMapPanel.SetActive(isMapOpen);
            Time.timeScale = isMapOpen ? 0f : 1f;
        }
    }

    // 해당 좌표의 미니맵 아이콘을 생성 또는 활성화 영역
    public void RevealRoom(Vector2Int grid, StageType type)
    {
        if (spawnedIcons.ContainsKey(grid))
        {
            spawnedIcons[grid].SetActive(true);
            return;
        }

        GameObject prefab = GetPrefabForType(type);
        if (prefab == null) return;

        GameObject icon = Instantiate(prefab, iconParent, false);
        icon.GetComponent<RectTransform>().anchoredPosition = GetAnchoredPosition(grid);
        spawnedIcons[grid] = icon;
    }


    // 미니맵에 플레이어가 위치한 방 강조 영역
    // 각 미니맵 프리팹에 존재하는 자식 Player Point를 활성화
    // 활성화 조건은 해당 방에 존재할 때
    public void HighlightIcon(Vector2Int grid)
    {
        foreach (var pair in spawnedIcons)
        {
            var marker = pair.Value.transform.Find("Player Point");
            if (marker != null)
                marker.gameObject.SetActive(false);
        }

        if (spawnedIcons.TryGetValue(grid, out var currentIcon))
        {
            var marker = currentIcon.transform.Find("Player Point");
            if (marker != null)
                marker.gameObject.SetActive(true);
        }
    }

    // 미니맵 라인 처리 수행 영역
    // 각 미니맵간의 연결 및 현재 위치에서 다음 위치를 모를 때 길을 알려줄 Line을 연결된 방끼리 보여주는 영역
    public void ShowLinesFromThisRoom(Vector2Int currentGrid, StageData roomData)
    {
        if (!spawnedIcons.TryGetValue(currentGrid, out GameObject icon)) return;

        Transform lineGroup = icon.transform.Find("Line Group");
        if (lineGroup == null) return;

        // 라인 비활성화 기능 추가
        foreach (Transform child in lineGroup)
        {
            child.gameObject.SetActive(false);
        }

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int neighborGrid = currentGrid + dir;

            // 이웃이 존재하는 경우 가져오기
            StageData neighborData = StageManager.Instance.GetRoomAt(neighborGrid);

            // 없는 방에 라인이 뻗지 않게
            if (neighborData == null || neighborData.instance == null)
            {
                continue;
            }

            // Start 방과 관련된 상하 라인은 무조건 표시 안 함
            // 상하 방향 차단 (Start 방과 연결된 경우)
            bool startIsHere = roomData.type == StageType.Start;
            bool startIsNeighbor = neighborData != null && neighborData.type == StageType.Start;
            bool vertical = dir == Vector2Int.up || dir == Vector2Int.down;

            if (vertical && (startIsHere || startIsNeighbor))
            {
                continue;
            }

            // 이웃 방이 Start  상하 라인 차단
            if (neighborData != null &&
                neighborData.type == StageType.Start &&
                (dir == Vector2Int.down || dir == Vector2Int.up))
                continue;

            // 연결된 이웃이 없으면 표시 안 함
            if (!roomData.HasNeighbor(dir)) continue;

            if (neighborData == null) continue;

            // 쌍방 연결 확인
            Vector2Int oppositeDir = -dir;
            if (!neighborData.HasNeighbor(oppositeDir))
            {
                continue;
            }


            // 반대쪽에서 이미 표시했으면 생략
            if (spawnedIcons.TryGetValue(neighborGrid, out var neighborIcon))
            {
                Transform neighborLineGroup = neighborIcon.transform.Find("Line Group");
                if (neighborLineGroup != null)
                {
                    string oppositeLineName = GetLineName(oppositeDir);
                    Transform neighborLine = neighborLineGroup.Find(oppositeLineName);
                    if (neighborLine != null && neighborLine.gameObject.activeSelf)
                    {
                        continue;
                    }
                }
            }

            // 라인 표시
            string lineName = GetLineName(dir);
            Transform line = lineGroup.Find(lineName);
            if (line != null)
            {
                line.gameObject.SetActive(true);
            }
        }
    }

    // Line Group의 4방향 라인 이름
    private string GetLineName(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return "Top Line";
        if (dir == Vector2Int.down) return "Down Line";
        if (dir == Vector2Int.left) return "Left Line";
        if (dir == Vector2Int.right) return "Right Line";
        return null;
    }

    // Line의 활성화 기준은 이전 방에서부터 뻗어서 다음 방에 연결되는 구조이기에 다음 방에서 이전 방으로 Line이 뻗지 않게 방지하는 영역
    public void ShowOnlyLineInDirection(Vector2Int grid, StageData roomData, Vector2Int fromDir)
    {
        if (!spawnedIcons.TryGetValue(grid, out GameObject icon)) return;

        var group = icon.transform.Find("Line Group");
        if (group == null) return;

        // 반대쪽 방의 라인 확인
        Vector2Int previousGrid = grid + fromDir;
        Vector2Int oppositeDir = -fromDir;

        // 각각 대응되는 라인이 이전 방에 생성되면 현재 방은 없도록
        if (spawnedIcons.TryGetValue(previousGrid, out GameObject prevIcon))
        {
            Transform prevLineGroup = prevIcon.transform.Find("Line Group");
            if (prevLineGroup != null)
            {
                string prevLineName = GetLineName(oppositeDir); 
                Transform prevLine = prevLineGroup.Find(prevLineName);

                if (prevLine != null && prevLine.gameObject.activeSelf)
                {
                    return; // 현재 방에서는 표시하지 않음
                }
            }
        }

        if (roomData.HasNeighbor(fromDir))
        {
            string lineName = GetLineName(fromDir);
            var line = group.Find(lineName);
            if (line != null)
                line.gameObject.SetActive(true);
        }

        // Start 영역으로 위나 아래 Line이 뻗지 않도록(즉, 인접하되 그곳에 Finish가 없기에 갈 수 없음을 나타내는 것)
        if ((roomData.type == StageType.Start) && (fromDir == Vector2Int.up || fromDir == Vector2Int.down)) return;
    }

    // 보스 미니맵만 생성하고 다른 기존 미니맵들을 삭제하는 영역
    public void ShowOnlyBossRoom(Vector2Int bossGrid, StageType type)
    {
        foreach (var bossicon in spawnedIcons.Values)
        {
            if (bossicon != null) Destroy(bossicon);
        }
        spawnedIcons.Clear();

        GameObject prefab = GetPrefabForType(type);
        if (prefab == null)
        {
            return;
        }

        GameObject icon = Instantiate(prefab, iconParent, false);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        spawnedIcons[bossGrid] = icon;

        var marker = icon.transform.Find("Player Point");
        if (marker != null)
            marker.gameObject.SetActive(true);
    }

    // 아이콘 초기화
    public void ClearAllIcons()
    {
        foreach (var icon in spawnedIcons.Values)
        {
            if (icon != null)
                Destroy(icon);
        }
        spawnedIcons.Clear();
    }

    public bool TryGetIcon(Vector2Int gridPos, out GameObject icon)
    {
        return spawnedIcons.TryGetValue(gridPos, out icon);
    }

    private Vector2 GetAnchoredPosition(Vector2Int grid)
    {
        Vector2 center = new Vector2(4.5f, 8f); // generator 기준 center (cols/2, rows/2)
        Vector2 offset = (Vector2)grid - center;
        Vector2 pos = offset * iconSpacing;
        // Y축으로 위로 올리기
        return pos + miniMapOffset;
    }

    private GameObject GetPrefabForType(StageType type)
    {
        return type switch
        {
            StageType.Start => startPrefab,
            StageType.Normal => normalPrefab,
            StageType.Hard => hardPrefab,
            StageType.Store => storePrefab,
            StageType.Event => eventPrefab,
            StageType.Boss => bossPrefab,
            _ => null
        };
    }
}
