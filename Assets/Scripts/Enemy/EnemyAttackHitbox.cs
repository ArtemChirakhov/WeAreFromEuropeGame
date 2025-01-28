using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private EnemyStateMachine enemy;
    public float hitboxDistanceFromEnemy = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = transform.parent.GetComponent<EnemyStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateAndPositionHitbox();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player playerHealth = collider.GetComponent<Player>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(enemy.damage);
        }
    }

    private void RotateAndPositionHitbox()
    {
        Vector3 direction = enemy.target.position - enemy.transform.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = enemy.transform.position +
            new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * hitboxDistanceFromEnemy;

    }
}
