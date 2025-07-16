using UnityEngine;

public class StartStageFinishTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StageManager.Instance.GenerateFirstRoom(); // 첫 방 생성
    }
}
