using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{

    [SerializeField] Player player;

    public void GoBack()
    {
        SceneManager.LoadScene("MenuTitle");
    }
    
    void Start()
    {
        
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

    // public void ChangeKeybinds()
    // {
    //     player.playerActions.Player.Disable();
    //     player.playerActions.Player.Dash.PerformInteractiveRebinding()
    //     .OnComplete(callback => {
    //         Debug.Log(callback);
    //         callback.Dispose();
    //         player.playerActions.Player.Enable();
    //     })
    //     .Start();
    // }
}