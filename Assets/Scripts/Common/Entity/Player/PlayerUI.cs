using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hp;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image DashCool;
    [SerializeField] private TextMeshProUGUI goldText;

    private PlayerController player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        UpdateCooldownUI();
        UpdateHP();
        UpdateGold();
    }

    private void UpdateCooldownUI()
    {
        if (player == null) return;

        float timeSinceLastDash = Time.time - player.lastDashTime;
        float cooldownProgress = timeSinceLastDash / player.DashCooldown;

        // 쿨다운 이미지 업데이트
        if (DashCool != null)
        {
            if (player.CanUseDash)
            {
                DashCool.fillAmount = 0f; // 사용 가능할 때는 투명
            }
            else
            {
                DashCool.fillAmount = 1f - cooldownProgress;
            }
        }
    }

    private void UpdateHP()
    {
        if (player == null) return;
        int currenthp = player.Stats.currentHealth;
        int MaxHP = player.Stats.maxHealth.GetValue();
        hp.text = $"HP : {currenthp}/{MaxHP}";
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currenthp / MaxHP;
        }
    }

    private void UpdateGold()
    {
        if (player == null) return;
        int gold = PlayerGold.Instance.gold;
        goldText.text = gold.ToString();
    }
}
