using UnityEngine;

public class StunEffect : IMeleeEffect
{
    public void OnHit(GameObject gameObject)
    {
        Debug.Log("스턴" + gameObject.name);
    }
}
