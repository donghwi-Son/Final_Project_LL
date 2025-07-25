public class StateMachine<T> where T : Entity
{
    public State<T> CurrentState { get; private set; }
    public State<T> PreviousState { get; private set; }

    public void ChangeState(State<T> newState)
    {
        CurrentState?.Exit();
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState.Enter();
    }
}
