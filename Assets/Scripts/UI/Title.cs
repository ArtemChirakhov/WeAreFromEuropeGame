using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{

    public static void GoToMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void ExitGame()
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
