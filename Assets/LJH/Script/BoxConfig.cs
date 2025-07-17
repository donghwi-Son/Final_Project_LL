using UnityEngine;

[CreateAssetMenu(fileName = "BoxConfig", menuName = "Configs/BoxConfig")]
public class BoxConfig : ScriptableObject       //아이템 확률 설정
{
    [System.Serializable]
    public struct RarityRate
    {
        public ItemInfo.ItemRarity rarity;  
        [Range(0,100)] public float rate;  
    }

    public RarityRate[] rates; 
        
}
