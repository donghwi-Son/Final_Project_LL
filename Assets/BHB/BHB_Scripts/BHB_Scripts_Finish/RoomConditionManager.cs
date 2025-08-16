using System.Collections.Generic;
using UnityEngine;

public class RoomConditionManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies;           // 방에 있는 적들
    [SerializeField] private GameObject finishObject;       // 일반 Finish Group
    [SerializeField] private GameObject bossFinishObject;   // Boss Finish Group
    [SerializeField] private GameObject lootBox;

    //private bool isBossRoom = false;
    private bool finishActivated = false;

    private void Start()
    {
        if (finishObject != null) finishObject.SetActive(false);
        if (bossFinishObject != null) bossFinishObject.SetActive(false);

        if (lootBox != null)
        {
            lootBox.SetActive(false);
            lootBox.GetComponent<BoxCollider2D>().enabled = false;
        }

        //var data = StageManager.Instance?.GetStageDataByInstance(root);
        //if (data != null && data.type == StageType.Boss)
        //{
        //    isBossRoom = true;
        //    Debug.Log("[RoomConditionManager] 보스 방으로 인식됨 (루트 기준)");
        //}
        //else
        //{
        //    Debug.LogWarning("[RoomConditionManager] Boss 인식 실패! → data: " + (data == null ? "null" : data.type.ToString()));
        //}
    }

    private void OnEnable()
    {
        if (enemies == null || enemies.Count == 0)
        {
            Debug.Log("[RoomConditionManager] 적 목록이 비어 있습니다.");
            CreateNextRoom();

            return;
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnDie += OnEnemyDie;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnDie -= OnEnemyDie;
            }
        }
    }

    private void OnEnemyDie()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.Stats.IsDead)
            {
                enemies.Remove(enemy);
                Debug.Log($"[RoomConditionManager] 적 제거됨: {enemy.name}");

                break;
            }
        }

        // 적이 모두 제거되었는지 확인
        if (enemies.Count == 0 && !finishActivated)
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
            ActivateBossFinish(); // 보스 방 내부에서 피니시 켜기
        }
        else
        {
            // 일반 방인데, Boss Finish 로 연결되는 경우
            if (current >= max)
            {
                ActivateBossFinish(); // Boss Finish 1 만 열기
            }
            else
            {
                ActivateNormalFinish(); // 일반 Finish 켜기
            }
        }

        finishActivated = true;
    }


    public void ActivateNormalFinish()
    {
        if (finishObject != null)
        {
            lootBox.GetComponent<BoxCollider2D>().enabled = true;
            lootBox.SetActive(true);
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
        if (bossFinishObject != null)
        {
            Debug.Log("[RoomConditionManager] bossFinishObject 확인: " + bossFinishObject.name);
            
            lootBox.GetComponent<BoxCollider2D>().enabled = true;
            lootBox.SetActive(true);

            bossFinishObject.SetActive(true);
            finishActivated = true;
            Debug.Log("[RoomConditionManager] Boss Finish 활성화 완료");

            //DestroyAllWithTag("Enemy");
            //DestroyAllWithTag("NPC");
        }
        else
        {
            Debug.LogWarning("[RoomConditionManager] Boss Finish 오브젝트가 null임");
        }
    }

    //private void DestroyAllWithTag(string tag)
    //{
    //    var targets = GameObject.FindGameObjectsWithTag(tag);
    //    foreach (var obj in targets)
    //    {
    //        Destroy(obj);
    //    }
    //    Debug.Log($"[RoomConditionManager] 모든 {tag} 제거 완료");
    //}
}
