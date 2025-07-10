using UnityEngine;

public class PlayerAttackState : PlayerState
{
    PlayerController player => psm.player;

    bool isHolding = false;
    float holdTime;
    float requiredHoldTime = 1f;
    bool isHoldAttack = false;
    PlayerChargeBar chargeBar;


    public PlayerAttackState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.rb.linearVelocityX = 0f;
        player.CanFlip = false;
        isHolding = true;
        holdTime = 0f;
        isHoldAttack = false;
        chargeBar = player.GetComponent<PlayerChargeBar>();
        chargeBar?.ShowChargeBar();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (Input.GetMouseButton(0) && isHolding)
        {
            holdTime += Time.deltaTime;
            chargeBar?.UpdateChargeBar(holdTime / requiredHoldTime);
            if (holdTime >= requiredHoldTime)
            {
                isHoldAttack = true;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            chargeBar?.HideChargeBar();
            if (isHoldAttack)
            {
                player.AttackManager.ChargeAttack(player.attackMode, player.IsFacingRight);
            }
            else
            {
                player.AttackManager.Attack(player.attackMode, player.IsFacingRight);
            }
            isHoldAttack = false;
            holdTime = 0f;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        player.CanFlip = true;
    }

}
