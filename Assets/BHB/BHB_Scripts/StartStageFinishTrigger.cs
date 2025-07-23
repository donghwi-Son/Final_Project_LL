using UnityEngine;

public class StartStageFinishTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        Invoke(nameof(GenerateRoomDelayed), 0.1f);
    }

    private void GenerateRoomDelayed()
    {
        StageManager.Instance.GenerateFirstRoom(); // 첫 방 생성
    }
}
