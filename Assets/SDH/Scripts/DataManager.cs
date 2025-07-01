using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GlobalGameStats
{
    public float totalPlayTime;
    public int totalDeaths;
    public int totalMonsterKills;
    public float totalFoodConsumed;
    public float totalWaterConsumed;
    public int totalItemsCrafted;
    public float totalDistanceTraveled;
}
[Serializable]
public class SurvivalStats
{
    // 생존 통계
    public float totalSurvivedTime;
    public int totalDayCount;
    public int totalDeaths;
    public int gamesPlayed;
    public float totalDistanceTraveled;

    // 전투 통계
    public int totalMonsterKills;
    public int totalDamageTaken;
    public int totalDamageDealt;

    // 자원 소비 통계
    public float totalFoodConsumed;
    public float totalWaterConsumed;
    public int totalItemsCrafted;

    // 기타 통계
    // 업적 퀘스트같은거 생기면 추가?
}

[Serializable]
public class PlayerInfo
{
    public float transformX;
    public float transformY;
    public float transformZ;
    public float currentHealth;
    public float currentHunger;
    public float currentThirst;
    public bool isHungerDebuffed;
    public bool isThirstDebuffed;
    public int playerLevel;
    public float experience;
    public int skillPoints;
    public int money;
}

[Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public AchievementType type;
    public float targetValue;
    public float currentProgress;
    public bool isUnlocked;
    public string unlockedDate;
    public int rewardMoney;
    public int rewardSkillPoints;

    public Achievement(string id, string title, string description, AchievementType type, float targetValue, int rewardMoney, int rewardSkillPoints)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.type = type;
        this.targetValue = targetValue;
        this.rewardMoney = rewardMoney;
        this.rewardSkillPoints = rewardSkillPoints;
        this.currentProgress = 0f;
        this.isUnlocked = false;
        this.unlockedDate = string.Empty;
    }
}

public enum AchievementType
{
    Survival,      // 생존
    Combat,        // 전투
    Crafting,      // 제작
    Exploration,   // 탐험
    Collection,    // 수집
    Special        // 특수
}

[Serializable]
public class GameSettings
{
    // 오디오 설정
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    // 화면 설정
    public bool isFullscreen = true;
    public int qualityLevel;

    // 기타 설정
    public float mouseSensitivity = 1f;
    public bool autoSave = true;
    public float autoSaveInterval = 300f;
}

[Serializable]
public class SaveSlotInfo
{
    public int slotIndex;
    public bool isEmpty;
    public string saveName;
    public string lastSaved;
    public int playerLevel;
    public int currentDay;
    public float totalPlayTime;
    public string previewImagePath;
}

