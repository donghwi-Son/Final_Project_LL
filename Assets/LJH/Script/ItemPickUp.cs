using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemIndex;
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        var def = ItemDatabase.Instance.GetDefinition(itemIndex);
        if (_sr != null && def.icon != null)
            _sr.sprite = def.icon;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerController.Instance.OnItemAcquired(itemIndex);
            Destroy(gameObject);
        }
    }
    
    // public void OnItemAcquired(int idx)      //나중에 플레이어에 넣어야 하는 부분 아이템 획득 적용하는 부분
    // {
    //     if (!acquired.Add(idx)) 
    //         return;
    //
    //     // 정의 꺼내서 효과 적용
    //     var def = ItemDatabase.Instance.GetDefinition(idx);
    //     ItemEffectFactory.ApplyEffect(def);
    //
    //     // 획득 목록 저장
    //     SaveAcquiredSet();
    // }
    //
    // private void SaveAcquiredSet()
    // {
    //     // PlayerPrefs 방식으로 acquired 저장
    // }
}
