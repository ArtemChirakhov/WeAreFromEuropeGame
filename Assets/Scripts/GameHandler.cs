using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{

    public static GameHandler _instance { get; private set; }
    public static MainActionMap playerActions;

    private static Dictionary<String, String> defaultKeybinds = new()
    {
        ["Up"] = "W",
        ["Left"] = "A",
        ["Down"] = "S",
        ["Right"] = "D",
        ["Pause"] = "Esc"
    };

    public static Dictionary<String, String> currentKeybinds = new(defaultKeybinds);
    
    void Awake()
    {
        playerActions = new();
    
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void Update()
    {
        
    }
}
