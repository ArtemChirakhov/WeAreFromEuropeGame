using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    public void GoBackButton()
    {
        SceneManager.LoadScene("MenuSettings");
    }
    
    void Start()
    {
        ChangeKeybinds();
    }

    void Update()
    {
     
    }

    private static Dictionary<String, String> defaultKeybinds = new()
    {
        ["Up"] = "W",
        ["Left"] = "A",
        ["Down"] = "S",
        ["Right"] = "D",
        ["Pause"] = "Esc"
    };

    public void ChangeKeybinds()
    {
        Player.playerActions.Player.Disable();
        Player.playerActions.Player.Dash.PerformInteractiveRebinding()
        .OnComplete(callback => {
            Debug.Log(callback);
            callback.Dispose();
            Player.playerActions.Player.Enable();
        })
        .Start();
    }
}