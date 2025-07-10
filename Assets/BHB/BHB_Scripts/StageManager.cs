using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스테이지-버튼을 인덱스 번호가 아닌 각기 연결
// 이전까지는 스테이지-버튼이 동일해야 했지만, 이를 통해 인덱스 번호만 맞추도록 수정
[System.Serializable]
public class StageButtonBinding
{
    public Button button; // 스테이지 이동 버튼 연결
    public int stageIndex; // 버튼이 이동시킬 스테이지 인덱스 (Stages 배열의 인덱스)
    public StageType stagetype; // 스테이지 별 타입 구분(시작/일반/어려움/상점/이벤트/보스)
}

// 스테이지 타입 관리 영역
public enum StageType
{
    Start,
    Normal,
    Hard,
    Store,
    Event,
    Boss
}

// 스테이지 관리 매니저
// 나중에 GameManager와 합칠 수 있도록 조정 중...
public class StageManager : MonoBehaviour
{
    [Header("플레이어")]
    public Transform player;       // 플레이어 위치 초기화를 위한 참조
    public PlayerController playerController; // 플레이어 일시 정지를 위한 컨트롤러

    [Header("캔버스")]
    public GameObject stageChoicePanel; // 버튼을 감싸는 패널
    public GameObject stopPanel; // 일시 정지 시 나타나는 반투명 패널

    [Header("카운트")]
    public int stageIndex = 0;     // 현재 스테이지 인덱스
    public int stageCounter; // 스테이지 선택마다 +
    //private int bossstageCounter = 5; // 보스 스테이지 카운터

    [Header("버튼 위치")]
    // 각 버튼을 고정된 위치가 아니라, 좌우에서 나오도록
    // 차후 이미지와 패널을 넣어 좀 더 수정할 계획
    [SerializeField] private Vector2 leftButtonPosition;
    [SerializeField] private Vector2 rightButtonPosition;
    [SerializeField] private Vector2 centerButtonPosition;

    [Header("스테이지 인덱스와 버튼")]
    public GameObject[] Stages;    // 타일맵 GameObject들을 배열로 등록
    public List<StageButtonBinding> stageButtons; // 일반 선택지 랜덤 2개 선택 // 시작 및 보스 스테이지도 이곳에 통합

    private void Start()
    {
        // 첫 시작 시에는 ui 숨기기
        stageChoicePanel.SetActive(false);  
        stopPanel.SetActive(false);
    }

    // 4개의 스테이지 중 2개의 선택지를 랜덤으로 제시하는 메서드
    // 플레이어가 Finish 태그에 닿으면 작동
    public void OnFinish()
    {
        playerController.enabled = false; // 플레이어 조작 비활성화

        // 모든 버튼 숨기고 리스너 초기화
        foreach (var binding in stageButtons)
        {
            binding.button.gameObject.SetActive(false);
            binding.button.onClick.RemoveAllListeners();
        }

        // stageCounter는 이제 단순 진행용이므로 그대로 유지

        // stageCounter가 6이 되면, 보스 스테이지 입장 시점이기에 클리어하면 다시 Start로 돌아오도록 하는 기능
        if (stageCounter == 6)
        {
            var startEntry = stageButtons.Find(b => b.stagetype == StageType.Start);
            if(startEntry != null)
            {
                ShowButton(startEntry, centerButtonPosition);
                return;
            }
        }

        // stageCounter가 5가 되면, 보스 스테이지 입장 버튼만 나타나도록 하는 기능
        if (stageCounter == 5)
        {
           var bossEntry = stageButtons.Find(b => b.stagetype == StageType.Boss);
           if(bossEntry != null)
           {
                ShowButton(bossEntry, centerButtonPosition);
                return;
           }
        }

        // 일반, 어려운 적, 상점, 이벤트 스테이지 선택지 기능
        // 아래 리스트를 통해 1번부터 4번까지 등록한 스테이지 중 랜덤으로 2개의 선택지를 플레이어에게 제시
        // 타입별로 연결
        var candidates = stageButtons.FindAll(b => b.stagetype == StageType.Normal || b.stagetype == StageType.Hard || b.stagetype == StageType.Store || b.stagetype == StageType.Event);

        Shuffle(candidates);

        for (int i = 0; i < 2 && i < candidates.Count; i++)
        {
            var entry = candidates[i];
            var pos = (i == 0) ? leftButtonPosition : rightButtonPosition;
            ShowButton(entry, pos);
        }


        stageChoicePanel.SetActive(true);
        stopPanel.SetActive(true);
    }

    // 리스트 섞기 메서드
    void Shuffle(List<StageButtonBinding> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    // 버튼 활성화 메서드
    private void ShowButton(StageButtonBinding entry, Vector2 position)
    {
        entry.button.gameObject.SetActive(true);
        RectTransform rect = entry.button.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        entry.button.onClick.AddListener(() => NextStage(entry.stageIndex));
        stageChoicePanel.SetActive(true);
        stopPanel.SetActive(true);
    }

    // 다음 스테이지 이동 기능
    public void NextStage(int newStageIndex)
    {
        // 현재 스테이지 비활성화
        if (stageIndex >= 0 && stageIndex < Stages.Length)
            Stages[stageIndex].SetActive(false);

        // 새 스테이지 활성화
        stageIndex = newStageIndex;

        if (stageIndex >= 0 && stageIndex < Stages.Length)
            Stages[stageIndex].SetActive(true);

        // ui 버튼이 사라지고 조작을 다시 활성화 및 플레이어 위치 초기화
        stageChoicePanel.SetActive(false);
        stopPanel.SetActive(false);
        playerController.ResetSpeed(); // 기존 enabled = true를 playerController로 이동
        PlayerReposition();

        // 선택지를 선택하고 넘어갈 때마다 카운트 증가, 6(=보스 클리어) 이후에는 다시 0으로 해서 0~6을 반복
        stageCounter++;

        if (stageCounter > 6)
        {
            stageCounter = 0;
        }
    }

    // 플레이어 위치를 스테이지 클리어 후 다음 스테이지 시작마다 초기화 기능
    void PlayerReposition()
    {
        // 지정한 좌표로 플레이어가 이동
        player.position = new Vector3(-24.2f, 30.45f, 0);
    }

}