using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSettings : MonoBehaviour
{
    
    public Dictionary<String, String> GetKeyBinds()
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

    public void ChangeKeyBinds()
    {
        
    }
    
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
}