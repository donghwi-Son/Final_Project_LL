using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    [Header("패널 및 부모")]
    [SerializeField] private GameObject fullMapPanel;
    [SerializeField] private Transform iconParent;

    [Header("타입별 아이콘 프리팹")]
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

    //public void InitializeMiniMap(Dictionary<Vector2Int, StageType> placedRooms)
    //{
    //    foreach (var kv in placedRooms)
    //    {
    //        if (spawnedIcons.ContainsKey(kv.Key)) continue;

    //        GameObject prefab = GetPrefabForType(kv.Value);
    //        if (prefab == null) continue;

    //        GameObject icon = Instantiate(prefab, iconParent, false);
    //        icon.GetComponent<RectTransform>().anchoredPosition = GetAnchoredPosition(kv.Key);
    //        spawnedIcons[kv.Key] = icon;
    //    }
    //}

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
            StageType.Normal => normalPrefab,
            StageType.Hard => hardPrefab,
            StageType.Store => storePrefab,
            StageType.Event => eventPrefab,
            StageType.Boss => bossPrefab,
            _ => null
        };
    }
}
