// SubAttackSystem.cs
using UnityEngine;

public class SubAttackSystem : MonoBehaviour
{
    public static SubAttackSystem Instance { get; private set; }
    private float nextFireTime;

    [SerializeField] private Transform attackSpawnPoint; // 플레이어 앞 위치
    [SerializeField] private LayerMask enemyLayer;

    private GameObject currentSubAttackPrefab;
    private float cooldown;
    private int damage;

    void Awake()
    {
        Instance = this;
    }

    public void SubAttackRegister(GameObject prefab, float cd, int dmg)
    {
        currentSubAttackPrefab = prefab;
        cooldown = cd;
        damage = dmg;
    }

    void Update()
    {
        if (currentSubAttackPrefab == null) return;

        if (Time.time >= nextFireTime)
        {
            // 쿨타임마다 자동 발동
            DoSubAttack();
            nextFireTime = Time.time + cooldown;
        }
    }

    private void DoSubAttack()
    {
        Vector3 spawnPos = attackSpawnPoint.position;
        GameObject atk = Instantiate(currentSubAttackPrefab, spawnPos, Quaternion.identity);

        // 바라보는 방향 적용
        atk.transform.localScale = new Vector3(
            Mathf.Sign(transform.localScale.x) * atk.transform.localScale.x,
            atk.transform.localScale.y,
            atk.transform.localScale.z
        );

        atk.GetComponent<SubAttackDamage >()?.Init(damage, 0.2f, enemyLayer);
    }
}