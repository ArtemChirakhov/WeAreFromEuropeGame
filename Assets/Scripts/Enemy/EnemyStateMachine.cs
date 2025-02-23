using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Timeline;

/// <summary>
/// Скрипт для изменения состояний врага: патруль - погоня - атака - поиск - ожидание (Wait).
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
    #region Attack variables
    [Header("Attack Settings")]
    public int damage = 20;            // Attack damage
    public float attackSpeed = 0.5f;   // Duration of the attack animation
    public float attackCooldown = 1f; // Cooldown between attacks
    public float attackRadius = 10f;
    private float lastAttackTime = -Mathf.Infinity; // Last time an attack was made
    private GameObject enemyAttackHitbox;   // Reference to the attack hitbox
    private bool isAttacking = false;  // Is the player currently attacking?
    private float attackTimer = 0f;    // Timer for tracking attack animation
    #endregion

    private enum States
    {
        Patrol,
        Chase,
        Attack,
        Search,
        Orbit
    }
    private States currentState;

    [Header("Vision Settings")]
    [SerializeField] private float visionRadius = 20f;
    [SerializeField] private float fovAngle = 360f; // Угол обзора в градусах

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Массив патрульных точек
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float alertRadius = 10f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Search Settings")]
    [SerializeField] private float searchTime = 5f;
    [SerializeField] private float searchSpeed = 3.5f;

    [Header("Target Settings")]
    public Transform target;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitSpeed = 20f;
    [SerializeField] private float orbitDistOffset = 1f;
    private float currentOrbitAngle;
    private float orbitDirection;


    [Header("Raycast Settings")]
    [SerializeField] private LayerMask obstacleLayer; // Слои, которые Raycast будет учитывать
    [Header("Separation settings")]
    [SerializeField] private float separationRadius = 1.0f;
    [SerializeField] private float separationStrength = 1.0f;
    // Для управления логикой
    private float searchTimer = 0f;
    private Transform currentTargetPoint;
    [SerializeField] private NavMeshAgent agent;
    private Vector3 alertPosition;

    private Vector3 playerLastSeenPosition = Vector3.zero;

    // Новое поле: можно ли врагу «атаковать/преследовать» игрока в данный момент?
    private bool canEngage = true;

    void Start()
    {
        // Регистрируем врага в координаторе
        EnemyCoordinator.Instance.RegisterEnemy(this);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        enemyAttackHitbox = transform.GetChild(0).gameObject;

        // Начальное состояние
        currentState = States.Patrol;
        currentOrbitAngle = UnityEngine.Random.Range(0, 360);
        orbitDirection = (UnityEngine.Random.value < 0.5f) ? 1f : -1f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateState();
        switch (currentState)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Chase:
                Chase();
                break;
            case States.Search:
                Search();
                break;
            case States.Orbit:
                Orbit();
                break;

        }
    }

    void Update()
    {
        if (currentState == States.Attack && CanAttack())
        {
            Attack();
            lastAttackTime = Time.time;
        }
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer > 1 / attackSpeed)
            {
                attackTimer = 0f;
                isAttacking = false;
                enemyAttackHitbox.SetActive(isAttacking);
            }
        }
        Debug.Log(currentState);
    }

    /// <summary>
    /// Основная логика переключения состояний.
    /// </summary>
    /// 

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
            alertPosition = target.position;

            // Оповещаем всех врагов в радиусе alertRadius
            EnemyCoordinator.Instance.Alert(alertPosition, alertRadius);

            float distance = Vector3.Distance(target.position, transform.position);

            // Если мы можем "engage", то переходим в Chase или Attack
            if (canEngage)
            {
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
                // ВАЖНО: если видим игрока, но слот "engage" нам недоступен, 
                // чтобы враг не входил в зону - переводим в Wait
                currentState = States.Orbit;
            }
        }
        else
        {
            // Если игрока не видно
            if (currentState == States.Chase || currentState == States.Attack)
            {
                currentState = States.Search;
            }
            else if (currentState == States.Search)
            {
                // Пока не истечет searchTime, враг будет ходить по последней точке
            }
            else if (currentState == States.Orbit)
            {
                // Если раньше стоял в Wait, а потом игрок пропал из виду - 
                // можно вернуться в Patrol (или оставить Wait, 
                // в зависимости от нужд игры)
                currentState = States.Patrol;
            }
            else
            {
                currentState = States.Patrol;
            }
        }
        if (!playerVisible)
        {
            Debug.Log("Player not visible. FOV: " + IsPlayerInFOV() + ", LOS: " + HasLineOfSight());
        }
    }


    /// <summary>
    /// Проверяет, находится ли игрок в пределах угла обзора fovAngle и радиуса visionRadius.
    /// </summary>
    /// <returns>true, если в поле зрения; иначе false</returns>
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

    /// <summary>
    /// Raycast проверка на наличие прямой видимости до игрока (нет препятствий).
    /// </summary>
    private bool HasLineOfSight()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);
        if (hit.collider != null)
        {
            // Check if the hit collider is the player or part of the player
            if (hit.collider.gameObject == target.gameObject || hit.collider.transform.IsChildOf(target))
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
            return true;
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
        if (patrolPoints.Length == 0) return;

        int nextPatrolPointIndex = UnityEngine.Random.Range(0, patrolPoints.Length);
        currentTargetPoint = patrolPoints[nextPatrolPointIndex];
    }

    private void Chase()
    {
        agent.speed = chaseSpeed;
        Vector2 targetPosition = target.position;
        Vector2 separationOffset = Vector2.zero;
        foreach (var other in EnemyCoordinator.Instance.GetAllEnemies())
        {
            if (other == this) continue;
            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance < separationRadius && distance > 0f)
            {
                Vector2 direction = (transform.position - other.transform.position).normalized;
                float factor = (separationRadius - distance) / separationRadius;
                separationOffset += direction * factor * separationStrength;
            }
        }
        float maxSeparation = 2f;
        if (separationOffset.magnitude > maxSeparation)
        {
            separationOffset = separationOffset.normalized * maxSeparation;
        }
        Vector2 finalDestination = targetPosition + separationOffset;

        agent.SetDestination(finalDestination);
    }
        
    

    private void Search()
    {
        agent.speed = chaseSpeed;
        float distanceToSearchPoint = Vector2.Distance(transform.position, playerLastSeenPosition);
        agent.SetDestination(playerLastSeenPosition);

        // Когда дошли до точки, «прочёсываем» местность
        if (distanceToSearchPoint < 0.1f)
        {
            searchTimer += Time.deltaTime;
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
            agent.speed = searchSpeed;
            agent.SetDestination(playerLastSeenPosition + randomOffset);

            if (searchTimer >= searchTime)
            {
                searchTimer = 0f;
                currentState = States.Patrol;
            }
        }
    }


    private void Orbit()
    {
        agent.speed = chaseSpeed;
        if (EnemyCoordinator.Instance == null || EnemyCoordinator.Instance.player == null)
        {
            agent.SetDestination(transform.position);
            return;
        }
        
        float orbitRadius = EnemyCoordinator.Instance.engagementRadius + orbitDistOffset;
        Vector2 center = EnemyCoordinator.Instance.player.position;
        currentOrbitAngle += orbitDirection * orbitSpeed * Time.deltaTime;
        float angleRad = currentOrbitAngle * Mathf.Deg2Rad;

        float x = Mathf.Cos(angleRad) * orbitRadius;
        float y = Mathf.Sin(angleRad) * orbitRadius;

        Vector2 orbitPosition = new Vector2(center.x + x, center.y + y);

        Vector2 separationOffset = Vector2.zero;
        foreach (var other in EnemyCoordinator.Instance.GetAllEnemies())
        {
            if (other == this) continue;
            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance < separationRadius && distance > 0f)
            {
                Vector2 direction = (transform.position - other.transform.position).normalized;
                float factor = (separationRadius - distance) / separationRadius;
                separationOffset += direction * factor * separationStrength;
            }
        }
        float maxSeparation = 2f;
        if (separationOffset.magnitude > maxSeparation)
        {
            separationOffset = separationOffset.normalized * maxSeparation;
        }
        Vector2 finalDestination = orbitPosition + separationOffset;

        agent.SetDestination(finalDestination);
    }

    private void OnDestroy()
    {
        // Снимаем с регистрации при уничтожении
        if (EnemyCoordinator.Instance != null)
        {
            EnemyCoordinator.Instance.UnregisterEnemy(this);
        }
    }

    /// <summary>
    /// Когда враг получает сигнал тревоги от координатора.
    /// Если он в состоянии Patrol или Search, переходим в поиск места, где был замечен игрок.
    /// </summary>
    public void OnAlertReaction(Vector3 position)
    {
        if (currentState == States.Patrol || currentState == States.Search)
        {
            playerLastSeenPosition = position;
            currentState = States.Search;
        }
    }

    /// <summary>
    /// С помощью этого метода координатор даёт врагу «право» входить в Chase/Attack.
    /// Если false — враг будет переключаться в состояние Wait, даже если видит игрока.
    /// </summary>
    public void SetCanEngage(bool value)
    {
        canEngage = value;
    }

    /// <summary>
    /// Визуализация радиусов и углов обзора в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Радиус обзора
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        // Угол обзора
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fovAngle / 2f) * transform.up;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fovAngle / 2f) * transform.up;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRadius);

        // Линия до игрока (если Chase/Attack/Search)
        if (currentState == States.Chase || currentState == States.Attack)
        {
            Gizmos.color = Color.red;
            if (target != null)
            {
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
        else if (currentState == States.Search)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerLastSeenPosition);
        }
    }

    #region Attack
    void Attack()
    {
        isAttacking = true;
        enemyAttackHitbox.SetActive(isAttacking);
    }

    bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }
    #endregion
    
}
