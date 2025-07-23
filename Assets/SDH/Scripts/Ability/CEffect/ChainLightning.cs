using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    float lightningRange = 10f;

    LineRenderer lightning;

    private void Awake()
    {
        lightning = GetComponent<LineRenderer>();
    }

    public GameObject FindNeareastEnemy(GameObject currentEnemy)
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, lightningRange, LayerMask.GetMask("Enemy"));
        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider col in colliders)
        {
            if (col.gameObject == currentEnemy) continue; // 자기 자신 제외
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = col.gameObject;
            }
        }
        return nearestEnemy;
    }

    //[Header("체인 라이트닝 설정")]
    //public int maxChainCount = 3;           // 최대 연쇄 횟수
    //public float chainRange = 10f;          // 연쇄 범위
    //public float damage = 50f;              // 기본 데미지  
    //public LayerMask targetLayer = -1;      // 타겟 레이어

    //[Header("비주얼 설정")]
    //public float lightningDuration = 0.5f;  // 번개 지속시간
    //public float lightningWidth = 0.1f;     // 번개 두께

    //[Header("랜덤 효과")]
    //public float randomOffset = 1f;         // 번개 랜덤 오프셋
    //public float flickerSpeed = 20f;        // 번개 깜빡임 속도

    //LineRenderer lightningLine;

    //private List<GameObject> currentTargets = new List<GameObject>();
    //private List<LineRenderer> activeLightnings = new List<LineRenderer>();
    //private List<GameObject> excludeList = new List<GameObject>(); // 제외할 대상 리스트

    //private void Awake()
    //{
    //    lightningLine = GetComponent<LineRenderer>();
    //}


    //public void CastChainLightning(GameObject firstEnemy)
    //{
    //    StartCoroutine(ExecuteChainLightning(firstEnemy));
    //}

    //private IEnumerator ExecuteChainLightning(GameObject firstEnemy)
    //{
    //    GameObject target = FindNearestTarget(firstEnemy);
    //    CreateLightning(firstEnemy.transform.position, target?.transform.position ?? firstEnemy.transform.position);
    //    yield return new WaitForSeconds(lightningDuration);
    //}

    //private GameObject FindNearestTarget(GameObject currentEnemy)
    //{
    //    Collider[] colliders = Physics.OverlapSphere(currentEnemy.transform.position, chainRange, targetLayer);
    //    GameObject nearestTarget = null;
    //    float nearestDistance = float.MaxValue;

    //    foreach (Collider col in colliders)
    //    {
    //        자기 자신이나 이미 타격한 대상 제외
    //        if (col.gameObject == currentEnemy ||
    //            (excludeList != null && excludeList.Contains(col.gameObject)))
    //            continue;

    //        적 태그나 컴포넌트 확인(필요에 따라 수정)
    //        if (!col.CompareTag("Enemy"))
    //            continue;

    //        float distance = Vector3.Distance(currentEnemy.transform.position, col.transform.position);
    //        if (distance < nearestDistance)
    //        {
    //            nearestDistance = distance;
    //            nearestTarget = col.gameObject;
    //        }
    //    }

    //    return nearestTarget;
    //}

    //private void CreateLightning(Vector3 start, Vector3 end)
    //{
    //    if (lightningPrefab == null) return;

    //    LineRenderer lightning = Instantiate(lightningPrefab);
    //    lightning.startWidth = lightningWidth;
    //    lightning.endWidth = lightningWidth;



    //    activeLightnings.Add(lightning);
    //}



    //private void ClearPreviousLightnings()
    //{
    //    foreach (LineRenderer lightning in activeLightnings)
    //    {
    //        if (lightning != null)
    //        {
    //            Destroy(lightning.gameObject);
    //        }
    //    }
    //    activeLightnings.Clear();
    //}

    //기즈모로 연쇄 범위 표시
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, chainRange);
    //}
}
