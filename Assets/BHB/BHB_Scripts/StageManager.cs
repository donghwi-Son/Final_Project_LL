using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스테이지 관리 매니저
// 나중에 GameManager와 합칠 수 있도록 조정 중...
public class StageManager : MonoBehaviour
{
    public GameObject[] Stages;    // 타일맵 GameObject들을 배열로 등록
    public int stageIndex = 0;     // 현재 스테이지 인덱스
    public Transform player;       // 플레이어 위치 초기화를 위한 참조

    public Button[] stageselect; // 스테이지 선택 버튼
    public int[] stageOptionIndices = new int[2]; // 버튼 리스트 중 2개만 선택지로 나오게 지정

    public GameObject stageChoicePanel; // 버튼을 감싸는 패널
    public PlayerController PlayerController; // 플레이어 일시 정지를 위한 컨트롤러

    public int stageCounter; // 스테이지 선택마다 +
    private int bossstageCounter = 5; // 보스 스테이지 카운터

    private void Start()
    {
        stageChoicePanel.SetActive(false);  // 첫 시작 시에는 ui 숨기기
    }

    // 4개의 스테이지 중 2개의 선택지를 랜덤으로 제시하는 메서드
    // 플레이어가 Finish 태그에 닿으면 작동
    public void OnFinish()
    {
        PlayerController.enabled = false; // 플레이어 조작 비활성화

        // 버튼 비활성화 및 중복 이벤트 방지를 위해 버튼 이벤트 초기화(RemoveAllListeners())
        foreach (var btn in stageselect)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.RemoveAllListeners();
        }

        // stageCounter가 6이 되면, 보스 스테이지 입장 시점이기에 클리어하면 다시 Start로 돌아오도록 하는 기능
        if (stageCounter == 6)
        {
            Button startBtn = stageselect[0];
            startBtn.gameObject.SetActive(true);
            startBtn.onClick.AddListener(() => NextStage(0));
            stageChoicePanel.SetActive(true);
            return;
        }

        // stageCounter가 5가 되면, 보스 스테이지 입장 버튼만 나타나도록 하는 기능
        if (stageCounter == 5)
        {
            Button bossBtn = stageselect[5];
            bossBtn.gameObject.SetActive(true);
            bossBtn.onClick.AddListener(() => NextStage(bossstageCounter));

            // 보완 부분: Boss 버튼이 나오면 다른 버튼이 안 나오게
            stageChoicePanel.SetActive(true);
            return;
        }

        // 일반, 어려운 적, 상점, 이벤트 스테이지 선택지 기능
        // 아래 리스트를 통해 1번부터 4번까지 등록한 스테이지 중 랜덤으로 2개의 선택지를 플레이어에게 제시
        // 고른 선택지는 버튼 인덱스 = Stage 번호로 하여 스테이지 번호를 버튼과 연결해서 클릭 시 해당 스테이지로 이동
        List<int> candidates = new List<int> { 1, 2, 3, 4 }; // Stage 번호(타일맵의 인덱스이자 버튼 인덱스임을 혼동하지 말 것)
        Shuffle(candidates);

        for (int i = 0; i < 2; i++)
        {
            int targetStage = candidates[i];
            int buttonIndex = targetStage; // 버튼 인덱스 = Stage 번호

            Button btn = stageselect[buttonIndex];
            btn.gameObject.SetActive(true);
            btn.onClick.AddListener(() => NextStage(targetStage));
        }

        stageChoicePanel.SetActive(true);
    }

    // 리스트 섞기 메서드
    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
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
        PlayerController.enabled = true;
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