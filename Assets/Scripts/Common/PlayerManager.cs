using Singleton.Component;
using UnityEngine;

public class PlayerManager : SingletonComponent<PlayerManager>
{
    public PlayerController player;

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
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
}
