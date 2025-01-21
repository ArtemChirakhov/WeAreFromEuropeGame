using UnityEngine;

public class FollowPlayer : MonoBehaviour // скрипт чтобы камера следовала за игроком
{
    [SerializeField] private GameObject Player;
    [SerializeField] private Vector2 boxSize = new Vector2(6, 4); // размеры bounding box для камеры
    [SerializeField] private float smoothingFactor = 2f; 
    [SerializeField] private float predictionMultiplier = 0.3f; 
    [SerializeField] private float stopThreshold = 0.0001f; // порог скорости ниже которого считаем что игрок стоит на месте
    private Rigidbody2D playerRb;
    private Camera _camera;

    private void Awake()
    {
        playerRb = Player.GetComponent<Rigidbody2D>(); // проверяем есть ли rb тк потом у нас может пропадать моделька во время чего нибудь вдруг
        _camera = Camera.main;
    }
    
    void FixedUpdate()
    {
        Vector3 playerPosition = Player.transform.position;
        float speed = playerRb ? playerRb.linearVelocity.magnitude : 0f; // получаем скорость игрока

        Vector3 targetPosition;
        if (speed < stopThreshold) // иф стейтмент чтобы проверить стоит ли игрок на месте и если да камера полетит к нему
        {
            // если игрок стоит на месте целевая позиция — позиция игрока
            targetPosition = new Vector3(playerPosition.x, playerPosition.y, transform.position.z);
        }
        else
        {
            // предсказываем позицию игрока
            Vector2 velocity = playerRb ? playerRb.linearVelocity : Vector2.zero;
            Vector3 predictedPosition = playerPosition + (Vector3)velocity * predictionMultiplier;
            
            // определяем ббоксы
            Vector3 cameraCenter = transform.position;
            cameraCenter.z = playerPosition.z; 
            Vector3 minBounds = new Vector3(transform.position.x - boxSize.x / 2, transform.position.y - boxSize.y / 2, cameraCenter.z);
            Vector3 maxBounds = new Vector3(transform.position.x + boxSize.x / 2, transform.position.y + boxSize.y / 2, cameraCenter.z);
            
            // если предсказанная позиция выходит за ббокс, ставим её, иначе оставляем положение игрока
            float targetX = (predictedPosition.x > maxBounds.x || predictedPosition.x < minBounds.x) ? predictedPosition.x : playerPosition.x;
            float targetY = (predictedPosition.y > maxBounds.y || predictedPosition.y < minBounds.y) ? predictedPosition.y : playerPosition.y;
            targetPosition = new Vector3(targetX, targetY, transform.position.z);
        }

        // добавляем сглаживание к движению камеры основываясь на времени и модификаторе сглаживания
        Vector3 delta = targetPosition - transform.position;
        Vector3 step = delta * (1 - Mathf.Exp(-smoothingFactor * Time.deltaTime));
        transform.Translate(step);
    }

    private void OnDrawGizmos() // рисуем ббоксы в редакторе (не в игре)
    {
        if (Application.isPlaying)
            return;
        Gizmos.color = Color.blue;
        Vector3 cameraCenter = transform.position;
        Vector3 currentMinBounds = new Vector3(transform.position.x - boxSize.x / 2, transform.position.y - boxSize.y / 2, cameraCenter.z);
        Vector3 currentMaxBounds = new Vector3(transform.position.x + boxSize.x / 2, transform.position.y + boxSize.y / 2, cameraCenter.z);
        Gizmos.DrawLine(new Vector3(currentMinBounds.x, currentMinBounds.y, 0), new Vector3(currentMinBounds.x, currentMaxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(currentMinBounds.x, currentMaxBounds.y, 0), new Vector3(currentMaxBounds.x, currentMaxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(currentMaxBounds.x, currentMaxBounds.y, 0), new Vector3(currentMaxBounds.x, currentMinBounds.y, 0));
        Gizmos.DrawLine(new Vector3(currentMaxBounds.x, currentMinBounds.y, 0), new Vector3(currentMinBounds.x, currentMinBounds.y, 0));
    }
}
