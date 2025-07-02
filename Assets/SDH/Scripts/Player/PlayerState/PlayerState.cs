using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine psm;
    public PlayerState(PlayerStateMachine psm)
    {
        this.psm = psm;
    }

    public virtual void EnterState()
    {
    }

    public virtual void UpdateState()
    {
    }

    public virtual void ExitState()
    {
    }

}
