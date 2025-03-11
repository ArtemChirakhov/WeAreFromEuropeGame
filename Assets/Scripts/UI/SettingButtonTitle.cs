using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonTitle : MonoBehaviour
{

    private static readonly Dictionary<String, String> keybindsDefault = new()
    {
        ["Up"] = "W",
        ["Left"] = "A",
        ["Down"] = "S",
        ["Right"] = "D",
        ["Pause"] = "Esc"
    };

    public Dictionary<String, String> keybindsAction = new(keybindsDefault);
    private Dictionary<String, String> GetKeyBinds()
    {
        Dictionary<String, String> keybindsDefault = new()
        {
            ["Up"] = "W",
            ["Left"] = "A",
            ["Down"] = "S",
            ["Right"] = "D",
            ["Pause"] = "Esc"
        };

        return keybindsDefault;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}