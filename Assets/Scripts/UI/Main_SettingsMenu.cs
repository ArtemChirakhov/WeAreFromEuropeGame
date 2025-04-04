using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuSettings : MonoBehaviour
{
    public void GoBackButton()
    {
        SceneManager.LoadScene("MenuTitle");
    }

    public static void GoToMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void Start()
    {

    }

    void Update()
    {
    
    }
}