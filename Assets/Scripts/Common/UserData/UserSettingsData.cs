using UnityEngine;

public class UserSettingsData : IUserData
{
    public int ResolutionIndex { get; set; }
    public bool FullScreen { get; set; }
    public float Music_Volume { get; set; } = 1.0f;
    public float SFX_Volume { get; set; } = 1.0f;

    // 기본값으로 초기화
    public void SetDefaultData()
    {
        Debug.Log($"{GetType()}::SetDefaultData");

        ResolutionIndex = 0; // 기본 해상도 인덱스 (예: 첫 번째 해상도)
        FullScreen = true; // 기본 전체 화면 모드
        Music_Volume = 1.0f; // 기본 BGM 볼륨
        SFX_Volume = 1.0f; // 기본 SFX 볼륨
    }

    // PlayerPrefs에서 설정 데이터 로드
    public bool LoadData()
    {
        Debug.Log($"{GetType()}::LoadData");

        bool result = false; // 로드 결과 저장용 변수

        try
        {
            // PlayerPrefs에서 저장된 값 불러오기
            ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            FullScreen = PlayerPrefs.GetInt("FullScreen") == 1 ? true : false;
            Music_Volume = PlayerPrefs.GetFloat("Music");
            SFX_Volume = PlayerPrefs.GetFloat("SFX");
            result = true; // 로드 성공
        }
        catch (System.Exception e)
        {
            // 로드 실패 처리
            Debug.Log("Load failed (" + e.Message + ")");
        }

        return result; // 로드 결과 반환
    }

    // PlayerPrefs에 설정 데이터 저장
    public bool SaveData()
    {
        Debug.Log($"{GetType()}::SaveData");

        bool result = false; // 저장 결과 저장용 변수

        try
        {
            // PlayerPrefs에 현재 설정 값 저장
            PlayerPrefs.SetInt("ResolutionIndex", ResolutionIndex);
            PlayerPrefs.SetInt("FullScreen", FullScreen ? 1 : 0);
            PlayerPrefs.SetFloat("Music", Music_Volume);
            PlayerPrefs.SetFloat("SFX", SFX_Volume);
            PlayerPrefs.Save(); // 변경사항을 디스크에 저장

            result = true; // 저장 성공
        }
        catch (System.Exception e)
        {
            // 저장 실패 처리
            Debug.Log("Save failed (" + e.Message + ")");
        }

        return result; // 저장 결과 반환
    }
}
