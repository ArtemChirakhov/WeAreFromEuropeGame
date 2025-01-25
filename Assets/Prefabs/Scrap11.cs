using UnityEngine;

public class Scrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
    if (collision.CompareTag("Player"))
    {
        Scraps.Instance.CollectScrap();
        Destroy(gameObject); // Удаляем "Scrap" после подбора
    }
    }

}
