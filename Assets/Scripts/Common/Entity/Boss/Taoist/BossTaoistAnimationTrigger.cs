using UnityEngine;

public class BossTaoistAnimationTrigger : MonoBehaviour
{

    private BossTaoist boss => GetComponentInParent<BossTaoist>();

    private void AnimTrigger()
    {
        boss.AnimationTrigger();
    }
}
