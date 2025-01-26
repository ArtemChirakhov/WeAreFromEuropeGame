using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Timeline;

public class EnemyStateMachine : MonoBehaviour //скрипт для изменения состояний врага патруль - погоня - атака
{
    #region Attack variables
    [Header("Attack Settings")]
    public int damage = 20;            // Attack damage
    public float attackSpeed = 0.5f;   // Duration of the attack animation
    public float attackCooldown = 1f; // Cooldown between attacks
    private float lastAttackTime = -Mathf.Infinity; // Last time an attack was made
    private GameObject enemyAttackHitbox;   // Reference to the attack hitbox
    private bool isAttacking = false;  // Is the player currently attacking?
    private float attackTimer = 0f;    // Timer for tracking attack animation
    #endregion

    private enum States
    {
        Patrol,
        Chase,
        Attack
    }
    private States currentState;
    [SerializeField] private float visionRadius = 10f; 
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private Transform[] patrolPoints; //массив патрульных точек
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    private Transform currentTargetPoint;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = States.Patrol;
        enemyAttackHitbox = transform.GetChild(0).gameObject;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        float chaseThreshold = visionRadius;
        float attackThreshold = attackRadius;

        CheckDistanceToDefineState(distance, chaseThreshold, attackThreshold);

        switch (currentState)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Chase:
                Chase();
                break;

        }
    }

    void Update()
    {
        if (currentState == States.Attack)
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
    }
    
    private void CheckDistanceToDefineState(float distance, float chaseThreshold, float attackThreshold)
    {
        if (distance < attackThreshold)
            currentState = States.Attack;
        else if (distance < chaseThreshold && distance > attackThreshold)
            currentState = States.Chase;
        else
            currentState = States.Patrol;
    }
    private void Patrol()
    {   
        if (currentTargetPoint == null && patrolPoints.Length > 0)
        {
            int nextPatrolPointIndex = Random.Range(0, patrolPoints.Length); //выбираем случайную точку из массива патрульных точек
            currentTargetPoint = patrolPoints[nextPatrolPointIndex];
        }

        if (currentTargetPoint == null)
        return;

        transform.position= Vector2.MoveTowards(transform.position, currentTargetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.1f) //проверяем насколько близко подошел враг к точке
        {
            int nextPatrolPointIndex = Random.Range(0, patrolPoints.Length); //если достаточно близко - выбираем новую
            currentTargetPoint = patrolPoints [nextPatrolPointIndex];
        }
    }
    private void Chase()
    {   
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
    }

    #region Attack
    private void Attack()
    {
        isAttacking = true;
        enemyAttackHitbox.SetActive(isAttacking);
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }
    #endregion
}
