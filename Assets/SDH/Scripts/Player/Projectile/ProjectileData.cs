using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectileData", menuName = "Game/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("기본 정보")]
    public ProjectileType projectileType;
    public string projectileName;

    [Header("기본 스탯")]
    public float baseDamage = 5f;
    public float baseSpeed = 10f;
    public float baseLifeTime = 5f;
    public int basePiercingCount = 0;
}
