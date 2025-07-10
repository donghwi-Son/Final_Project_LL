using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectilePoolData", menuName = "ScriptableObject/ProjectilePoolData")]
public class ProjectilePoolData : ScriptableObject
{
    public ProjectileType projectileType;
    public GameObject projectilePrefab;
    public int initialPoolSize = 15;
}
