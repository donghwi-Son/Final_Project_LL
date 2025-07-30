using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// M 키 전체 미니맵 패널 UI 전담 클래스
public class MiniMapManager : MonoBehaviour
{
    [Header("패널 및 부모")]
    [SerializeField] private GameObject fullMapPanel;
    [SerializeField] private Transform iconParent;

    [Header("타입별 아이콘 프리팹")]
    public GameObject startPrefab;
    public GameObject normalPrefab;
    public GameObject hardPrefab;
    public GameObject storePrefab;
    public GameObject eventPrefab;
    public GameObject bossPrefab;

    [Header("설정")]
    public float iconSpacing = 40f;

    private Dictionary<Vector2Int, GameObject> spawnedIcons = new();
    private bool isMapOpen = false;
    public static MiniMapManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
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
    public void HighlightIcon(Vector2Int grid)
    {
        foreach (var pair in spawnedIcons)
        {
            var marker = pair.Value.transform.Find("CurrentIcon");
            if (marker != null)
                marker.gameObject.SetActive(false);
        }

        if (spawnedIcons.TryGetValue(grid, out var currentIcon))
        {
            var marker = currentIcon.transform.Find("CurrentIcon");
            if (marker != null)
                marker.gameObject.SetActive(true);
        }
    }

    // 미니맵 라인 처리 수행 영역
    public void ShowLinesFromThisRoom(Vector2Int currentGrid, StageData roomData)
    {
        if (!spawnedIcons.TryGetValue(currentGrid, out GameObject icon)) return;

        Transform lineGroup = icon.transform.Find("Line Group");
        if (lineGroup == null) return;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            if (!roomData.HasNeighbor(dir)) continue;

            Vector2Int neighborGrid = currentGrid + dir;
            Vector2Int oppositeDir = -dir;

            // 반대쪽이 이미 라인을 표시했으면 현재 쪽은 비활성 유지
            if (spawnedIcons.TryGetValue(neighborGrid, out var neighborIcon))
            {
                Transform neighborLineGroup = neighborIcon.transform.Find("Line Group");
                if (neighborLineGroup != null)
                {
                    string oppositeLineName = GetLineName(oppositeDir);
                    Transform neighborLine = neighborLineGroup.Find(oppositeLineName);
                    if (neighborLine != null && neighborLine.gameObject.activeSelf)
                    {
                        Debug.Log($"[MiniMap] {dir} 방향 라인 생략: 반대쪽에서 이미 표시됨");
                        continue;
                    }
                }
            }

            string lineName = GetLineName(dir);
            Transform line = lineGroup.Find(lineName);
            if (line != null)
            {
                line.gameObject.SetActive(true);
                Debug.Log($"[MiniMap] {lineName} 활성화: {currentGrid}");
            }
        }
    }




    private string GetLineName(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return "Top Line";
        if (dir == Vector2Int.down) return "Down Line";
        if (dir == Vector2Int.left) return "Left Line";
        if (dir == Vector2Int.right) return "Right Line";
        return null;
    }

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
                    Debug.Log($"[MiniMap] {grid}: {fromDir} 방향 라인 비활성 (이미 반대쪽 있음)");
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
    }


    // 미니맵 라인 영역

    public bool TryGetIcon(Vector2Int gridPos, out GameObject icon)
    {
        return spawnedIcons.TryGetValue(gridPos, out icon);
    }

    private Vector2 GetAnchoredPosition(Vector2Int grid)
    {
        Vector2 center = new Vector2(4.5f, 8f); // generator 기준 center (cols/2, rows/2)
        Vector2 offset = (Vector2)grid - center;
        return offset * iconSpacing;
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
