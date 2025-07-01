


public class PlayerState
{
    protected PlayerStateMachine psm;
    protected PlayerState(PlayerStateMachine psm)
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
