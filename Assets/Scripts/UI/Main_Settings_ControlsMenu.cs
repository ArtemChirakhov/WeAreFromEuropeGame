using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Main_Settings_ControlsMenu : MonoBehaviour
{
    public static void GoToMenu(string sceneName)
    {
        GameHandler.SwitchScene(sceneName);
    }

    public void GoBackButtonPressed()
    {
        SceneManager.LoadScene("Main_Settings");
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

    public void ChangeKeybinds()
    {

        GameHandler.playerActions.Player.Disable();
        
        var rebindOpertion = GameHandler.playerActions.Player.PerformInteractiveRebinding();        
        rebindOpertion.OnComplete(callback => {
            Debug.Log(callback);
            callback.Dispose();
        })

        GameHandler.playerActions.Player.Enable();
        

        // GameHandler.playerActions.Player.Disable();
        // GameHandler.playerActions.Player.Dash.PerformInteractiveRebinding()
        // .OnComplete(callback => {
        //     Debug.Log(callback);
        //     callback.Dispose();
        //     GameHandler.playerActions.Player.Enable();
        // })
        // .Start();
    }
}