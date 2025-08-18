using System.Collections.Generic;
using UnityEngine;

public class RoomConditionManager : MonoBehaviour
{
    [SerializeField] private Transform enemyGroup;
    [SerializeField] private GameObject finishObject;
    [SerializeField] private GameObject bossFinishObject;
    [SerializeField] private GameObject lootBox;
    [SerializeField] private List<GameObject> boss;
    [SerializeField] private bool isBossRoom = false;
    private int count;
    private bool finishActivated = false;

    private void Awake()
    {
        var data = StageManager.Instance?.GetStageDataByInstance(gameObject);
        if (data != null && (data.type == StageType.Event || data.type == StageType.Store))
        {
            if (finishObject != null)
            {
                // Finish Group의 개별 Finish를 방향별로 확인
                Transform finishGroup = finishObject.transform;
                Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach (var dir in directions)
                {
                    string finishName = GetFinishName(dir, false);
                    Transform finish = finishGroup.Find(finishName);
                    if (data.HasNeighbor(dir))
                    {
                        if (finish != null)
                        {
                            finish.gameObject.SetActive(true);
                            var trigger = finish.GetComponent<FinishTrigger>();
                            if (trigger != null)
                            {
                                trigger.direction = dir;
                                trigger.isBoss = false;
                            }
                            Debug.Log($"[RoomConditionManager] Activated {finishName} for {data.type} room at {data.GetGridPosition()}");
                        }
                        else
                        {
                            Debug.LogWarning($"[RoomConditionManager] Finish {finishName} not found in {data.type} room at {data.GetGridPosition()}");
                        }
                    }
                    else
                    {
                        if (finish != null)
                        {
                            finish.gameObject.SetActive(false);
                            Debug.Log($"[RoomConditionManager] Deactivated {finishName} for {data.type} room at {data.GetGridPosition()} (no neighbor)");
                        }
                    }
                }
                finishActivated = true;
                Debug.Log($"[RoomConditionManager] Processed connected Finish objects for {data.type} room at {data.GetGridPosition()}");
            }
            else
            {
                Debug.LogWarning($"[RoomConditionManager] finishObject is null for {data.type} room");
            }
            if (lootBox != null)
            {
                lootBox.GetComponent<BoxCollider2D>().enabled = true;
                lootBox.SetActive(true);
            }
        }
        else if (isBossRoom)
        {
            count = 1; // 보스 방은 항상 1개의 적이 존재

            int randomIndex = Random.Range(0, boss.Count);
            GameObject selectedBoss = Instantiate(boss[randomIndex], enemyGroup.position, Quaternion.identity);

            selectedBoss.transform.SetParent(enemyGroup, true);

            StageManager.Instance.BossObj = selectedBoss;

            switch (randomIndex)
            {
                case 0:
                    StageManager.Instance.BossKey = "Boss1";
                    Debug.Log("[RoomConditionManager] Boss: Crow");
                    break;
                case 1:
                    StageManager.Instance.BossKey = "Boss2";
                    Debug.Log("[RoomConditionManager] Boss: Taoist");
                    break;
                default:
                    Debug.LogWarning("[RoomConditionManager] Unknown boss type selected");
                    break;
            }

            if (bossFinishObject != null)
            {
                bossFinishObject.SetActive(false);
            }
            if (lootBox != null)
            {
                lootBox.SetActive(false);
                lootBox.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        else
        {
            if (enemyGroup != null) count = enemyGroup.childCount;
            if (finishObject != null)
            {
                finishObject.SetActive(false);
            }
            if (lootBox != null)
            {
                lootBox.SetActive(false);
                lootBox.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        if (bossFinishObject != null)
        {
            bossFinishObject.SetActive(false);
            Debug.Log($"[RoomConditionManager] Boss Finish Object deactivated on Start: {bossFinishObject.name}");
        }
        else
        {
            Debug.LogWarning("[RoomConditionManager] bossFinishObject is null on Start");
        }
    }

    private void OnEnable()
    {
        if (enemyGroup == null) return;
        foreach (Transform enemy in enemyGroup)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnDie += OnEnemyDie;
            }
        }
    }

    private void OnDisable()
    {
        if (enemyGroup == null) return;
        foreach (Transform enemy in enemyGroup)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnDie -= OnEnemyDie;
            }
        }
    }

    private void OnEnemyDie()
    {
        count--;
        if (count <= 0 && !finishActivated)
        {
            Debug.Log("[RoomConditionManager] 모든 적 제거됨, 다음 방 생성 조건 달성");
            CreateNextRoom();
        }
    }

    public void CreateNextRoom()
    {
        int current = StageManager.Instance.stageCounter;
        int max = StageManager.Instance.maxStageCounter;
        Debug.Log($"[RoomConditionManager] 조건 달성됨: {current} / {max}");
        var data = StageManager.Instance.GetStageDataByInstance(gameObject);
        if (data != null && data.type == StageType.Boss)
        {
            ActivateBossFinish();
        }
        else
        {
            if (current >= max)
            {
                ActivateBossFinish();
            }
            else
            {
                ActivateNormalFinish();
            }
        }
        finishActivated = true;
    }

    public void ActivateNormalFinish()
    {
        if (finishObject != null)
        {
            if (lootBox != null)
            {
                lootBox.GetComponent<BoxCollider2D>().enabled = true;
                lootBox.SetActive(true);
            }
            finishObject.SetActive(true);
            finishActivated = true;
            Debug.Log("[RoomConditionManager] 일반 Finish 활성화 완료");
        }
        else
        {
            Debug.LogWarning("[RoomConditionManager] 일반 Finish 오브젝트가 비어 있음");
        }
    }

    public void ActivateBossFinish()
    {
        if (StageManager.Instance.stageCounter < StageManager.Instance.maxStageCounter)
        {
            Debug.Log($"[RoomConditionManager] Boss Finish not activated: stageCounter ({StageManager.Instance.stageCounter}) < maxStageCounter ({StageManager.Instance.maxStageCounter})");
            return;
        }
        if (bossFinishObject != null)
        {
            Debug.Log($"[RoomConditionManager] bossFinishObject 확인: {bossFinishObject.name}");
            if (lootBox != null)
            {
                lootBox.GetComponent<BoxCollider2D>().enabled = true;
                lootBox.SetActive(true);
            }
            bossFinishObject.SetActive(true);
            var trigger = bossFinishObject.GetComponent<FinishTrigger>();
            if (trigger != null)
            {
                trigger.direction = Vector2Int.zero;
                trigger.isBoss = true;
            }
            finishActivated = true;
            Debug.Log("[RoomConditionManager] Boss Finish 활성화 완료");
        }
        else
        {
            Debug.LogWarning("[RoomConditionManager] Boss Finish 오브젝트가 null임");
        }
    }

    private string GetFinishName(Vector2Int dir, bool isBoss)
    {
        string prefix = isBoss ? "Boss " : "";
        if (dir == Vector2Int.up) return prefix + "Top Finish";
        if (dir == Vector2Int.down) return prefix + "Down Finish";
        if (dir == Vector2Int.left) return prefix + "Left Finish";
        if (dir == Vector2Int.right) return prefix + "Right Finish";
        return null;
    }
}