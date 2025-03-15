using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public GameObject gearPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void Die()
    {
        if (gearPrefab != null)
        {
            int gearsCount = UnityEngine.Random.Range(1, 4);
            for (int i = 0; i < gearsCount; i++)
            {
                float randomOffsetX = UnityEngine.Random.Range(-1.5f, 1.5f);
                float randomOffsetY = UnityEngine.Random.Range(-1.5f, 1.5f);

                Vector2 spawnPosition = new Vector2(
                transform.position.x + randomOffsetX, 
                transform.position.y + randomOffsetY
            );

                Instantiate(gearPrefab, spawnPosition, quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
}
