using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private Animation bossAnim;
    [SerializeField] private GameObject hpObject;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject boss;
    [SerializeField] private TextMeshProUGUI bossNameText;

    public void Show()
    {
        hpObject.SetActive(false);

        boss = StageManager.Instance.BossObj;

        boss.GetComponent<CharacterStats>().OnHealthChanged += UpdateHealthUI;

        bossNameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("DialogTable", StageManager.Instance.BossKey, LocalizationSettings.SelectedLocale);

        StartCoroutine(BossCo()); //보스 애니메이션 시작 후 체력 UI 활성화
    }

    private void Update()
    {
        if (boss == null) return;
        UpdateHealthUI(); //체력 UI 업데이트
    }

    private IEnumerator BossCo()
    {
        bossAnim.Play();

        yield return new WaitForSeconds(bossAnim.clip.length);

        hpObject.SetActive(true); //보스 애니메이션이 끝난 후 체력 UI 활성화
    }

    private void UpdateHealthUI()
    {
        int currenthp = boss.GetComponent<CharacterStats>().currentHealth;
        int MaxHP = boss.GetComponent<CharacterStats>().maxHealth.GetValue();

        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currenthp / MaxHP;
        }

        if(currenthp <= 0)
        {
            gameObject.SetActive(false); //보스가 죽으면 체력 UI 비활성화
        }
    }

    private void OnDisable()
    {
        boss.GetComponent<CharacterStats>().OnHealthChanged -= UpdateHealthUI; //체력 변경 시 UI 업데이트 해제
    }
}
