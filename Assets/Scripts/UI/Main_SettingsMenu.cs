using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_SettingsMenu : MonoBehaviour
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
}