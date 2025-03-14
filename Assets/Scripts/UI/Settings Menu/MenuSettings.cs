using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonTitle : MonoBehaviour
{
    
    public static Dictionary<String, String> GetKeyBinds()
    {
        Dictionary<String, String> DefaultKeybinds = new()
        {
            ["Up"] = "W",
            ["Left"] = "A",
            ["Down"] = "S",
            ["Right"] = "D",
            ["Pause"] = "Esc"
        };

        Dictionary<String, String> ActualKeybinds = new(DefaultKeybinds);
        return ActualKeybinds;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}