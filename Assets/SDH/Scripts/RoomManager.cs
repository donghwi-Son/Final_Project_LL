using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<GameObject> normalRooms = new List<GameObject>();
    public List<GameObject> hardRooms = new List<GameObject>();
    public GameObject bossRoom;


}
