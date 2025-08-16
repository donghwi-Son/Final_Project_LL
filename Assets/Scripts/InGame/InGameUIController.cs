using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.Fade(Color.black, 1f, 0f, 0.5f, 0f, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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
}
