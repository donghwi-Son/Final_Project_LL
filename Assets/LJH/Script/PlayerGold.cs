using System;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public static PlayerGold Instance { get; private set; }
    
    public AudioSource source;
    public AudioClip clip;
    
    public int gold = 1000;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Box.OnBoxOpened += HandleBoxOpened;
    }
    
    private void OnDisable()
    {
        Box.OnBoxOpened -= HandleBoxOpened;
    }

    void HandleBoxOpened(Box box, int reward)
    {
        AddGold(reward);
    }

    public void AddGold(int amount)
    {
        source.PlayOneShot(clip);
        gold += amount;
        Debug.Log("보유 골드 :"+gold);
    }
}
