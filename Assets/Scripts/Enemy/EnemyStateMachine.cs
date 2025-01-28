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
    public float attackRadius = 1.5f;
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
        Wait
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

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask obstacleLayer; // Слои, которые Raycast будет учитывать

    // Для управления логикой
    private float searchTimer = 0f;
    private Transform currentTargetPoint;
    private NavMeshAgent agent;
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
            case States.Wait:
                Wait();
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

        // Если мы видим игрока напрямую...
        if (playerVisible)
        {
            playerLastSeenPosition = target.position;
            alertPosition = target.position;

            // Оповещаем всех врагов в радиусе alertRadius
            EnemyCoordinator.Instance.Alert(alertPosition, alertRadius);

            float distance = Vector3.Distance(target.position, transform.position);

            // Если мы можем «engage», то переходим в Chase или Attack
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
                // Если видим игрока, но слот «engage» не доступен — ждём (Wait)
                currentState = States.Wait;
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
                // Если мы ищем, то продолжаем искать, пока таймер не выйдет.
                // Логика останется в Search, пока не истечёт searchTime.
            }
            else if (currentState == States.Wait)
            {
                // Если мы в режиме Wait и больше не видим игрока,
                // теоретически можно перейти в Patrol или продолжать ждать.
                // Вариант 1 (можно менять под нужды игры):
                currentState = States.Patrol;
            }
            else
            {
                currentState = States.Patrol;
            }
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
            // Если луч упёрся в коллайдер игрока, то линия видимости есть
            if (hit.collider.transform == target)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Если луч никого не задел, значит тоже препятствий нет — значит, видим
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

        // Когда дошли до точки, «прочёсываем» местность
        if (distanceToSearchPoint < 0.1f)
        {
            searchTimer += Time.deltaTime;
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            agent.speed = searchSpeed;
            agent.SetDestination(playerLastSeenPosition + randomOffset);

            if (searchTimer >= searchTime)
            {
                searchTimer = 0f;
                currentState = States.Patrol;
            }
        }
    }

    /// <summary>
    /// Новое состояние ожидания. Враги, которые «видят» игрока, 
    /// но не имеют слот «engage», могут, к примеру, двигаться вокруг зоны или стоять на месте.
    /// </summary>
    private void Wait()
    {
        // В примере сделаем простую логику ожидания на месте или медленного патруля вокруг позиции.
        agent.speed = 1f;
        // Можно задать какую-то минимальную активность или просто стоять.
        agent.SetDestination(transform.position);
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
