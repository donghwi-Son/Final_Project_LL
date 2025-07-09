using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] tilemapPrefabs;
    public Vector2 roomSize = new Vector2(28.8456f, 16.1808f);
    public int rows = 16;
    public int cols = 9;
    public Vector3 origin = Vector3.zero;
    public int maxPrefabCount = 10;

    private List<Vector2Int> usedPositions = new List<Vector2Int>();

    void Start()
    {
        SpawnRooms();
    }

    void SpawnRooms()
    {
        // 1. 가능한 모든 방 위치 수집
        usedPositions.Clear();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                usedPositions.Add(new Vector2Int(col, row));
            }
        }

        // 2. 랜덤하게 섞고 최대 N개만 사용
        Shuffle(usedPositions);

        int spawnCount = Mathf.Min(maxPrefabCount, usedPositions.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2Int gridPos = usedPositions[i];
            GameObject prefab = tilemapPrefabs[Random.Range(0, tilemapPrefabs.Length)];

            Vector3 spawnPos = origin + new Vector3(gridPos.x * roomSize.x, gridPos.y * roomSize.y, 0);
            Instantiate(prefab, spawnPos, Quaternion.identity, this.transform);
        }
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
