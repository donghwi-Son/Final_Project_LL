using UnityEngine;

public class RotateZone : MonoBehaviour
{
    [SerializeField] private Transform target;        
    [SerializeField] private float zDegPerSec = 1f; 

    void Awake()
    {
        if (target == null) target = transform;
    }

    void Update()
    {
        float delta = zDegPerSec * Time.deltaTime;
        
        var angles = target.eulerAngles;
        angles.z = Mathf.Repeat(angles.z + delta, 360f);
        target.eulerAngles = angles;
    }
}