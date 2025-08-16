using UnityEngine;

public class BossCrowAnimationTrigger : MonoBehaviour
{

    private BossCrow boss => GetComponentInParent<BossCrow>();

    private void AnimTrigger()
    {
        boss.AnimationTrigger();
    }
}
