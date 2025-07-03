using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private HashSet<int> acquired = new HashSet<int>();
    
    public static PlayerTest Instance { get; private set; }
    
    private float maxHealth = 50f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnItemAcquired(0);
            OnItemAcquired(1);
            OnItemAcquired(2);
            OnItemAcquired(3);
            OnItemAcquired(4);
            OnItemAcquired(5);
        }
    }
    
    public void OnItemAcquired(int idx)      //나중에 플레이어에 넣어야 하는 부분 아이템 획득 적용하는 부분
    {
        if (!acquired.Add(idx)) 
            return;
    
        // 정의 꺼내서 효과 적용
        var def = ItemDatabase.Instance.GetDefinition(idx);
        ItemEffectFactory.ApplyEffect(def);
        SynergyManager.Instance.OnItemAcquired(def);
    
        // 획득 목록 저장
        SaveAcquiredSet();
    }
    
    private void SaveAcquiredSet()
    {
        // PlayerPrefs 방식으로 acquired 저장, 저장 필요 없으면 안할듯
    }

    public void AddHealth(float amount)
    {
        maxHealth += amount;
        Debug.Log(maxHealth);
    }

    public void AttackEnhance(ItemInfo.AttackEnhanceType enhanceType)
    {
        //공격 강화
    }

    public void AddSkill(ItemInfo.SkillType skilltype)
    {
        //기존 스킬에서 얻은 스킬로 변경
    }

    public void SubAttackRegister(ItemInfo.SubAttackType subType)
    {
        //서브 공격 동작
    }

    public void EnableDoubleJump()
    {
        //더블점프 활성화
    }
}
