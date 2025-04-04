using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void ExitGame()
    {
        Debug.Log("Ragequit");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
