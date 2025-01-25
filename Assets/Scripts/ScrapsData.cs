using UnityEngine;
using UnityEngine.UI;
public class ScrapsData : MonoBehaviour
{
    public static Scraps Instance;
    [Header("UI")]
    [SerializeField] private Text scrapsText;

    [Header("Scrap Prefab")]
    [SerializeField] private GameObject scrapPrefab;

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

    private void SpawnScrap(Vector3 position)
    {
        Instantiate(scrapPrefab, position, Quaternion.identity);
    }

    public void CollectScrap()
    {
        PlayerData.currentScraps++;
        UpdateScrapUI();
    }
    
    private void UpdateScrapUI()
    {
        if (scrapsText != null)
        {
            scrapsText.text = $"Scraps: {PlayerData.currentScraps}";
        }
    }
}
