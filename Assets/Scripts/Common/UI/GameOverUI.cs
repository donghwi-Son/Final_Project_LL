using System.Collections;
using UnityEngine;

public class GameOverUI : UIBase
{
    [SerializeField] private Animation gameOverAnim; // 게임 오버 애니메이션 컴포넌트

    public override void SetInfo(UIBaseData uiData)
    {
        base.SetInfo(uiData);

        StartCoroutine(GameOverCo()); // 게임 오버 코루틴 시작
    }

    private IEnumerator GameOverCo()
    {
        // 게임 오버 애니메이션 재생
        gameOverAnim.Play();

        // 애니메이션 재생 시간만큼 대기
        yield return new WaitForSeconds(gameOverAnim.clip.length);

        UIManager.Instance.CloseAllOpenUI();
        SceneLoader.Instance.LoadScene(SceneType.Lobby);
    }
}
