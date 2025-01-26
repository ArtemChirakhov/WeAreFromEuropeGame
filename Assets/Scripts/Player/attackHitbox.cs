using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private Player player; // Reference to the parent Player script
    public float hitboxDistanceFromPlayer = 1f; // Distance from player at which the attack hitbox will apear 

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Make the hitbox follow the cursor and rotate accordingly
        RotateAndPositionHitbox();
    }

    // Deals damage when an object with EnemyHealth script is inside an active hitbox
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collided object has an EnemyHealth component
        EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Deal damage to the enemy using the player's damage value
            enemyHealth.TakeDamage(player.damage);
        }
    }

    // Adjust the hitbox position and rotation to follow the cursor
    private void RotateAndPositionHitbox()
    {
        // Calculate the direction vector from the player to the cursor
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        direction.z = 0; // Ensure it only operates in 2D

        // Calculate the angle in degrees between the x-axis and the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the hitbox to face the cursor
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Position the hitbox at a fixed distance from the player in the direction of the cursor
        transform.position = player.transform.position +
            new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * hitboxDistanceFromPlayer;
    }
}

