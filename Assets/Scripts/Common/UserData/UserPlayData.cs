using System;
using System.IO;
using UnityEngine;

public class UserPlayData : IUserData
{
    public bool ExistsSavedPlayData { get; set; }
    public Vector3 PlayerPosition { get; set; }
    public Vector3 PlayerRotation { get; set; }
    public float PlayTime { get; set; }
    public float TotalPlayTime { get; set; }
    public float NewRecord { get; set; }

    private GlobalGameRecord globalGameRecord;

    private const string GLOBAL_STATS_FILE = "GlobalGameRecord.json";

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

    // 저장된 데이터를 불러오는 메서드
    public bool LoadData()
    {
        // 메서드 호출 로그 출력
        Debug.Log($"{GetType()}::LoadData");

        // 로드 결과를 저장할 변수
        bool result = false;

        // 예외 처리를 위한 try-catch 블록 시작
        string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, GLOBAL_STATS_FILE);
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                globalGameRecord = JsonUtility.FromJson<GlobalGameRecord>(json);
            }
            else
            {
                globalGameRecord = new GlobalGameRecord();
            }
        }
        catch (Exception e)
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
            if (globalGameRecord != null)
            {
                string json = JsonUtility.ToJson(globalGameRecord, true);
                string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, GLOBAL_STATS_FILE);
                File.WriteAllText(filePath, json);

                result = true;
            }
        }
        catch (Exception e)
        {
            // 저장 실패 로그 출력
            Debug.Log($"Save failed. (" + e.Message + ")");
        }

        // 저장 결과 반환
        return result;
    }
}
