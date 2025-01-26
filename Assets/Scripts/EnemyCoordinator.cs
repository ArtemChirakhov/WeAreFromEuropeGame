using System.Collections.Generic;
using UnityEngine;

public class EnemyCoordinator : MonoBehaviour
{
    public static EnemyCoordinator Instance { get; private set; }
    
    private List<EnemyStateMachine> allEnemies = new List<EnemyStateMachine>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy(EnemyStateMachine enemy)
    {
        allEnemies.Add(enemy);
    }
    
    public void UnregisterEnemy(EnemyStateMachine enemy)
    {
        allEnemies.Remove(enemy);
    }

    public void Alert(Vector3 alertPosition, float alertRadius)
    {
        foreach (var enemy in allEnemies)
        {
            float dist = Vector3.Distance(alertPosition, enemy.transform.position);
            if (dist < alertRadius)
            {
                enemy.OnAlertReaction(alertPosition);
            }
        }
    }
}
