using System.Collections;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Transform startpos;
    public Transform endpos;
    public Transform pos;
    public float speed;

    void Start()
    {
        transform.position = startpos.position;
        pos = endpos;
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, pos.position, Time.deltaTime * speed);

        if(Vector2.Distance(transform.position, pos.position) <= 0.05f)
        {
            if(pos == endpos)
            {
                pos = startpos;
            }
            else
            {
                pos = endpos;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(SetParentNull(collision.transform));
        }
    }

    IEnumerator SetParentNull(Transform playerTransform)
    {
        yield return new WaitForEndOfFrame();
        if (playerTransform != null)
        {
            playerTransform.SetParent(null);
        }
    }
}
