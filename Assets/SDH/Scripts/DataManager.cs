using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GlobalGameRecord
{
    public float totalPlayTime;
    public int totalDeaths;
    public int totalMonsterKills;
    public int totalUsedMoney;
    //public List<Artifact> artifacts = new List<Artifact>();
}
[Serializable]
public class SingleGameRecord
{
    public float playTime;
    public int monsterKills;
    public int usedMoney;
}

[Serializable]
public class PlayerInfo
{
    public int RoomID;
    public AttackMode attackMode;
    //public List<ItemData> items = new();
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
    public string previewImagePath;
}

[Serializable]
public class GameData
{
    public SingleGameRecord singleGameRecord = new();
    public PlayerInfo player = new();
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

    public GameData currentGameData;
    private GameData gameDataforGlobal;
    private GlobalGameRecord globalGameRecord;
    private string saveFolderPath;
    private const string SAVE_FOLDER = "SaveData";
    private const string SAVE_FILE_NAME = "GameSave_Slot_";
    private const string SAVE_FILE_EXTENSION = ".json";
    private const string SETTINGS_FILE = "GameSettings.json";
    private const string GLOBAL_STATS_FILE = "GlobalGameRecord.json";

    // 이벤트 시스템
    //public event Action<Achievement> OnAchievementUnlocked;
    //public event Action<int> OnLevelUp;
    public event Action OnDataLoaded;
    public event Action OnDataSaved;
    //public event Action<string> OnStatUpdated;

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
            if (enableDebugLog)
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
                globalGameRecord = JsonUtility.FromJson<GlobalGameRecord>(json);
            }
            else
            {
                globalGameRecord = new GlobalGameRecord();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 글로벌 통계 로드 실패: {e.Message}");
        }
    }

    public void SaveGlobalStats()
    {
        if (globalGameRecord == null) return;
        try
        {
            string json = JsonUtility.ToJson(globalGameRecord, true);
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
        SaveGlobalStats();
    }

    public void SavePlayerInfo()
    {
        if (currentGameData == null || currentGameData.player == null) return;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;
    }
}
