using System.IO;
using System;
using UnityEngine;

[Serializable]
public class StatisticsData
{
    public float totalPlayTime;
    public int totalDeaths;
    public int totalMonsterKills;
    public int totalUsedMoney;
    //public List<Artifact> artifacts = new List<Artifact>();
}

public class UserStatisticsData : IUserData
{
    public StatisticsData Statistics { get; set; } = new();
    private const string STATISTICS_FILE = "Statistics.json";

    public void SetDefaultData()
    {
        Debug.Log($"{GetType()}::SetDefaultData");

        Statistics = new StatisticsData
        {
            totalPlayTime = 0f,
            totalDeaths = 0,
            totalMonsterKills = 0,
            totalUsedMoney = 0
            // artifacts = new List<Artifact>() // 필요시 초기화
        };
    }

    public bool LoadData()
    {
        // 메서드 호출 로그 출력
        Debug.Log($"{GetType()}::LoadData");

        // 로드 결과를 저장할 변수
        bool result = false;

        try
        {
            string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, STATISTICS_FILE);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Statistics = JsonUtility.FromJson<StatisticsData>(json);
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

    public bool SaveData()
    {
        // 메서드 호출 로그 출력
        Debug.Log($"{GetType()}::SaveData");

        // 저장 결과를 저장할 변수
        bool result = false;

        try
        {
            if (Statistics != null)
            {
                string json = JsonUtility.ToJson(Statistics, true);
                string filePath = Path.Combine(UserDataManager.Instance.SaveFolderPath, STATISTICS_FILE);
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
