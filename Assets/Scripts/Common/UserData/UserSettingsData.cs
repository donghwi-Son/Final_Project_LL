using System;
using System.IO;
using UnityEngine;

public enum LocalizationLanguage
{
    English,
    Korean,
}

[Serializable]
public class GameSettings
{
    public LocalizationLanguage Language;
    public int ResolutionIndex;
    public bool FullScreen;
    public float Music_Volume;
    public float SFX_Volume;
}

public class UserSettingsData : IUserData
{
    public GameSettings Settings { get; set; } = new();

    private const string SETTINGS_FILE = "Settings.json";

    public void SetDefaultData()
    {
        Debug.Log($"{GetType()}::SetDefaultData");

        Settings = new GameSettings
        {
            Language = LocalizationLanguage.English, // 기본 언어 설정
            ResolutionIndex = 0, // 기본 해상도 인덱스
            FullScreen = true, // 전체 화면 모드 기본값
            Music_Volume = 1.0f, // 음악 볼륨 기본값
            SFX_Volume = 1.0f // 효과음 볼륨 기본값
        };
    }

    public bool LoadData()
    {
        Debug.Log($"{GetType()}::LoadData");

        bool result = false; // 로드 결과 저장용 변수

        try
        {
            string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, SETTINGS_FILE);
            string json = File.ReadAllText(filePath);
            Settings = JsonUtility.FromJson<GameSettings>(json);

            result = true; // 로드 성공
        }
        catch (Exception e)
        {
            // 로드 실패 처리
            Debug.Log("Load failed (" + e.Message + ")");
        }

        return result; // 로드 결과 반환
    }

    public bool SaveData()
    {
        Debug.Log($"{GetType()}::SaveData");

        bool result = false; // 저장 결과 저장용 변수

        try
        {
            string json = JsonUtility.ToJson(Settings, true);
            string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, SETTINGS_FILE);
            File.WriteAllText(filePath, json);

            result = true; // 저장 성공
        }
        catch (Exception e)
        {
            // 저장 실패 처리
            Debug.Log("Save failed (" + e.Message + ")");
        }

        return result; // 저장 결과 반환
    }
}
