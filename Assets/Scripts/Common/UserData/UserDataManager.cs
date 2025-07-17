using Singleton.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserDataManager : SingletonComponent<UserDataManager>
{
    // 저장된 데이터가 존재하는지 확인하는 프로퍼티
    public bool ExistsSavedData { get; private set; }
    // 모든 사용자 데이터 인스턴스를 관리하는 리스트

    public List<IUserData> UserDataList { get; private set; } = new List<IUserData>();

    [Header("설정")]
    [SerializeField] private int maxSaveSlots = 3;
    [SerializeField] private bool enableDebugLog = true;

    public GameData currentGameData;
    private GameData gameDataforGlobal;
    public string SaveFolderPath { get; private set; }
    private const string SAVE_FOLDER = "SaveData";
    private const string SAVE_FILE_NAME = "GameSave_Slot_";
    private const string SAVE_FILE_EXTENSION = ".json";
    private const string SETTINGS_FILE = "GameSettings.json";

    // 이벤트 시스템
    //public event Action<Achievement> OnAchievementUnlocked;
    //public event Action<int> OnLevelUp;
    public event Action OnDataLoaded;
    public event Action OnDataSaved;
    //public event Action<string> OnStatUpdated;

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
        SetupPaths();
        InitializeData();
        UserDataList.Add(new UserSettingsData());
        UserDataList.Add(new UserPlayData());
        return true;
    }

    protected override void ReleaseInstance()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (Instance != this)
            Destroy(gameObject);
    }
    #endregion

    private void SetupPaths()
    {
        SaveFolderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);

        if (!Directory.Exists(SaveFolderPath))
            Directory.CreateDirectory(SaveFolderPath);
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
            //SavePlayerInfo();
            currentGameData.lastSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SaveSlotInfo slotInfo = currentGameData.saveSlots[slotIndex];
            slotInfo.isEmpty = false;
            slotInfo.lastSaved = currentGameData.lastSaved;

            string jsonData = JsonUtility.ToJson(currentGameData, true);
            string filePath = Path.Combine(SaveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);
            File.WriteAllText(filePath, jsonData);

            OnDataSaved?.Invoke();

            if (enableDebugLog)
                Debug.Log($"[DataManager] 게임 데이터가 저장되었습니다: {SaveFolderPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 데이터 저장 실패: {e.Message}");
        }
    }

    public void LoadGameData(int slotIndex)
    {
        string filePath = Path.Combine(SaveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);
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
        string filePath = Path.Combine(SaveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);
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

        string filePath = Path.Combine(SaveFolderPath, SAVE_FILE_NAME + slotIndex + SAVE_FILE_EXTENSION);

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

    // 모든 사용자 데이터를 기본값으로 설정하는 메서드
    public void SetDefaultUserData()
    {
        // 사용자 데이터 리스트 개수만큼 반복
        for (int i = 0; i < UserDataList.Count; i++)
        {
            // 각 사용자 데이터의 기본값 설정
            UserDataList[i].SetDefaultData();
        }
    }

    // 저장된 사용자 데이터를 불러오는 메서드
    public void LoadUserData()
    {
        // PlayerPrefs에서 저장된 데이터 존재 여부 확인
        ExistsSavedData = PlayerPrefs.GetInt("ExistsSavedData") == 1 ? true : false;

        // 저장된 데이터가 존재하는 경우
        if (ExistsSavedData)
        {
            // 사용자 데이터 리스트 개수만큼 반복
            for (int i = 0; i < UserDataList.Count; i++)
            {
                // 각 사용자 데이터 불러오기
                UserDataList[i].LoadData();
            }
        }
    }

    // 사용자 데이터를 저장하는 메서드
    public void SaveUserData()
    {
        // 저장 오류 발생 여부를 확인하는 변수
        bool hasSaveError = false;

        // 사용자 데이터 리스트 개수만큼 반복
        for (int i = 0; i < UserDataList.Count; i++)
        {
            // 각 사용자 데이터 저장 및 성공 여부 확인
            bool isSaveSuccess = UserDataList[i].SaveData();
            // 저장에 실패한 경우
            if (!isSaveSuccess)
            {
                // 오류 플래그 설정
                hasSaveError = true;
            }
        }

        // 저장 오류가 없는 경우
        if (!hasSaveError)
        {
            // 저장된 데이터 존재 플래그 설정
            ExistsSavedData = true;
            // PlayerPrefs에 저장된 데이터 존재 여부 저장
            PlayerPrefs.SetInt("ExistsSavedData", 1);
            // PlayerPrefs 저장 실행
            PlayerPrefs.Save();
        }
    }

    // 제네릭을 사용하여 특정 타입의 사용자 데이터를 가져오는 메서드
    public T GetUserData<T>() where T : class, IUserData
    {
        // LINQ를 사용하여 해당 타입의 첫 번째 데이터 반환
        return UserDataList.OfType<T>().FirstOrDefault();
    }
}
