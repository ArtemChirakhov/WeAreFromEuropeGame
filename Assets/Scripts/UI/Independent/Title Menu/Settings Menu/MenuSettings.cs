using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuSettings : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("MenuTitle");
    }

    void Start()
    {
        SceneManager.LoadScene("MenuTitle");

    }

    public abstract class ControlsData
    {
        private static Dictionary<String, String> defaultKeybinds = new()
        {
            ["Up"] = "W",
            ["Left"] = "A",
            ["Down"] = "S",
            ["Right"] = "D",
            ["Pause"] = "Esc"
        };

        public void ChangeKeyBinds()
        {
            Player.playerActions.Player.Disable();
            Player.playerActions.Player.Dash.PerformInteractiveRebinding()
            .OnComplete(callback => {
                Debug.Log(callback);
            })
            .Start();
        }

    }
}