using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform spawn;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = spawn.position;
        }
    }
}
