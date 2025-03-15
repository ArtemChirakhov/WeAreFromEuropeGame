using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;

public class GearPickup : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 5f;
    public float pickupDistance = 0.1f;

    private Transform player;
    private NavMeshAgent agent;
    private bool isAttracted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден на GearPickup");
            return;
        }
        agent.speed = moveSpeed;
        agent.angularSpeed = 240f;   // Плавнее поворот
        agent.acceleration = 8f;     // Реалистичнее набор скорости
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = pickupDistance + 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= pickupDistance)
        {
            CurrencyManager.Instance.AddGear(1);
            Destroy(gameObject);
        }  
    }
}
