using System;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : MonoBehaviour
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
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}