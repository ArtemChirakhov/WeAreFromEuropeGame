using UnityEngine;

public class attackHitbox : MonoBehaviour
{
    private Player player;
    public int damage = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.GetComponent<EnemyHealth>() != null)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(player.damage);
        }
    }
}
