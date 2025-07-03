using UnityEngine;

public class UserPlayData : IUserData
{
    public bool ExistsSavedPlayData { get; set; }
    public Vector3 PlayerPosition { get; set; }
    public Vector3 PlayerRotation { get; set; }
    public float PlayTime { get; set; }
    public float TotalPlayTime { get; set; }
    public float NewRecord { get; set; }

    // 기본 데이터 설정 메서드
    public void SetDefaultData()
    {
        Debug.Log($"{GetType()}::SetDefaultData");

        // 기본값으로 데이터 초기화
        ExistsSavedPlayData = false;
        PlayerPosition = new Vector3(0, 75f, 0f);
        PlayerRotation = new Vector3(0f, 180f, 0f);
        PlayTime = 0f;
        TotalPlayTime = 0f;
        NewRecord = Mathf.Infinity;
    }

    public void SoftResetData()
    {
        Debug.Log($"{GetType()}::SoftResetData");

        // 소프트 리셋: 플레이 시간과 플레이어 위치만 초기화
        ExistsSavedPlayData = false;
        PlayTime = 0f;
        PlayerPosition = new Vector3(0, 75f, 0f);
        PlayerRotation = new Vector3(0f, 180f, 0f);
    }

    // 저장된 데이터를 불러오는 메서드
    public bool LoadData()
    {
        // 메서드 호출 로그 출력
        Debug.Log($"{GetType()}::LoadData");

        // 로드 결과를 저장할 변수
        bool result = false;

        // 예외 처리를 위한 try-catch 블록 시작
        try
        {
            // PlayerPrefs에서 저장된 데이터 불러오기
            ExistsSavedPlayData = PlayerPrefs.GetInt("ExistsSavedPlayData") == 1 ? true : false;
            PlayerPosition = new Vector3(
                PlayerPrefs.GetFloat("PlayerPositionX"),
                PlayerPrefs.GetFloat("PlayerPositionY"),
                PlayerPrefs.GetFloat("PlayerPositionZ")
            );
            PlayerRotation = new Vector3(
                PlayerPrefs.GetFloat("PlayerRotationX"),
                PlayerPrefs.GetFloat("PlayerRotationY"),
                PlayerPrefs.GetFloat("PlayerRotationZ")
            );
            PlayTime = PlayerPrefs.GetFloat("PlayTime");
            TotalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime");
            NewRecord = PlayerPrefs.GetFloat("NewRecord");

            // 로드 성공으로 설정
            result = true;
        }
        // 예외 발생 시 처리
        catch (System.Exception e)
        {
            // 로드 실패 로그 출력
            Debug.Log($"Load failed. (" + e.Message + ")");
        }

        // 로드 결과 반환
        return result;
    }

    // 데이터를 저장하는 메서드
    public bool SaveData()
    {
        // 메서드 호출 로그 출력
        Debug.Log($"{GetType()}::SaveData");

        // 저장 결과를 저장할 변수
        bool result = false;

        // 예외 처리를 위한 try-catch 블록 시작
        try
        {
            // PlayerPrefs에 현재 플레이 데이터 저장
            PlayerPrefs.SetInt("ExistsSavedPlayData", ExistsSavedPlayData ? 1 : 0);

            PlayerPrefs.SetFloat("PlayerPositionX", PlayerPosition.x);
            PlayerPrefs.SetFloat("PlayerPositionY", PlayerPosition.y);
            PlayerPrefs.SetFloat("PlayerPositionZ", PlayerPosition.z);

            PlayerPrefs.SetFloat("PlayerRotationX", PlayerRotation.x);
            PlayerPrefs.SetFloat("PlayerRotationY", PlayerRotation.y);
            PlayerPrefs.SetFloat("PlayerRotationZ", PlayerRotation.z);

            PlayerPrefs.SetFloat("PlayTime", PlayTime);
            PlayerPrefs.SetFloat("TotalPlayTime", TotalPlayTime);
            PlayerPrefs.SetFloat("NewRecord", NewRecord);

            // PlayerPrefs 저장 실행
            PlayerPrefs.Save();

            // 저장 성공으로 설정
            result = true;
        }
        // 예외 발생 시 처리
        catch (System.Exception e)
        {
            // 저장 실패 로그 출력
            Debug.Log($"Save failed. (" + e.Message + ")");
        }

        // 저장 결과 반환
        return result;
    }
}
