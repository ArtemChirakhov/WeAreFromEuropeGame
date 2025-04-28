using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Play : MonoBehaviour
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
