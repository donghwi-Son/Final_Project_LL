using Unity.VisualScripting;
using UnityEngine;

public class ItemEffectFactory : MonoBehaviour
{
    public static void ApplyEffect(ItemDefinition def)
    {
        switch (def.upgradeType)
        {
            case ItemInfo.ItemUpgradeType.StatIncrease:
                Debug.Log("스탯 증가 아이템 획득");
                //플레이어 스탯 들고와서 증가
                break;
            
            case ItemInfo.ItemUpgradeType.AttackEnhance:
                Debug.Log("기본 공격 강화 아이템 획득");
                break;
            
            case ItemInfo.ItemUpgradeType.SkillUpgrade:
                Debug.Log("스킬 강화 아이템 획득");
                break;
                

            case ItemInfo.ItemUpgradeType.SubAttack:
                Debug.Log("보조 공격 아이템 획득");
                break;
                
            case ItemInfo.ItemUpgradeType.Utility:
                Debug.Log("유틸 아이템 획득");
                break;
        }
    }
}
