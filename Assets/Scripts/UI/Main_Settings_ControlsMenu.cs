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

    void Start()
    {
        
    }

    void Update()
    {
     
    }

    public void ChangeKeybinds()
    {

        GameHandler.playerActions.Player.Disable();
        
        var rebindOpertion = GameHandler.playerActions.Player.Dash.PerformInteractiveRebinding();        
        rebindOpertion.OnComplete(callback => {
            Debug.Log(callback);
            callback.Dispose();
        });

        GameHandler.playerActions.Player.Enable();
        
    }
}