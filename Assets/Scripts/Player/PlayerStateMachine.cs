using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{

    public PlayerInput playerInput;
    public PlayerState playerState;

    public enum PlayerState
    {
        Idle = 0,
        Moving = 1,
        Dashing = 2
    }

    void Update()
    {
        
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }
}
