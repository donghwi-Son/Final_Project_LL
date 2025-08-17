using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SynergyEvent : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nametext;
    [SerializeField] private TextMeshProUGUI desctext;
    [SerializeField] private CanvasGroup cg;

    [Header("조절")]
    [SerializeField] private float startDelay = 2f; 
    [SerializeField] private float showTime = 2.2f;
    [SerializeField] private float fadeTime = 0.25f;
    
    private Sprite iconArmor;
    private Sprite iconGoblinMask;
    private Sprite iconWindBelt;
    private Sprite iconPolearm;
    private Sprite iconFlameAmulet;

    private Coroutine showCo;

    void Awake()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 0f;
        
        iconArmor       = Resources.Load<Sprite>("ItemIcons/Armor");
        iconGoblinMask  = Resources.Load<Sprite>("ItemIcons/GoblinMask");
        iconWindBelt    = Resources.Load<Sprite>("ItemIcons/WindBelt");
        iconPolearm     = Resources.Load<Sprite>("ItemIcons/Polearm");
        iconFlameAmulet = Resources.Load<Sprite>("ItemIcons/FlameAmulet");
    }

    void OnEnable()
    {
        SynergyManager.OnSynergyTagActivated += HandleActivated;
    }

    void OnDisable()
    {
        SynergyManager.OnSynergyTagActivated -= HandleActivated;
    }

    private void HandleActivated(string tag)
    {
        switch (tag)
        {
            case "CombatSet":
                Set(iconArmor,     "전투장비",     "체력 +10, 올스탯 +1");
                break;

            case "GoblinSet":
                Set(iconGoblinMask,"도깨비의 친구","공격력 +20");
                
                break;

            case "WindSet":
                Set(iconWindBelt,  "바람의 가호",  "이동속도 +5");
                break;

            case "WeaponMaster":
                Set(iconPolearm,   "웨폰 마스터", "공격력 +10");
                break;

            case "Amulet":
                Set(iconFlameAmulet,"엘리멘탈 마스터","공격 효과가 번개 속성으로 변합니다.");
                break;

            default:
                Set(null, $"{tag} 시너지 발동", "효과가 적용되었습니다.");
                Debug.LogWarning($"[SynergyEvent] 매핑되지 않은 태그: {tag}");
                break;
        }
        
        if (showCo != null) StopCoroutine(showCo);
        showCo = StartCoroutine(ShowRoutine());
    }

    private void Set(Sprite sp, string title, string desc)
    {
        if (icon)      icon.sprite = sp;
        if (nametext)  nametext.text = title;
        if (desctext)  desctext.text = desc;
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        if (cg == null)
        {
            yield return new WaitForSeconds(showTime);
            yield break;
        }
        
        yield return Fade(0f, 1f, fadeTime);
        
        yield return new WaitForSeconds(showTime);
        
        yield return Fade(1f, 0f, fadeTime);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
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
