using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public static class PlayerStateMachine
{

    public static PlayerState playerState;

    public enum PlayerState
    {
        Idle = 0,
        Moving = 1,
        Dashing = 2
    }

    public static PlayerState GetPlayerState()
    {
        return playerState;
    }

    public static void Manage()
    {

    }
}
