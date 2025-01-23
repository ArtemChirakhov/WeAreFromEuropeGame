
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Timeline;

public class EnemyStateMachine : MonoBehaviour //скрипт для изменения состояний врага патруль - погоня - атака
{
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
    [SerializeField] Transform target;
    private Transform currentTargetPoint;
    NavMeshAgent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentState = States.Patrol;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector3.Distance(target.position, transform.position);
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
            case States.Attack:
                Attack();
                break;

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
        agent.speed = patrolSpeed;

        if (currentTargetPoint == null && patrolPoints.Length > 0)
        {
            int nextPatrolPointIndex = Random.Range(0, patrolPoints.Length); //выбираем случайную точку из массива патрульных точек
            currentTargetPoint = patrolPoints[nextPatrolPointIndex];
        }

        if (currentTargetPoint == null)
        return;

        agent.SetDestination(currentTargetPoint.position); // ?? PatrolSpeed * Time.deltaTime ??

        if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.1f) //проверяем насколько близко подошел враг к точке
        {
            int nextPatrolPointIndex = Random.Range(0, patrolPoints.Length); //если достаточно близко - выбираем новую
            currentTargetPoint = patrolPoints [nextPatrolPointIndex];
        }
    }
    private void Chase()
    {   
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position); // chaseSpeed * Time.deltaTime ??
    }
    private void Attack()
    {

    }
}
