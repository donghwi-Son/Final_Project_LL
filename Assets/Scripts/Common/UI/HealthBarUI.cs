using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Entity entity; //Entity 컴포넌트 참조
    private CharacterStats myStats; //CharacterStats 컴포넌트 참조
    private RectTransform myTransform; //RectTransform 컴포넌트 참조
    private Slider slider; //Slider 컴포넌트 참조

    private void Start()
    {
        myTransform = GetComponent<RectTransform>(); //RectTransform 컴포넌트 가져오기
        entity = GetComponentInParent<Entity>(); //부모 오브젝트에서 Entity 컴포넌트 가져오기
        myStats = GetComponentInParent<CharacterStats>(); //부모 오브젝트에서 CharacterStats 컴포넌트 가져오기
        slider = GetComponent<Slider>(); //Slider 컴포넌트 가져오기

        entity.OnFliped += FlipUI; //Entity의 onFliped 델리게이트에 FlipUI 메서드 등록
        myStats.OnHealthChanged += UpdateHealthUI; //체력 변경 시 UI 업데이트

        Debug.Log("체력바 부르고 있다.");
    }

    private void Update()
    {
        UpdateHealthUI(); //체력 UI 업데이트
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.maxHealth.GetValue(); //최대 체력 설정
        slider.value = myStats.currentHealth; //현재 체력 설정
    }

    //UI를 180도 회전시켜서 반대 방향으로 표시
    public void FlipUI() => myTransform.Rotate(0, 180, 0);

    private void OnDisable()
    {
        entity.OnFliped -= FlipUI; //Entity의 onFliped 델리게이트에서 FlipUI 메서드 등록 해제
        myStats.OnHealthChanged -= UpdateHealthUI; //체력 변경 시 UI 업데이트 해제
    }
}
