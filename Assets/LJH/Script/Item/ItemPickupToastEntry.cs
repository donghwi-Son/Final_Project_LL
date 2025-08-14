using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class ItemPickupToastEntry : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private CanvasGroup cg; // 투명도 제어

    [Header("Options")]
    [SerializeField] private Sprite fallbackIcon;   // 아이콘 없을 때 대체용
    [SerializeField] private int maxDescChars = 120; // 0이면 제한 없음

    void Reset()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (!iconImage) iconImage = GetComponentInChildren<Image>(true);
        var tmps = GetComponentsInChildren<TMP_Text>(true);
        if (tmps != null && tmps.Length > 0)
        {
            titleText = tmps[0];
            if (tmps.Length > 1) descText = tmps[1];
        }
    }

    void Awake()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 0f;
    }
    
    public void Setup(Sprite icon, string title, string desc)
    {
        if (iconImage) iconImage.sprite = icon ? icon : fallbackIcon;
        if (titleText) titleText.text = string.IsNullOrEmpty(title) ? "Unknown Item" : title;

        if (descText)
        {
            if (string.IsNullOrEmpty(desc))
                descText.text = "";
            else if (maxDescChars > 0 && desc.Length > maxDescChars)
                descText.text = desc.Substring(0, maxDescChars) + "...";
            else
                descText.text = desc;
        }
        
        var rt = transform as RectTransform;
        if (rt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }
    
    public IEnumerator Fade(float from, float to, float duration)
    {
        if (!cg || duration <= 0f)
        {
            if (cg) cg.alpha = to;
            yield break;
        }

        float t = 0f;
        cg.alpha = from;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}
