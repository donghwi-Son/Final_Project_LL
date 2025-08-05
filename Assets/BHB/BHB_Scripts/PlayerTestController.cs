using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerTestController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;
    private bool isGrounded = false;

    [Header("바닥 체크")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

        if (xInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(xInput), 1, 1);
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector2Int currentGrid = StageManager.Instance.GetCurrentGrid();
            StageData currentRoom = StageManager.Instance.GetRoomAt(currentGrid);
            if (currentRoom?.instance != null)
            {
                var condition = currentRoom.instance.GetComponentInChildren<IRoomCondition>();
                if (condition != null)
                {
                    Debug.Log("[Player] P 키 상호작용 시도");
                    condition.OnPlayerInteract(); // 핵심 인터랙션 호출
                }
                else
                {
                    Debug.LogWarning("[Player] 현재 방에 IRoomCondition이 존재하지 않음");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
