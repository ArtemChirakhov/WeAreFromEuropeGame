using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public static void GoToMenu(string sceneName)
    {
        GameHandler.SwitchScene(sceneName);
    }
    
    public void ExitButtonPressed()
    {
        Debug.Log("Ragequit");
    }
    
    void Awake()
    {        

    }
    
    void Start()
    {

    }

    void Update()
    {

    }
}
