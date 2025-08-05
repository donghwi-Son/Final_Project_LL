using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NpcGroundChecker : MonoBehaviour
{
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            rb.gravityScale = 0;
            // 바닥에 붙도록 수직 속도 제거
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
        else
        {
            rb.gravityScale = 1;
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
