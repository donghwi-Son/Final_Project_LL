using Singleton.Component;
using UnityEngine;

public class GameManager : SingletonComponent<GameManager>
{
    public bool IsPaused { get; private set; }

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
        IsPaused = false;
        return true;
    }

    protected override void ReleaseInstance()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (Instance != this)
            Destroy(gameObject);
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame(bool _pause)
    {
        IsPaused = _pause;

        if (IsPaused)
        {
            Time.timeScale = 0f; // 게임 시간 정지
        }
        else
        {
            Time.timeScale = 1f; // 게임 시간 재개
        }
    }
}
