using UnityEngine;

public class NpcInteractionCondition : MonoBehaviour, IRoomCondition
{
    private bool interacted = false;

    public bool IsConditionMet() => interacted;

    public void Setup(GameObject roomInstance)
    {
        // 여기에 enemy 관련 조건 로직...
    }

    public void OnPlayerInteract()
    {
        if (!interacted)
        {
            // NPC 상호작용 처리 (대사 등은 여기에)
            interacted = true;
        }
    }
}
