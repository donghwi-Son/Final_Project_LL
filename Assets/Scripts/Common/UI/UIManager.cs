using Singleton.Component;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonComponent<UIManager>
{
    [SerializeField] private Transform m_OpenedUITrs;
    [SerializeField] private Transform m_ClosedUITrs;
    [SerializeField] private Image m_Fade;
    private UIBase m_FrontUI;
    private Dictionary<System.Type, GameObject> m_OpenUIPool = new Dictionary<System.Type, GameObject>();
    private Dictionary<System.Type, GameObject> m_ClosedUIPool = new Dictionary<System.Type, GameObject>();

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
        m_Fade.transform.localScale = Vector3.zero;
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

    private UIBase GetUI<T>(out bool isAlreadyOpen)
    {
        System.Type uiType = typeof(T);

        UIBase ui = null;
        isAlreadyOpen = false;

        if (m_OpenUIPool.ContainsKey(uiType))
        {
            ui = m_OpenUIPool[uiType].GetComponent<UIBase>();
            isAlreadyOpen = true;
        }
        else if (m_ClosedUIPool.ContainsKey(uiType))
        {
            ui = m_ClosedUIPool[uiType].GetComponent<UIBase>();
            m_ClosedUIPool.Remove(uiType);
        }
        else
        {
            var uiObj = Instantiate(Resources.Load($"UI/{uiType}", typeof(GameObject))) as GameObject;
            ui = uiObj.GetComponent<UIBase>();
        }

        return ui;
    }

    public void OpenUI<T>(UIBaseData uiData)
    {
        System.Type uiType = typeof(T);

        Debug.Log($"{GetType()}::OpenUI({uiType})");

        bool isAlreadyOpen = false;
        var ui = GetUI<T>(out isAlreadyOpen);

        if (!ui)
        {
            Debug.LogError($"{uiType} does not exist.");
            return;
        }

        if (isAlreadyOpen)
        {
            Debug.LogError($"{uiType} is already open.");
            return;
        }

        ui.Init(m_OpenedUITrs);
        ui.gameObject.SetActive(true);
        ui.SetInfo(uiData);
        ui.ShowUI();

        m_FrontUI = ui;
        m_OpenUIPool[uiType] = ui.gameObject;
    }

    public void CloseUI(UIBase ui)
    {
        System.Type uiType = ui.GetType();

        Debug.Log($"CloseUI UI:{uiType}");

        ui.gameObject.SetActive(false);
        m_OpenUIPool.Remove(uiType);
        m_ClosedUIPool[uiType] = ui.gameObject;
        ui.transform.SetParent(m_ClosedUITrs);

        m_FrontUI = null;
        if(m_OpenedUITrs.childCount > 0)
        {
            var lastChild = m_OpenedUITrs.GetChild(m_OpenedUITrs.childCount - 1);
            if (lastChild)
            {
                m_FrontUI = lastChild.gameObject.GetComponent<UIBase>();
            }
        }
    }

    public T GetActiveUI<T>()
    {
        var uiType = typeof(T);
        return m_OpenUIPool.ContainsKey(uiType) ? m_OpenUIPool[uiType].GetComponent<T>() : default(T);
    }

    public bool ExistsOpenUI()
    {
        return m_FrontUI != null;
    }

    public UIBase GetCurrentFrontUI()
    {
        return m_FrontUI;
    }

    public void CloseCurrFrontUI()
    {
        m_FrontUI.CloseUI();
    }

    public void CloseAllOpenUI()
    {
        while (m_FrontUI)
        {
            m_FrontUI.CloseUI(true);
        }
    }

    public void Fade(Color color, float startAlpha, float endAlpha, float duration, float startDelay, bool deactiveOnFinish, Action onFinish = null)
    {
        StartCoroutine(FadeCo(color, startAlpha, endAlpha, duration, startDelay, deactiveOnFinish, onFinish));
    }

    private IEnumerator FadeCo(Color color, float startAlpha, float endAlpha, float duration, float startDelay, bool deactiveOnFinish, Action onFinish)
    {
        yield return new WaitForSeconds(startDelay);

        m_Fade.transform.localScale = Vector3.one;
        m_Fade.color = new Color(color.r, color.g, color.b, startAlpha);

        var startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < duration)
        {
            m_Fade.color = new Color(color.r, color.g, color.b, Mathf.Lerp(startAlpha, endAlpha, (Time.realtimeSinceStartup - startTime) / duration));
            yield return null;
        }

        m_Fade.color = new Color(color.r, color.g, color.b, endAlpha);

        if (deactiveOnFinish)
        {
            m_Fade.transform.localScale = Vector3.zero;
        }

        onFinish?.Invoke();
    }

    public void CancelFade()
    {
        m_Fade.transform.localScale = Vector3.zero;
    }
}
