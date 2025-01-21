using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private Vector2 boxSize = new Vector2(5, 3); // размеры bounding box для камеры
    [SerializeField] private float smoothingFactor = 2f; 
    [SerializeField] private float predictionMultiplier = 0.3f; 
    private Rigidbody2D playerRb;
    private Camera _camera;

    private void Awake()
    {
        playerRb = Player.GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }
    
    void FixedUpdate()
    {
        Vector3 playerPosition = Player.transform.position;
        // я проверяю есть ли rb тк потом у нас может пропадать моделька во время чего нибудь вдруг
        Vector2 velocity = playerRb ? playerRb.linearVelocity : Vector2.zero;
        // предсказываем позицию игрока
        Vector3 predictedPosition = playerPosition + (Vector3)velocity * predictionMultiplier;
        
        // определяем ббоксы
        Vector3 cameraCenter = transform.position;
        cameraCenter.z = playerPosition.z; 
        Vector3 minBounds = new Vector3(transform.position.x - boxSize.x / 2, transform.position.y - boxSize.y / 2, cameraCenter.z);
        Vector3 maxBounds = new Vector3(transform.position.x + boxSize.x / 2, transform.position.y + boxSize.y / 2, cameraCenter.z);

        // проверяем внутри ббокса ли предсказанная позиция игрока и устанавливаем новую позицию камеры
        float targetX = (predictedPosition.x > maxBounds.x || predictedPosition.x < minBounds.x) ? predictedPosition.x : transform.position.x;
        float targetY = (predictedPosition.y > maxBounds.y || predictedPosition.y < minBounds.y) ? predictedPosition.y : transform.position.y;
        Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);

        // добавляем сглаживание к движению камеру основываясь на времени и модификаторе сглаживания
        Vector3 delta = targetPosition - transform.position;
        Vector3 step = delta * (1 - Mathf.Exp(-smoothingFactor * Time.deltaTime));

        transform.Translate(step);
    }

    private void OnDrawGizmos() //рисуем ббоксы в редакторе (не в игре)
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
