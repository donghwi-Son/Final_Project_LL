using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public enum ItemRarity
    {
        Common,             //5등급
        Uncommon,           //4등급
        Rare,               //3등급
        Epic,               //2등급
        Legendary,          //1등급
        Boss                //보스 유물
    }
    
    public enum ItemUpgradeType
    {
        StatIncrease,    // 스탯 증가 (공격력, 방어력, 속도 등등)
        AttackEnhance,   // 공격 강화 (스킬 강화)
        SubAttack,       // 서브 어택 (기본 공격, 스킬 이외의 공격)
        Utility          // 유틸 (유틸 관련)
    }
    
    public enum StatType    //이후 플레이어 스탯을 상속 받아서 진행
    {
        Health,
        Defense,
        MoveSpeed
    }
    
    [System.Serializable]
    public class EffectData
    {
        public ItemUpgradeType type;

        // 스탯 증가
        public float statAmount;                //증가량
        public StatType statType;               //스탯 종류

        // 공격 강화 부분은 스킬 보고 해야할 듯

        // 서브 어택
        public GameObject subAttackPrefab;          //보조 공격 수단 프리펩
        public float subAttackCooldown;             //클타임

        // 유틸도 이후에 진행
    }
}
