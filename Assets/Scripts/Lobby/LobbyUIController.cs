using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private EventSystem m_EventSystem;
    [SerializeField] private GameObject m_ContinueBtn;
    [SerializeField] private GameObject m_NewGameBtn;

    private void Start()
    {
        var userPlayData = UserDataManager.Instance.GetUserData<UserPlayData>();
        if (userPlayData != null && userPlayData.ExistsSavedPlayData)
        {
            m_ContinueBtn.SetActive(true);
            m_EventSystem.SetSelectedGameObject(m_ContinueBtn, null);
        }
        else
        {
            m_ContinueBtn.SetActive(false);
            m_EventSystem.SetSelectedGameObject(m_NewGameBtn, null);
        }

        UIManager.Instance.Fade(Color.black, 1f, 0f, 0.5f, 0f, true);
    }

    public void OnClickContinueButton()
    {
        // 저장된 게임을 로드
        //GameManager.Instance.LoadPlayData(); // 플레이 데이터 로드
        //UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        //{
        //    SceneLoader.Instance.LoadScene(SceneType.MainScene);
        //});
    }

    public void OnClickNewGameButton()
    {
        // 새로운 게임 시작
        //GameManager.Instance.ResetPlayData(); // 플레이 데이터 초기화
        //GameManager.Instance.LoadPlayData(); // 플레이 데이터 로드
        //UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        //{
        //    SceneLoader.Instance.LoadScene(SceneType.InGame);
        //});
    }

    public void OnClickSettingsButton()
    {
        // 설정 UI 열기
        //var uiData = new UIBaseData();
        //UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickQuitButton()
    {
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            GameManager.Instance.QuitGame();
        });
    }
}
