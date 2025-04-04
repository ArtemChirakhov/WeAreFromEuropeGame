using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSettings : MonoBehaviour
{
    
    Dictionary<String, String> defaultKeybinds = new()
    {
        ["Up"] = "W",
        ["Left"] = "A",
        ["Down"] = "S",
        ["Right"] = "D",
        ["Pause"] = "Esc"
    };

    Dictionary<String, String> keyBinds = new(defaultKeybinds);

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