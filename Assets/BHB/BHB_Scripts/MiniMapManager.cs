using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public GameObject minimapIconPrefab; // Square Sprite
    public Vector3 offset = Vector3.zero;
    public float iconSpacing = 2f; // 방 간 거리 보정
    public static MiniMapManager Instance;

    private Dictionary<Vector2Int, GameObject> spawnedIcons = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void SpawnIcon(Vector2Int gridPos, StageType type)
    {
        if (spawnedIcons.ContainsKey(gridPos)) return;

        Vector3 worldPos = new Vector3(gridPos.x * iconSpacing, gridPos.y * iconSpacing, 0) + offset;
        GameObject icon = Instantiate(minimapIconPrefab, worldPos, Quaternion.identity);
        icon.layer = LayerMask.NameToLayer("MiniMap");

        var sr = icon.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = GetColorForType(type);

        spawnedIcons[gridPos] = icon;
    }

    public void HighlightCurrent(Vector2Int current)
    {
        foreach (var kv in spawnedIcons)
        {
            var sr = kv.Value.GetComponent<SpriteRenderer>();
            sr.color = (kv.Key == current) ? Color.cyan : GetColorForType(StageManager.Instance.placedRooms[kv.Key].type);
        }
    }

    private Color GetColorForType(StageType type)
    {
        return type switch
        {
            StageType.Start => Color.green,
            StageType.Normal => Color.white,
            StageType.Hard => new Color(1f, 0.5f, 0.5f),
            StageType.Store => Color.yellow,
            StageType.Event => Color.magenta,
            StageType.Boss => Color.black,
            _ => Color.gray
        };
    }
}
