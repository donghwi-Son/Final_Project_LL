using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectileData", menuName = "ScriptableObject/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("기본 정보")]
    public ProjectileType projectileType;
    public string projectileName;

    [Header("기본 스탯")]
    public float damageMultiplier = 1f;
    public float speed = 10f;
    public float rateMultiplier = 1f;
    public int piercingCount = 0;
}
