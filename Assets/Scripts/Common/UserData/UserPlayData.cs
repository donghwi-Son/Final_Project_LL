using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float playTime;
    public int monsterKills;
    public int usedMoney;
    public int RoomID;
    //public List<ItemData> items = new();
}

[Serializable]
public class GameData
{
    public PlayerData singleGameRecord = new();
    public string lastSaved;
}

public class UserPlayData : IUserData
{
    public GameData currentGameData;

    public PlayerData SingleGameRecord { get; set; } = new();

    private const string SAVE_FILE = "GameSave.json";

    // 기본 데이터 설정 메서드
    public void SetDefaultData()
    {
        Debug.Log($"{GetType()}::SetDefaultData");

        // 기본값으로 데이터 초기화
        SingleGameRecord = new PlayerData
        {
            playTime = 0f,
            monsterKills = 0,
            usedMoney = 0,
            RoomID = 0,
            // items = new List<ItemData>() // 필요시 초기화
        };
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
            string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, SAVE_FILE);
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                currentGameData = JsonUtility.FromJson<GameData>(jsonData);

                result = true;
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
            if(currentGameData != null)
            {
                currentGameData.lastSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string jsonData = JsonUtility.ToJson(currentGameData, true);
                string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, SAVE_FILE);
                File.WriteAllText(filePath, jsonData);

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
