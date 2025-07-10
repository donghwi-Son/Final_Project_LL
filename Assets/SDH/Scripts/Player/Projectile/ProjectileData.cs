using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectileData", menuName = "ScriptableObject/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("기본 정보")]
    public ProjectileType projectileType;
    public string projectileName;

    [Header("기본 스탯")]
    public float damageMultiplier = 1f;
    public float speedMultiplier = 10f;
    public float rateMultiplier = 1f;
    public int piercingCount = 0;

    [Header("속성에 따른 스프라이트")]
    public Sprite projectileSprite;
    public Sprite iceSprite;
    public Sprite fireSprite;
    public Sprite poisonSprite;

}
