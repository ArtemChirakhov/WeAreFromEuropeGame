using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int gearCount = 0;
    public TextMeshProUGUI gearCountText;

    private void Awake()
    {
     if (Instance == null)
     {
        Instance = this;
     }   
     else
     {
        Destroy(gameObject);
     }
    }
    void Start()
    {
        UpdateGearUI();
    }
    
    public void AddGear(int amount)
    {
        gearCount += amount;
        UpdateGearUI();
    }

    private void UpdateGearUI()
    {
        if (gearCountText != null){
            gearCountText.text = gearCount.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
