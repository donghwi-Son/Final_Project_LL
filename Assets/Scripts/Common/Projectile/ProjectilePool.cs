using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [System.Serializable]
    public struct ProjectileMapping
    {
        [Header("Projectile Setup")]
        public ProjectileType type;
        public GameObject prefab;
    }

    public static ProjectilePool Instance { get; private set; }


    public List<ProjectileMapping> projectileMappings;
    Dictionary<ProjectileType, Queue<GameObject>> activePool = new();
    ProjectileType currentProjectileType;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool();
            ChangeProjectile(ProjectileType.AAA);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void InitPool()
    {
        GeneratePool(ProjectileType.AAA);
    }

    public Projectile GetProjectile()
    {
        if (activePool.ContainsKey(currentProjectileType))
        {
            if (activePool[currentProjectileType].Count > 0)
            {
                GameObject obj = activePool[currentProjectileType].Dequeue();
                obj.SetActive(true);
                return obj.GetComponent<Projectile>();
            }
            else
            {
                GenerateProjectile(currentProjectileType);
                return GetProjectile();
            }
        }
        else
        {
            GeneratePool(currentProjectileType);
            return GetProjectile();
        }
    }

    public void ChangeProjectile(ProjectileType type)
    {
        if (activePool.ContainsKey(type))
        {
            currentProjectileType = type; // 현재 타입 변경
            return; // 이미 해당 타입의 풀링이 존재하면 리턴
        }
        currentProjectileType = type; // 현재 타입 변경
        GeneratePool(type);
    }

    void GeneratePool(ProjectileType type)
    {
        foreach (var data in projectileMappings)
        {
            if (data.type == type)
            {
                if (!activePool.ContainsKey(type))
                {
                    activePool[type] = new Queue<GameObject>();
                }
                for (int i = 0; i < 20; i++) // 초기 풀 크기 설정
                {
                    GameObject obj = Instantiate(data.prefab);
                    obj.transform.SetParent(transform); // 풀링 관리용으로 부모 설정
                    obj.SetActive(false);
                    Projectile projectile = obj.GetComponent<Projectile>();
                    projectile.OnProjectiledestroyed += ReturnProjectile;
                    activePool[type].Enqueue(obj);
                }
            }
        }
    }

    void GenerateProjectile(ProjectileType type)
    {
        foreach(var data in projectileMappings)
        {
            if (data.type == type)
            {
                GameObject obj = Instantiate(data.prefab);
                obj.transform.SetParent(transform); // 풀링 관리용으로 부모 설정
                obj.SetActive(false);
                Projectile projectile = obj.GetComponent<Projectile>();
                projectile.OnProjectiledestroyed += ReturnProjectile;
                activePool[type].Enqueue(obj);
                return;
            }
        }
    }

    void ReturnProjectile(Projectile projectile)
    {
        if (projectile == null) return;
        projectile.ClearEffects();
        ProjectileType type = projectile.projectileData.projectileType;
        if (activePool.ContainsKey(type))
        {
            projectile.gameObject.SetActive(false);
            activePool[type].Enqueue(projectile.gameObject);
        }
    }

}
