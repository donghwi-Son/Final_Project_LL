using UnityEngine;

public class EnemyClearCondition : MonoBehaviour, IRoomCondition
{
    public void Setup(GameObject roomInstance)
    {
        // 여기에 enemy 관련 조건 로직...
    }

    public bool IsConditionMet()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    public void OnPlayerInteract()
    {
        // 테스트용 적 전부 제거
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }
}
