using System;
using UnityEngine;

public class StoreMan : MonoBehaviour
{
    private static  int PlayerInsideCount = 0;
    public static bool PlayerInside => PlayerInsideCount > 0;
    private int localCount = 0;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        localCount++;
        if (localCount == 1)
            PlayerInsideCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (localCount > 0)
        {
            localCount--;
            if (localCount == 0)
                PlayerInsideCount = Mathf.Max(0, PlayerInsideCount - 1);
        }
    }
}
