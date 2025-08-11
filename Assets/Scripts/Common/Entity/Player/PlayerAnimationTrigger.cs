using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private PlayerController player => GetComponentInParent<PlayerController>();

    private void AnimationTrigger()
    {
        player.AnimationFinishTrigger();
    }

    private void AttackTrigger(float _rangeMod)
    {
        player.AttackManager.MeleeAttack(_rangeMod);
    }
}
