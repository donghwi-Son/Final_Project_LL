using UnityEngine;
using System.Collections.Generic;

public class ChainLightning : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public ParticleSystem hitEffectPrefab;
    public int maxChains = 5;
    public float chainRange = 5f;

    public void Fire(Vector3 start, List<Transform> targets)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 currentPos = start;
        points.Add(currentPos);

        for (int i = 0; i < maxChains && targets.Count > 0; i++)
        {
            // 가장 가까운 타겟 찾기
            Transform nearest = null;
            float minDist = float.MaxValue;
            foreach (var t in targets)
            {
                float dist = Vector3.Distance(currentPos, t.position);
                if (dist < minDist && dist <= chainRange)
                {
                    minDist = dist;
                    nearest = t;
                }
            }
            if (nearest == null) break;

            // 번개 경로에 포인트 추가 (중간에 랜덤 흔들기)
            Vector3 mid = Vector3.Lerp(currentPos, nearest.position, 0.5f) +
                          Random.insideUnitSphere * 0.2f;
            points.Add(mid);
            points.Add(nearest.position);

            // 파티클 효과 생성
            Instantiate(hitEffectPrefab, nearest.position, Quaternion.identity);

            currentPos = nearest.position;
            targets.Remove(nearest);
        }

        // LineRenderer에 포인트 적용
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}