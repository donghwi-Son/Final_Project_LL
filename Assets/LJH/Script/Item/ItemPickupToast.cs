using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemPickupToast : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform toastCanvasRoot;
    [SerializeField] private GameObject toastPrefab;

    [Header("Timing")]
    [SerializeField] private float showTime = 2.2f;
    [SerializeField] private float fadeTime = 0.25f;

    private readonly Queue<int> queue = new();
    private bool isShowing;

    void Awake()
    {
        if (toastCanvasRoot == null) toastCanvasRoot = transform;
    }

    public void Enqueue(int itemIndex)
    {
        queue.Enqueue(itemIndex);
        if (!isShowing) StartCoroutine(RunQueue());
    }

    public void Enqueue(ItemDefinition def)
    {
        if (def == null) return;
        StartCoroutine(ShowOnce(def));
    }

    private IEnumerator RunQueue()
    {
        isShowing = true;

        while (queue.Count > 0)
        {
            int idx = queue.Dequeue();
            var def = ItemDatabase.Instance.GetDefinition(idx);
            if (def != null) yield return ShowOnce(def);
            else Debug.LogWarning($"[ItemPickupToast] ItemDefinition 없음 (index={idx})");
        }

        isShowing = false;
    }

    private IEnumerator ShowOnce(ItemDefinition def)
    {
        var go = Instantiate(toastPrefab, toastCanvasRoot);
        var entry = go.GetComponent<ItemPickupToastEntry>();
        if (entry == null)
        {
            Debug.LogError("[ItemPickupToast] toastPrefab에 ItemPickupToastEntry가 없습니다.");
            Destroy(go);
            yield break;
        }

        entry.Setup(def.icon, def.name, def.description);

        yield return entry.Fade(0f, 1f, fadeTime);
        yield return new WaitForSeconds(showTime);
        yield return entry.Fade(1f, 0f, fadeTime);

        Destroy(go);
    }
}