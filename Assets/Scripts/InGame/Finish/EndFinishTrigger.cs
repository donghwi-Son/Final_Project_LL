using UnityEngine;
using UnityEngine.SceneManagement;

// 보스가 죽었을 때 로비 씬으로 돌아가는 기능
public class EndFinishTrigger : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject endFinishObject;
    [SerializeField] private string lobbySceneName = "Lobby";
    [SerializeField] private string playerTag = "Player";
    private bool activated = false;
    private Collider2D cd;

    private void Awake()
    {
        cd = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (endFinishObject != null)
        {
            endFinishObject.SetActive(false);
            cd.enabled = false;
        }
    }

    void Update()
    {
        if (activated) return;

        if (boss == null)
        {
            endFinishObject.SetActive(true);
            cd.enabled = true;
            activated = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        SceneManager.LoadScene(lobbySceneName);
    }
}