[Serializable]
public class GameData
{
    public SurvivalStats stats = new();
    public PlayerInfo player = new();
    public List<Achievement> achievements = new();
    public GameSettings settings = new();
    public List<SaveSlotInfo> saveSlots = new();
    public string lastSaved;
}


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("설정")]
    [SerializeField] private int maxSaveSlots = 3;
    [SerializeField] private bool enableDebugLog = true;
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveInterval = 300f;

    public GameData currentGameData;
    private GameData gameDataforGlobal;
    private GlobalGameStats globalStats;
    private string saveFolderPath;
    private const string SAVE_FOLDER = "SaveData";
    private const string SAVE_FILE_NAME = "GameSave_Slot_";
    private const string SAVE_FILE_EXTENSION = ".json";
    private const string SETTINGS_FILE = "GameSettings.json";
    private const string GLOBAL_STATS_FILE = "GlobalGameStats.json";

    // 이벤트 시스템
    public event Action<Achievement> OnAchievementUnlocked;
    public event Action<int> OnLevelUp;
    public event Action OnDataLoaded;
    public event Action OnDataSaved;
    public event Action<string> OnStatUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupPaths();
            InitializeData();
            LoadSettings();
            LoadGlobalStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void SetupPaths()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);

        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);
    }

    private void InitializeData()
    {
        currentGameData = new GameData();
        gameDataforGlobal = new GameData();

        for (int i = 0; i < maxSaveSlots; i++)
        {
            currentGameData.saveSlots.Add(new SaveSlotInfo
            {
                slotIndex = i,
                isEmpty = true,
                saveName = $"Save Slot {i + 1}",
                lastSaved = string.Empty,
                playerLevel = 1,
                currentDay = 1,
                totalPlayTime = 0f,
                previewImagePath = string.Empty
            });
        }
    }

    public void SaveGameData(int slotIndex)
    {
        if (currentGameData == null) return;
        if (slotIndex < 0 || slotIndex >= maxSaveSlots) return;

        try
        {
            SavePlayerInfo();
            currentGameData.lastSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SaveSlotInfo slotInfo = currentGameData.saveSlots[slotIndex];
            slotInfo.isEmpty = false;
            slotInfo.lastSaved = currentGameData.lastSaved;
            slotInfo.playerLevel = currentGameData.player.playerLevel;
            slotInfo.currentDay = currentGameData.stats.totalDayCount;
            slotInfo.totalPlayTime = currentGameData.stats.totalSurvivedTime;

            string jsonData = JsonUtility.ToJson(currentGameData, true);
            string filePath = Path.Combine(saveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);
            File.WriteAllText(filePath, jsonData);

            OnDataSaved?.Invoke();

            if (enableDebugLog)
                Debug.Log($"[DataManager] 게임 데이터가 저장되었습니다: {saveFolderPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 데이터 저장 실패: {e.Message}");
        }
    }

    public void LoadGameData(int slotIndex)
    {
        string filePath = Path.Combine(saveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);
        try
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                currentGameData = JsonUtility.FromJson<GameData>(jsonData);

                if (enableDebugLog)
                    Debug.Log("[DataManager] 게임 데이터가 로드되었습니다.");
            }
            else
            {
                if (enableDebugLog)
                    Debug.Log("[DataManager] 저장 파일이 없습니다. 새로운 게임 데이터를 생성합니다.");
                currentGameData = new GameData();
            }

            OnDataLoaded?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 데이터 로드 실패: {e.Message}");
        }
    }

    public void DeleteSaveSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots) return;
        string filePath = Path.Combine(saveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                currentGameData.saveSlots[slotIndex].isEmpty = true;
                currentGameData.saveSlots[slotIndex].lastSaved = string.Empty;
                currentGameData.saveSlots[slotIndex].playerLevel = 1;
                currentGameData.saveSlots[slotIndex].currentDay = 1;
                currentGameData.saveSlots[slotIndex].totalPlayTime = 0f;
                currentGameData.saveSlots[slotIndex].previewImagePath = string.Empty;
                if (enableDebugLog)
                    Debug.Log($"[DataManager] 저장 슬롯 {slotIndex}이(가) 삭제되었습니다.");
            }
            else
            {
                if (enableDebugLog)
                    Debug.Log($"[DataManager] 저장 슬롯 {slotIndex}이(가) 비어 있습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 저장 슬롯 삭제 실패: {e.Message}");
        }
    }

    public SaveSlotInfo GetSaveSlotInfo(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots) return null;

        string filePath = Path.Combine(saveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);

        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            else
            {
                string json = File.ReadAllText(filePath);
                GameData tempData = JsonUtility.FromJson<GameData>(json);

                if (tempData != null && tempData.saveSlots.Count > slotIndex)
                {
                    return tempData.saveSlots[slotIndex];
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read save slot info {slotIndex}: {e.Message}");
        }

        return null;
    }

    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(currentGameData.settings, true);
            string filePath = Path.Combine(saveFolderPath, SETTINGS_FILE);
            File.WriteAllText(filePath, json);
            if(enableDebugLog)
                Debug.Log("Settings saved successfully to " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }


    public void LoadSettings()
    {
        string filePath = Path.Combine(saveFolderPath, SETTINGS_FILE);

        if (!File.Exists(filePath))
        {
            // 설정 파일이 없으면 기본 설정 사용
            currentGameData.settings = new GameSettings();
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            GameSettings loadedSettings = JsonUtility.FromJson<GameSettings>(json);

            if (loadedSettings != null)
            {
                currentGameData.settings = loadedSettings;
                if (enableDebugLog)
                    Debug.Log("Settings loaded successfully");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
        }
    }

    private void LoadGlobalStats()
    {
        string filePath = Path.Combine(saveFolderPath, GLOBAL_STATS_FILE);
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                globalStats = JsonUtility.FromJson<GlobalGameStats>(json);
            }
            else
            {
                globalStats = new GlobalGameStats();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 글로벌 통계 로드 실패: {e.Message}");
        }
    }

    public void SaveGlobalStats()
    {
        if (globalStats == null) return;
        try
        {
            string json = JsonUtility.ToJson(globalStats, true);
            string filePath = Path.Combine(saveFolderPath, GLOBAL_STATS_FILE);
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 글로벌 통계 저장 실패: {e.Message}");
        }
    }

    public void UpdateGlobalStats()
    {
        globalStats.totalPlayTime += gameDataforGlobal.stats.totalSurvivedTime;
        globalStats.totalDeaths += gameDataforGlobal.stats.totalDeaths;
        globalStats.totalMonsterKills += gameDataforGlobal.stats.totalMonsterKills;
        globalStats.totalFoodConsumed += gameDataforGlobal.stats.totalSurvivedTime;
        globalStats.totalWaterConsumed += gameDataforGlobal.stats.totalSurvivedTime;
        globalStats.totalItemsCrafted += gameDataforGlobal.stats.totalItemsCrafted;
        globalStats.totalDistanceTraveled += gameDataforGlobal.stats.totalDistanceTraveled;
        SaveGlobalStats();
    }

    public void SavePlayerInfo()
    {
        if (currentGameData == null || currentGameData.player == null) return;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;
        currentGameData.player.transformX = playerObject.transform.position.x;
        currentGameData.player.transformY = playerObject.transform.position.y;
        currentGameData.player.transformZ = playerObject.transform.position.z;
    }
}
