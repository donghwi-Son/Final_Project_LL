using UnityEngine;

public class PlayerStateMachine
{
    private PlayerState currentState;
    public PlayerController player;
    public PlayerStateMachine(PlayerController playerController)
    {
        this.player = playerController;
    }

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

    public void Update()
    {
        currentState.UpdateState();
    }
}
