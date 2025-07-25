using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBase : Entity
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {

    }

    protected override void Update()
    {
        base.Update();
    }

    public void BossRotationZero()
    {
        anim.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
