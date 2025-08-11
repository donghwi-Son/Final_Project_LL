using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject woodmon;
    public GameObject wispmon;
    public GameObject firemon;
    public GameObject boss;

    Queue<GameObject> woodmonPool = new();
    Queue<GameObject> wispmonPool = new();
    Queue<GameObject> firemonPool = new();

    private void Awake()
    {
        GenerateMonsterPool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GoToNextLevel();
        }
    }

    void GenerateMonsterPool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject woodObj = Instantiate(woodmon);
            woodObj.SetActive(false);
            woodObj.transform.SetParent(this.transform);
            woodmonPool.Enqueue(woodObj);

            GameObject wispObj = Instantiate(wispmon);
            wispObj.SetActive(false);
            wispObj.transform.SetParent(this.transform);
            wispmonPool.Enqueue(wispObj);

            GameObject fireObj = Instantiate(firemon);
            fireObj.SetActive(false);
            fireObj.transform.SetParent(this.transform);
            firemonPool.Enqueue(fireObj);
        }
    }




    void GoToNextLevel()
    {
        RoomConditionManager room = FindFirstObjectByType<RoomConditionManager>();
        room.CreateNextRoom();
    }
}
