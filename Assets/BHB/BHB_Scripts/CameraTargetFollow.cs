using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0);

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity;
        }
    }
}
