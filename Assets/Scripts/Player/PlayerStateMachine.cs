using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine
{

    public PlayerInput playerInput;
    public PlayerState playerState = PlayerState.Idle;

    public enum PlayerState
    {
        Idle = 0,
        Moving = 1,
        Dashing = 2
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }
}
