using UnityEngine;

public class NormalBox : MonoBehaviour, BoxAni
{
    [SerializeField] private Sprite openedSprite;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetOpened()
    {
        if (sr != null && openedSprite != null)
            sr.sprite = openedSprite;
    }
}
