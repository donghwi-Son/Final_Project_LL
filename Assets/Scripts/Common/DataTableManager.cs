using System.Collections.Generic;
using System;
using UnityEngine;
using Singleton.Component;
using System.Linq;

public class DataTableManager : SingletonComponent<DataTableManager>
{
    private const string DATA_PATH = "DataTable";

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
