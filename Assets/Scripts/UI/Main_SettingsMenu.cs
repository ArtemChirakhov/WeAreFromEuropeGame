using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_SettingsMenu : MonoBehaviour
{
    public static void GoToMenu(string sceneName)
    {
        GameHandler.SwitchScene(sceneName);
    }

    public void GoBackButtonPressed()
    {
        SceneManager.LoadScene("Main");
    }

    void Start()
    {

    }

    void Update()
    {
    
    }
}