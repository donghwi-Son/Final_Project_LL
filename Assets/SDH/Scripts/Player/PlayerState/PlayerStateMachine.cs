


public class PlayerStateMachine
{
    private PlayerState currentState;

    public void InitState(PlayerState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
