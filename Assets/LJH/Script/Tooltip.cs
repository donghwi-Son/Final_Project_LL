using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private Image      icon;
    [SerializeField] private TMP_Text   nameText;
    [SerializeField] private TMP_Text   descText;
    private RectTransform rect;

    void Awake()
    {
        Instance = this;
        rect     = (RectTransform)transform;
        Hide();
    }
    
    public void Show(Sprite spr, string n, string d, Vector2 screenPos)
    {
        icon.sprite    = spr;
        nameText.text  = n;
        descText.text  = d;

        float halfWidth = Screen.width * 0.5f;
        
        if (screenPos.x < halfWidth)
        {
            rect.pivot = new Vector2(0f, 1f);
            rect.position = screenPos + new Vector2(10f, -10f);
        }
        else
        {
            rect.pivot = new Vector2(1f, 1f);
            rect.position = screenPos + new Vector2(-10f, -10f);
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}