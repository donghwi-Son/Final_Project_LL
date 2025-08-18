using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject bossUI;

    private void Start()
    {
        UIManager.Instance.Fade(Color.black, 1f, 0f, 0.5f, 0f, true);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnEnable()
    {
        StageManager.Instance.OnEnterBoss += ShowBossUI;
    }

    private void OnDisable()
    {
        StageManager.Instance.OnEnterBoss -= ShowBossUI;
    }

    private void TogglePause()
    {
        bool pause = !GameManager.Instance.IsPaused;

        if (pause)
        {
            var uiData = new UIBaseData();
            UIManager.Instance.OpenUI<PauseUI>(uiData);
        }
        else
        {
            UIManager.Instance.CloseAllOpenUI();
        }

        GameManager.Instance.PauseGame(pause);
    }

    private void ShowBossUI()
    {
        if (bossUI != null)
        {
            bossUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Boss UI is not assigned in InGameUIController.");
        }
    }
}
