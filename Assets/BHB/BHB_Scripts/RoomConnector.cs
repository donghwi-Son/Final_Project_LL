using System.Collections.Generic;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public static void ProcessConnections(
        Dictionary<Vector2Int, StageData> placedRooms,
        StageGenerator generator,
        int maxStageCount,
        int stageCounter)
    {
        foreach (var pair in placedRooms)
        {
            Vector2Int currentGrid = pair.Key;
            StageData currentRoom = pair.Value;
            GameObject instance = currentRoom.instance;
            if (instance == null) continue;

            // Start와 Boss 방은 별도 처리하므로 제외
            if (currentRoom.type == StageType.Start || currentRoom.type == StageType.Boss)
                continue;

            bool isStartRoom = currentRoom.type == StageType.Start;

            if (isStartRoom)
            {
                Vector2Int[] startDirs = { Vector2Int.left, Vector2Int.right };

                foreach (var dir in startDirs)
                {
                    Vector2Int neighborPos = currentGrid + dir;

                    if (!placedRooms.ContainsKey(neighborPos))
                    {
                        // 새 방 생성 (기본 Normal로)
                        var newRoom = new StageData(neighborPos.x, neighborPos.y, StageType.Normal);
                        placedRooms[neighborPos] = newRoom;
                    }

                    var neighborRoom = placedRooms[neighborPos];
                    currentRoom.Connect(neighborRoom, dir);
                    neighborRoom.Connect(currentRoom, -dir);

                    EnableFinish(instance, dir, false);
                }

                // 상하 연결 무시
                continue;
            }

            // 일반 방만 처리 / 2개 방향 랜덤 선택
            List<Vector2Int> direction = new List<Vector2Int>(directions);
            Shuffle(direction);
            int connectionsMade = 0;

            foreach (var dir in direction)
            {
                if (connectionsMade >= 2)
                    break;

                StageData neighborRoom = FindConnectedRoom(
                    placedRooms,
                    currentGrid,
                    dir,
                    maxDistance: 1,
                    //maxDistance: Mathf.Max(generator.rows, generator.cols),
                    rows: generator.rows,
                    cols: generator.cols);

                if (neighborRoom == null) continue;

                currentRoom.Connect(neighborRoom, dir);
                neighborRoom.Connect(currentRoom, -dir);

                //EnableFinish(instance, dir, isBoss: false);
                connectionsMade++;
            }
        }
    }

    // 맵 섞기
    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // 
    private static StageData FindConnectedRoom(
    Dictionary<Vector2Int, StageData> placedRooms,
    Vector2Int from,
    Vector2Int direction,
    int maxDistance,
    int rows,
    int cols)
    {
        Vector2Int current = from;

        for (int i = 1; i <= maxDistance; i++)
        {
            current += direction;

            // 그리드 벗어나면 중단
            if (current.x < 0 || current.x >= cols || current.y < 0 || current.y >= rows)
                break;

            if (placedRooms.TryGetValue(current, out StageData targetRoom))
                return targetRoom;
        }

        return null;
    }


    private static void EnableFinish(GameObject roomInstance, Vector2Int direction, bool isBoss)
    {
        string groupName = isBoss ? "Boss Finish Group" : "Finish Group";
        string finishName = GetFinishName(direction, isBoss);

        Transform group = roomInstance.transform.Find(groupName);
        if (group == null) return;

        Transform finish = group.Find(finishName);
        if (finish == null) return;

        finish.gameObject.SetActive(true);

        FinishTrigger trigger = finish.GetComponent<FinishTrigger>();
        if (trigger != null)
        {
            trigger.direction = direction;
            trigger.isBoss = isBoss;
        }
    }


    private static string GetFinishName(Vector2Int dir, bool isBoss)
    {
        string prefix = dir switch
        {
            { x: 0, y: 1 } => "Top",
            { x: 0, y: -1 } => "Down",
            { x: -1, y: 0 } => "Left",
            { x: 1, y: 0 } => "Right",
            _ => "Unknown"
        };

        return isBoss ? $"{prefix} Boss Finish" : $"{prefix} Finish";
    }
}
