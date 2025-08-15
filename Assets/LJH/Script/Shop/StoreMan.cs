using System;
using UnityEngine;

public class StoreMan : MonoBehaviour
{
    public static StoreMan Instance { get; private set; }
    public bool PlayerInside = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerInside = false;
    }
}
