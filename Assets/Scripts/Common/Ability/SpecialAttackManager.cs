using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackManager : MonoBehaviour
{
    public static SpecialAttackManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // 체인 라이트닝 =================================================

    public int maxTarget = 3;
    public float chainRange = 5f;

    public GameObject FindNearestTarget(GameObject currentEnemy, List<GameObject> hitTargets)
    {
        GameObject nearestTarget = null;
        float minDistance = float.MaxValue;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(currentEnemy.transform.position, chainRange, LayerMask.GetMask("Enemy"));
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject == currentEnemy) continue; // 현재 적은 제외
            if (hitTargets.Contains(enemy.gameObject)) continue; // 이미 맞은 적은 제외
            float distance = Vector3.Distance(currentEnemy.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = enemy.gameObject;
            }
        }
        return nearestTarget;
    }

    public void StartChain(GameObject currentEnemy, float dmg)
    {
        LineRenderer lineRenderer = EffectPool.Instance.GetLightningLine();
        List<GameObject> hitTargets = new List<GameObject>();
        List<Vector3> points = new List<Vector3>();

        Vector3 currentPos = currentEnemy.transform.position;
        points.Add(currentPos);
        hitTargets.Add(currentEnemy);
        PlayLightningEffect(currentPos);

        GameObject nextTarget;
        while (hitTargets.Count < maxTarget)
        {
            nextTarget = FindNearestTarget(currentEnemy, hitTargets);
            if (nextTarget == null) break; // 더 이상 타겟이 없으면 종료

            // 히트 이펙트 생성
            PlayLightningEffect(nextTarget.transform.position);

            // 다음 타겟 위치 추가
            Vector3 nextPos = nextTarget.transform.position;
            points.Add(nextPos);
            hitTargets.Add(nextTarget);

            // 현재 타겟을 다음 타겟으로 업데이트
            currentEnemy = nextTarget;
        }

        foreach (var target in hitTargets)
        {
            // 각 타겟에 데미지 적용
            Debug.Log($"Hit Target: {target.name}, Damage: {dmg / 5}");
        }

        // LineRenderer에 포인트 적용
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        // 독립적인 코루틴으로 정리 (각각의 lineRenderer 참조를 캡처)
        StartCoroutine(Clear(lineRenderer));
    }

    void PlayLightningEffect(Vector3 pos)
    {
        ParticleSystem lightningEffect = EffectPool.Instance.GetLightningEffect();
        lightningEffect.transform.position = pos;
        lightningEffect.Play();
        StartCoroutine(ClearHitEffect(lightningEffect));
    }

    IEnumerator Clear(LineRenderer lineRenderer)
    {
        yield return new WaitForSeconds(0.1f);
        // 각각의 lineRenderer를 독립적으로 반환
        if (lineRenderer != null)
        {
            EffectPool.Instance.ReturnLightningLine(lineRenderer);
        }
    }

    IEnumerator ClearHitEffect(ParticleSystem lightningEffect)
    {
        yield return new WaitForSeconds(lightningEffect.main.duration);
        if (lightningEffect != null)
        {
            EffectPool.Instance.ReturnLightningEffect(lightningEffect);
        }
    }


    //얼음 ================================================================

    public void SpawnIce(GameObject enemy)
    {
        ParticleSystem iceEffect = EffectPool.Instance.GetIceEffect();
        if (iceEffect != null)
        {
            iceEffect.transform.position = enemy.transform.position;
            iceEffect.Play();
            StartCoroutine(ReturnIce(iceEffect));
        }

        // 적 얼리기? 같은거
    }

    IEnumerator ReturnIce(ParticleSystem parti)
    {
        yield return new WaitForSeconds(parti.main.duration);
        EffectPool.Instance.ReturnIceEffect(parti);
    }

    // 독 ================================================================

    Dictionary<GameObject, Coroutine> poisonCoroutines = new Dictionary<GameObject, Coroutine>();

    public void SpawnPoison(GameObject enemy, float dmg)
    {
        var existing = enemy.gameObject.GetComponentInChildren<ParticleSystem>();


        if (poisonCoroutines.ContainsKey(enemy))
        {
            if (poisonCoroutines[enemy] != null)
                {
                    StopCoroutine(poisonCoroutines[enemy]);
                }
            poisonCoroutines.Remove(enemy);
        }

        if (existing != null)
        {
            existing.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            existing.Play();

            // 새로운 코루틴 시작하고 딕셔너리에 저장
            Coroutine newCoroutine = StartCoroutine(ReturnPoison(existing, enemy));
            poisonCoroutines[enemy] = newCoroutine;
            return;
        }
        else
        {
            ParticleSystem poisonEffect = EffectPool.Instance.GetPoisonEffect();
            if (poisonEffect != null)
            {
                poisonEffect.transform.position = enemy.transform.position;
                poisonEffect.transform.SetParent(enemy.transform);
                poisonEffect.Play();

                // 새로운 코루틴 시작하고 딕셔너리에 저장
                Coroutine newCoroutine = StartCoroutine(ReturnPoison(poisonEffect, enemy));
                poisonCoroutines[enemy] = newCoroutine;
            }
        }

        
        // 적 독걸기? 같은거 이미 중독일때 맞으면 시간만 갱신
    }

    IEnumerator ReturnPoison(ParticleSystem parti, GameObject enemy)
    {
        yield return new WaitForSeconds(parti.main.duration);

        // 딕셔너리에서 제거
        if (poisonCoroutines.ContainsKey(enemy))
        {
            poisonCoroutines.Remove(enemy);
        }

        EffectPool.Instance.ReturnPoisonEffect(parti);
    }

    // 폭발 ================================================================

    public void SpawnExplosive(GameObject enemy)
    {
        GameObject explosiveEffect = EffectPool.Instance.GetExplosiveEffect();
        if (explosiveEffect != null)
        {
            explosiveEffect.transform.position = enemy.transform.position;
            explosiveEffect.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.transform.position, 0.5f, LayerMask.GetMask("Enemy"));
            float dmg = PlayerStatus.Instance.damage.GetValue();
            foreach (var collider in colliders)
            {
                    Debug.Log($"폭발 Enemy: {collider.gameObject.name}, Damage: {dmg}");
            }
        }
    }

}
