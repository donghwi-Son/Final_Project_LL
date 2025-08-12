using UnityEngine;

public class PauseUI : UIBase
{
    public override void OnClickCloseButton()
    {
        // 게임 일시 정지 해제
        GameManager.Instance.PauseGame(false);
        base.OnClickCloseButton();
    }

    public void OnClickSettingsButton()
    {
        // 설정 UI 열기
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickReturnToLobbyButton()
    {
        // 게임 일시 정지 해제
        GameManager.Instance.PauseGame(false);

        // 로비로 돌아가기
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        });
    }

    public void OnClickQuitButton()
    {
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            GameManager.Instance.QuitGame();
        });
    }
}
