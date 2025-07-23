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

    public void InitializeMiniMap(Dictionary<Vector2Int, StageType> placedRooms)
    {
        foreach (var kv in placedRooms)
        {
            if (spawnedIcons.ContainsKey(kv.Key)) continue;

            GameObject prefab = GetPrefabForType(kv.Value);
            if (prefab == null) continue;

            GameObject icon = Instantiate(prefab, iconParent, false);
            icon.GetComponent<RectTransform>().anchoredPosition = GetAnchoredPosition(kv.Key);
            spawnedIcons[kv.Key] = icon;
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
