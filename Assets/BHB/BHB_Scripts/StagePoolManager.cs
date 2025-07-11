using System.Collections.Generic;
using UnityEngine;

public class StagePoolManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static StagePoolManager Instance { get; private set; }

    // 오브젝트 풀 항목
    [System.Serializable]
    public class StagePoolEntry
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<StagePoolEntry> poolEntries; // 풀 설정 리스트

    private Dictionary<string, Queue<GameObject>> poolDict;  // 실제 풀링 오브젝트 Queue
    private Dictionary<string, GameObject> prefabLookup; // 태그별 원본 프리팹

    // 초기화 시 싱글톤 설정 및 풀 생성 영역
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializePools();
    }

    // 풀 딕셔너리 및 프리팹 정보 초기화 영역
    void InitializePools()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();
        prefabLookup = new Dictionary<string, GameObject>();

        foreach (var entry in poolEntries)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < entry.size; i++)
            {
                GameObject obj = Instantiate(entry.prefab, transform); // 부모를 StagePoolManager로 설정해서 StagePoolManager 아래로 생성되게
                obj.SetActive(false); // 비활성화 상태로 초기화
                objectPool.Enqueue(obj);
            }

            poolDict[entry.tag] = objectPool;
            prefabLookup[entry.tag] = entry.prefab;
        }
    }

    // 오브젝트 풀에서 꺼내기
    public GameObject GetFromPool(string tag)
    {
        if (!poolDict.ContainsKey(tag))
        {
            return null;
        }

        if (poolDict[tag].Count > 0)
        {
            GameObject obj = poolDict[tag].Dequeue();
            obj.SetActive(true); // 활성화 후 반환
            return obj;
        }

        return null;
    }

    // 오브젝트 풀로 반환
    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false); // 비활성화
        obj.transform.SetParent(this.transform); // 다시 부모인 StagePoolManager로 복귀
        if (!poolDict.ContainsKey(tag))
        {
            return;
        }
        poolDict[tag].Enqueue(obj);
    }
}
