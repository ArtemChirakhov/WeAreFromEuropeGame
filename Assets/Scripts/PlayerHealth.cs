using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour 
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider; //UI element healthbar
    public GameObject deathScreen; //UI element death and restart screen 
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth == 0)
            Die();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
    
    void Die()
    {
        Debug.Log("You died");
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
