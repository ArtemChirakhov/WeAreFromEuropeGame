using System.Collections.Generic;
using UnityEngine;

public class EnemyCoordinator : MonoBehaviour
{
    public static EnemyCoordinator Instance { get; private set; }

    [Header("Player & Engagement Settings")]
    [Tooltip("Ссылка на трансформ игрока (назначается в инспекторе).")]
    public Transform player;
    
    [Tooltip("Радиус вокруг игрока, в котором может находиться ограниченное число врагов.")]
    public float engagementRadius = 5f;
    
    [Tooltip("Максимальное число врагов, которые могут находиться в engagementRadius одновременно.")]
    public int maxEnemiesInEngagementZone = 3;

    private readonly List<EnemyStateMachine> allEnemies = new List<EnemyStateMachine>();

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

    /// <summary>
    /// Оповещает врагов о позиции игрока в радиусе alertRadius.
    /// </summary>
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

    private void Update()
    {
        // Если не задан игрок, то пропускаем логику
        if (player == null) return;

        // 1) Собираем всех врагов, которые находятся внутри engagementRadius вокруг игрока
        List<EnemyStateMachine> enemiesInRange = new List<EnemyStateMachine>();
        
        foreach (var enemy in allEnemies)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);
            if (distanceToPlayer <= engagementRadius)
            {
                enemiesInRange.Add(enemy);
            }
        }

        // 2) Сортируем по дистанции к игроку, чтобы ближайшие получили слоты первыми
        enemiesInRange.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, player.position);
            float distB = Vector3.Distance(b.transform.position, player.position);
            return distA.CompareTo(distB);
        });

        // 3) Выдаём слот на «атаку/преследование» ограниченному количеству врагов
        // Все, кто не попал в этот список (или не хватает слотов), переходят в состояние Wait
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            // Если враг попадает в топ по расстоянию, даём ему право «engage»
            if (i < maxEnemiesInEngagementZone)
            {
                enemiesInRange[i].SetCanEngage(true);
            }
            else
            {
                enemiesInRange[i].SetCanEngage(false);
            }
        }

        // 4) Всем остальным врагам вне engagementRadius даём «canEngage = true», 
        //    чтобы они могли самостоятельно (по своим условиям) переходить в погоню, патруль и т.д.
        foreach (var enemy in allEnemies)
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemy.SetCanEngage(true);
            }
        }
    }
}
