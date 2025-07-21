using Singleton.Component;
using UnityEngine;
using UnityEngine.Localization.Settings;

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

    public void LoadPlayerSettings()
    {
        // 플레이어 설정 데이터 로드
        var userSettingsData =  UserDataManager.Instance.GetUserData<UserSettingsData>();
        if(userSettingsData != null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)userSettingsData.Settings.Language];
            AudioManager.Instance.SetMusicVolume(userSettingsData.Settings.Music_Volume);
            AudioManager.Instance.SetSFXVolume(userSettingsData.Settings.SFX_Volume);
            Screen.fullScreen = userSettingsData.Settings.FullScreen;
            if (userSettingsData.Settings.ResolutionIndex >= 0 && userSettingsData.Settings.ResolutionIndex < Screen.resolutions.Length)
            {
                Resolution resolution = Screen.resolutions[userSettingsData.Settings.ResolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, userSettingsData.Settings.FullScreen);
            }
            else
            {
                Debug.LogWarning("Invalid resolution index in user settings.");
            }
        }
    }

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
