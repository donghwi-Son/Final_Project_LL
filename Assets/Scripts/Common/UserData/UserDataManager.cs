using Singleton.Component;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : SingletonComponent<UserDataManager>
{
    // 저장된 데이터가 존재하는지 확인하는 프로퍼티
    public bool ExistsSavedData { get; private set; }
    // 모든 사용자 데이터 인스턴스를 관리하는 리스트

    public List<IUserData> UserDataList { get; private set; } = new List<IUserData>();

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {     
        UserDataList.Add(new UserSettingsData());
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
