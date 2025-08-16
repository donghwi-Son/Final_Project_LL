using System.Collections;
using UnityEngine;

public class TaoistSpecialAttack : MonoBehaviour
{
    private Transform playerTrans;
    private Collider2D cd;
    private SpriteRenderer sr;
    private CharacterStats parentsStats;

    [SerializeField] private float playerHeightSpace;
    [SerializeField] private float duration;
    [SerializeField] private float stopTiming;
    [SerializeField] private float activeTiming;
    [SerializeField] private Transform posA;
    [SerializeField] private Transform posB;

    [Header("스케일 애니메이션")]
    [SerializeField] private float growSpeed = 5f;     // 커지는 속도
    [SerializeField] private float shrinkSpeed = 15f;  // 줄어드는 속도
    [SerializeField] private float maxScaleX = 2f;     // 최대 X 스케일

    private bool isActive;

    private LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        cd = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (playerTrans == null)
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        Destroy(gameObject, duration);

        isActive = false;
    }

    private void Update()
    {
        if (!isActive)
        {
            if(stopTiming > 0)
            {
                stopTiming -= Time.deltaTime;

                Vector3 newDir = new Vector3(playerTrans.position.x, playerTrans.position.y + playerHeightSpace, playerTrans.position.z);
                transform.position = newDir;

                lineRenderer.SetPosition(0, posA.position);
                lineRenderer.SetPosition(1, posB.position);
            }
            else
            {
                activeTiming -= Time.deltaTime;

                if (activeTiming <= 0)
                {
                    BossBeamTrajectorySwitch(false);
                    isActive = true;
                    cd.enabled = true;
                    sr.enabled = true;
                    StartCoroutine(ScaleZEffect()); // 스케일 애니메이션 시작
                }
            }
        }
    }

    public void SetParentsStats(CharacterStats stats)
    {
        parentsStats = stats;
    }

    public void BossBeamTrajectorySwitch(bool value)
    {
        lineRenderer.enabled = value;
    }

    private IEnumerator ScaleZEffect()
    {
        Vector3 scale = transform.localScale;
        float originalX = scale.x;
        float targetX = originalX * maxScaleX; // 최대로 커질 비율
        float minX = originalX * 0.1f;    // 최소 크기 (원래의 10%)
        float growTime = 0.15f;
        float shrinkTime = 0.08f;

        // 1. 0.15초 동안 증가
        float t = 0f;
        while (t < growTime)
        {
            t += Time.deltaTime;
            float progress = t / growTime;
            scale.x = Mathf.Lerp(originalX, targetX, progress);
            transform.localScale = scale;
            yield return null;
        }

        // 2. 0.15초 동안 감소
        t = 0f;
        while (t < shrinkTime)
        {
            t += Time.deltaTime;
            float progress = t / shrinkTime;
            scale.x = Mathf.Lerp(targetX, minX, progress);
            transform.localScale = scale;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어 데미지 로직
            parentsStats.DoDamage(collision.GetComponent<PlayerStats>());

            Destroy(gameObject);
        }
    }
}
