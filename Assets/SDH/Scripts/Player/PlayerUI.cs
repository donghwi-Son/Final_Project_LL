using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text hp;
    public Image hpBar;
    public Image DashCool;

    private PlayerController player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

    }
    void Update()
    {
        UpdateCooldownUI();
        UpdateHP();
    }

    void UpdateCooldownUI()
    {
        if (player == null) return;

        float timeSinceLastDash = Time.time - player.lastDashTime;
        float cooldownProgress = timeSinceLastDash / player.dashCooldown;

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

    void UpdateHP()
    {
        if (player == null) return;
        int currenthp = player.stats.currentHealth;
        int MaxHP = player.stats.maxHealth.GetValue();
        hp.text = $"HP : {currenthp}/{MaxHP}";
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currenthp / MaxHP;
        }
    }

}
