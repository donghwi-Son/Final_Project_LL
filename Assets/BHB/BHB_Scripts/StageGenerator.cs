using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    public int rows = 16;
    public int cols = 9;

    public Vector2 roomSize = new Vector2(28.8456f, 16.1808f);
    public Vector3 origin = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 bottomLeft = origin + new Vector3(col * roomSize.x, row * roomSize.y, 0);
                Vector3 bottomRight = bottomLeft + new Vector3(roomSize.x, 0, 0);
                Vector3 topLeft = bottomLeft + new Vector3(0, roomSize.y, 0);
                Vector3 topRight = bottomLeft + new Vector3(roomSize.x, roomSize.y, 0);

                Gizmos.DrawLine(bottomLeft, bottomRight); // 아래
                Gizmos.DrawLine(bottomRight, topRight);   // 오른쪽
                Gizmos.DrawLine(topRight, topLeft);       // 위
                Gizmos.DrawLine(topLeft, bottomLeft);     // 왼쪽
            }
        }
    }
}
