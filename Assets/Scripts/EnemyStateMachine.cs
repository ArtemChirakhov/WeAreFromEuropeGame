using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour // Скрипт для изменения состояний врага: патруль - погоня - атака
{
    private enum States
    {
        Patrol,
        Chase,
        Attack,
        Search
    }
    private States currentState;

    [Header("Vision Settings")]
    [SerializeField] private float visionRadius = 20f; 
    [SerializeField] private float fovAngle = 360f; // Угол обзора в градусах

    [Header("Attack Settings")]
    [SerializeField] private float attackRadius = 1f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Массив патрульных точек
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Search settings")]
    [SerializeField] private float searchTime = 5f;
    [SerializeField] private float searchSpeed = 3.5f;

    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask obstacleLayer; // Слои, которые Raycast будет учитывать
    private float searchTimer = 0f;
    private Transform currentTargetPoint;
    private NavMeshAgent agent;

    private Vector3 playerLastSeenPosition = Vector3.zero;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentState = States.Patrol;
    }

    void FixedUpdate()
    {   
        UpdateState();
        Debug.Log(currentState);
        switch (currentState)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Chase:
                Chase();
                break;
            case States.Attack:
                Attack();
                break;
            case States.Search:
                Search();
                break;
        }
    }

    private void UpdateState()
    {
        bool playerInFOV = IsPlayerInFOV();
        bool playerVisible = false;

        if (playerInFOV)
        {
            playerVisible = HasLineOfSight();
        }

        if (playerVisible)
        {
            playerLastSeenPosition = target.position;
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance < attackRadius)
            {
                currentState = States.Attack;
            }
            else
            {
                currentState = States.Chase;
            }
        }
        else
        {
            if (currentState == States.Chase || currentState == States.Attack)
            {
                currentState = States.Search;
            }
            else if (currentState == States.Search)
            {

            }
            else
            {
                currentState = States.Patrol;
            }
        }
    }

    /// Проверяет находится ли игрок в пределах fov врага
    /// Возвращает true если игрок в FOV иначе false
    private bool IsPlayerInFOV()
{
    Vector2 directionToPlayer = target.position - transform.position;
    float distanceToPlayer = directionToPlayer.magnitude;
    if (distanceToPlayer > visionRadius)
    {
        return false;
    }


    Vector2 enemyForward = transform.up;
    float angleToPlayer = Vector2.Angle(enemyForward, directionToPlayer);

    if (angleToPlayer > fovAngle / 2f)
    {
        return false;
    }

    return true;
}

private bool HasLineOfSight()
{
    Vector2 direction = (target.position - transform.position).normalized;
    float distance = Vector2.Distance(transform.position, target.position);

    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer); 
    if (hit.collider != null)
    {
        if (hit.collider.transform == target)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}



    private void Patrol()
    {   
        agent.speed = patrolSpeed;

        if (currentTargetPoint == null && patrolPoints.Length > 0)
        {
            SelectNextPatrolPoint();
        }

        if (currentTargetPoint == null)
            return;

        agent.SetDestination(currentTargetPoint.position);

        if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.1f)
        {
            SelectNextPatrolPoint();
        }
    }


    private void SelectNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        int nextPatrolPointIndex = Random.Range(0, patrolPoints.Length);
        currentTargetPoint = patrolPoints[nextPatrolPointIndex];
    }

    private void Chase()
    {   
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);
    }

    private void Search()
    {
        agent.speed = chaseSpeed;
        float distanceToSearchPoint = Vector2.Distance(transform.position, playerLastSeenPosition);
        agent.SetDestination(playerLastSeenPosition);
        if (distanceToSearchPoint < 0.1f)
        {
            searchTimer += Time.deltaTime;
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            agent.speed = searchSpeed;
            agent.SetDestination(playerLastSeenPosition + randomOffset);
            if(searchTimer >= searchTime)
            {
                searchTimer = 0f;
                currentState = States.Patrol;
            }
        }
    }
    private void Attack()
    {
    }

/// <summary>
/// визуализация всего в редакторе
/// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fovAngle / 2f) * transform.up;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fovAngle / 2f) * transform.up;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRadius);

        if (currentState == States.Chase || currentState == States.Attack)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
        if(currentState == States.Search)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerLastSeenPosition, target.position);
        }
    }
}
