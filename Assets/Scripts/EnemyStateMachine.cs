
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
    private Transform currentTargetPoint;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = States.Patrol;
    }
    // Update is called once per frame
    void Update()
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
        Debug.Log("Patrolling");
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
        Debug.Log("Chasing");
    }
    private void Attack()
    {
        Debug.Log("Attacking");
    }
}
