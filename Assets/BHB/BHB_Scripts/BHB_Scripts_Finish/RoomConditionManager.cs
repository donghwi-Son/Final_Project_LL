using UnityEngine;

public class RoomConditionManager : MonoBehaviour
{
    [SerializeField] private Transform enemyGroup;
    [SerializeField] private GameObject finishObject;
    [SerializeField] private GameObject bossFinishObject;
    [SerializeField] private GameObject lootBox;
    private int count;
    private bool finishActivated = false;

    private void Start()
    {
        var data = StageManager.Instance?.GetStageDataByInstance(gameObject);
        if (data != null && (data.type == StageType.Event || data.type == StageType.Store))
        {
            if (finishObject != null)
            {
                finishObject.SetActive(true);
                finishActivated = true;
                Debug.Log($"[RoomConditionManager] Auto-activated Finish for {data.type} room");
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
        else
        {
            if (enemyGroup != null) count = enemyGroup.childCount;
            if (finishObject != null)
            {
                finishObject.SetActive(false);
                Debug.Log("[RoomConditionManager] Deactivated finishObject for non-Event/Store room");
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
            Debug.Log("[RoomConditionManager] Boss Finish Object deactivated on Start");
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
                ActivateNormalFinish(); // 일반 방 클리어 시 무조건 ActivateNormalFinish 호출
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
}