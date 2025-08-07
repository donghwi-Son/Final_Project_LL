using System.Collections;
using UnityEngine;

public interface IRoomCondition
{
    bool IsConditionMet(); // 조건 달성 여부
    void OnPlayerInteract(); // 상호작용 처리
    void Setup(GameObject roomInstance);
}

public class RoomConditionManager : MonoBehaviour
{
    public GameObject finishObject;       // 일반 Finish Group
    public GameObject bossFinishObject;   // Boss Finish Group

    private IRoomCondition condition;
    //private bool isBossRoom = false;
    private bool finishActivated = false;

    private void Start()
    {
        condition = GetComponent<IRoomCondition>();

        if (finishObject != null) finishObject.SetActive(false);
        if (bossFinishObject != null) bossFinishObject.SetActive(false);

        // 여기 핵심
        GameObject root = transform.root.gameObject;
        var data = StageManager.Instance?.GetStageDataByInstance(root);
        //if (data != null && data.type == StageType.Boss)
        //{
        //    isBossRoom = true;
        //    Debug.Log("[RoomConditionManager] 보스 방으로 인식됨 (루트 기준)");
        //}
        //else
        //{
        //    Debug.LogWarning("[RoomConditionManager] Boss 인식 실패! → data: " + (data == null ? "null" : data.type.ToString()));
        //}

        condition?.Setup(root); // 조건 설정도 반드시 루트 전달
    }



    private IEnumerator AssignRoomTypeAfterOneFrame()
    {
        yield return null; // 1프레임 기다림
        var data = StageManager.Instance?.GetStageDataByInstance(gameObject);
        //if (data != null && data.type == StageType.Boss)
        //{
        //    isBossRoom = true;
        //    Debug.Log("[RoomConditionManager] 보스 방으로 인식됨 (지연 확인)");
        //}
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

            bossFinishObject.SetActive(true);
            finishActivated = true;
            Debug.Log("[RoomConditionManager] Boss Finish 활성화 완료");

            DestroyAllWithTag("Enemy");
            DestroyAllWithTag("NPC");
        }
        else
        {
            Debug.LogWarning("[RoomConditionManager] Boss Finish 오브젝트가 null임");
        }
    }



    private void DestroyAllWithTag(string tag)
    {
        var targets = GameObject.FindGameObjectsWithTag(tag);
        foreach (var obj in targets)
        {
            Destroy(obj);
        }
        Debug.Log($"[RoomConditionManager] 모든 {tag} 제거 완료");
    }
}
